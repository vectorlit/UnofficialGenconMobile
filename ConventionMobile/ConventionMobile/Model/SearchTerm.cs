using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConventionMobile.Model
{
    public class SearchTerm : INotifyPropertyChanged
    {
        public string searchTerm
        {
            get
            {
                return _searchTerm;
            }
            set
            {
                _searchTerm = value;
                OnPropertyChanged(new PropertyChangedEventArgs(searchTerm));
            }
        }

        public void setSearchTermWithoutNotification(string searchTerm)
        {
            this._searchTerm = searchTerm;
            isIgnoringNextEvent = true;
            // Task.Run(async () => { await Task.Delay(3100); isIgnoringNextEvent = false; });
        }

        public bool isIgnoringNextEvent
        {
            get
            {
                bool returnMe = _isIgnoringNextEvent;
                _isIgnoringNextEvent = false;
                return returnMe;
            }
            set
            {
                _isIgnoringNextEvent = value;
            }
        }
        private bool _isIgnoringNextEvent = false;

        private void OnPropertyChanged(PropertyChangedEventArgs propertyChangedEventHandler)
        {
            PropertyChanged?.Invoke(this, propertyChangedEventHandler);
        }

        private string _searchTerm = "";
        
        public SearchTerm()
        {
            initObserver();
        }

        public SearchTerm(string searchTerm)
        {
            this.searchTerm = searchTerm;
            initObserver();
        }

        private void initObserver()
        {
            observer = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    d => this.PropertyChanged += d,
                    d => this.PropertyChanged -= d)
                    .Select(d => d.EventArgs.PropertyName);

            ////Setup our autocomplete
            //observer = this.ToObservable(x => x.searchTerm)                    //Convert the property to an observable
            //                   .Throttle(TimeSpan.FromMilliseconds(350))            //Wait for the user to pause typing 
            //                   .Where(x => !string.IsNullOrEmpty(x) && x.Length > 3)  //Make sure the search term is long enough
            //                   .DistinctUntilChanged();
        }

        public IObservable<string> observer;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        
    }

    public static class PropertyExtensions
    {
        /// <summary>
        /// Gets property information for the specified <paramref name="property"/> expression.
        /// </summary>
        /// <typeparam name="TSource">Type of the parameter in the <paramref name="property"/> expression.</typeparam>
        /// <typeparam name="TValue">Type of the property's value.</typeparam>
        /// <param name="property">The expression from which to retrieve the property information.</param>
        /// <returns>Property information for the specified expression.</returns>
        /// <exception cref="ArgumentException">The expression is not understood.</exception>
        public static PropertyInfo GetPropertyInfo<TSource, TValue>(this Expression<Func<TSource, TValue>> property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            var body = property.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Expression is not a property", "property");
            }

            var propertyInfo = body.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("Expression is not a property", "property");
            }

            return propertyInfo;
        }
    }

    public static class NotificationExtensions
    {
        /// <summary>
        /// Returns an observable sequence of the source any time the <c>PropertyChanged</c> event is raised.
        /// </summary>
        /// <typeparam name="T">The type of the source object. Type must implement <seealso cref="INotifyPropertyChanged"/>.</typeparam>
        /// <param name="source">The object to observe property changes on.</param>
        /// <returns>Returns an observable sequence of the value of the source when ever the <c>PropertyChanged</c> event is raised.</returns>
        public static IObservable<T> OnAnyPropertyChanges<T>(this T source)
            where T : INotifyPropertyChanged
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                               handler => handler.Invoke,
                               h => source.PropertyChanged += h,
                               h => source.PropertyChanged -= h)
                           .Select(_ => source);
        }

        /// <summary>
        /// Returns an observable sequence of the value of a property when <paramref name="source"/> raises <seealso cref="INotifyPropertyChanged.PropertyChanged"/> for the given property.
        /// </summary>
        /// <typeparam name="T">The type of the source object. Type must implement <seealso cref="INotifyPropertyChanged"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property that is being observed.</typeparam>
        /// <param name="source">The object to observe property changes on.</param>
        /// <param name="property">An expression that describes which property to observe.</param>
        /// <returns>Returns an observable sequence of the property values as they change.</returns>
        public static IObservable<TProperty> OnPropertyChanges<T, TProperty>(this T source, Expression<Func<T, TProperty>> property)
            where T : INotifyPropertyChanged
        {
            return Observable.Create<TProperty>(o =>
            {
                var propertyName = property.GetPropertyInfo().Name;
                var propertySelector = property.Compile();

                return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                handler => handler.Invoke,
                                h => source.PropertyChanged += h,
                                h => source.PropertyChanged -= h)
                            .Where(e => e.EventArgs.PropertyName == propertyName)
                            .Select(e => propertySelector(source))
                            .Subscribe(o);
            });
        }

        public static IObservable<R> ToObservable<T, R>(this T target, Expression<Func<T, R>> property) where T : INotifyPropertyChanged
        {
            var body = property.Body;
            var propertyName = "";

            if (body is MemberExpression)
                propertyName = ((MemberExpression)body).Member.Name;
            else if (body is MethodCallExpression)
                propertyName = ((MethodCallExpression)body).Method.Name;
            else
                throw new NotSupportedException("Only use expressions that call a single property or method");

            var getValueFunc = property.Compile();
            return Observable.Create<R>(o => {
                var eventHandler = new PropertyChangedEventHandler((s, pce) => {
                    if (pce.PropertyName == null || pce.PropertyName == propertyName)
                        o.OnNext(getValueFunc(target));
                });
                target.PropertyChanged += eventHandler;
                return () => target.PropertyChanged -= eventHandler;
            });
        }
    }
}
