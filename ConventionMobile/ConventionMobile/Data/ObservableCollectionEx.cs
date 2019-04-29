using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using ConventionMobile.Model;
using System.Text;
using static ConventionMobile.Model.GenEvent;
using Xamarin.Forms;

namespace ConventionMobile.Data
{
    public class ObservableCollectionEx<T> : ObservableCollection<T> where T : GenEvent
    {
        public ObservableCollectionEx(IList<T> list) : base(list)
        {
            Subscribe(list);
        }

        public ObservableCollectionEx(): base()
        {

        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                Unsubscribe((IList<T>)e.OldItems);
            }
            if (e.NewItems != null)
            {
                Subscribe((IList<T>)e.NewItems);
            }
            base.OnCollectionChanged(e);
        }

        protected override void ClearItems()
        {
            foreach (GenEvent element in this)
            {
                element.OnLongPressChanged -= OnLongPressFound;
                element.OnIsTapped -= OnIsTappedChanged;
            }               

            base.ClearItems();
        }

        private void Subscribe(IList<T> list)
        {
            if (list != null)
            {
                foreach (GenEvent element in list)
                {
                    element.OnLongPressChanged += OnLongPressFound;
                    element.OnIsTapped += OnIsTappedChanged;
                }
            }
        }

        private void OnIsTappedChanged(object sender, TappedEventArgs e)
        {
            OnIsTapped?.Invoke(sender, e);
        }

        public event IsTappedChangedEventHandler OnIsTapped;

        public delegate void IsTappedChangedEventHandler(object sender, TappedEventArgs e);

        private void OnLongPressFound(object sender, GenEvent.LongPressChangedEventArgs e)
        {
            OnLongPressChanged?.Invoke(sender, e);
        }

        public event LongPressChangedEventHandler OnLongPressChanged;

        public delegate void LongPressChangedEventHandler(object sender, LongPressChangedEventArgs e);

        private void Unsubscribe(IList<T> list)
        {
            if (list != null)
            {
                foreach (GenEvent element in list)
                {
                    element.OnLongPressChanged -= OnLongPressFound;
                }
            }
        }

        private void ContainedElementChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }
    }
}
