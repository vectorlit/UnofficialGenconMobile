using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using ConventionMobile;
using ConventionMobile.iOS;

[assembly: ResolutionGroupName("ConventionMobile")]
[assembly: ExportEffect(typeof(LongPressedEffect_iOS), "LongPressedEffect")]
namespace ConventionMobile.iOS
{
    /// <summary>
    /// iOS long pressed effect
    /// </summary>
    public class LongPressedEffect_iOS : PlatformEffect
    {
        private bool _attached;
        private readonly UILongPressGestureRecognizer _longPressRecognizer;
        private readonly UITapGestureRecognizer _tapGestureRecognizer;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:Yukon.Application.iOSComponents.Effects.iOSLongPressedEffect"/> class.
        /// </summary>
        public LongPressedEffect_iOS()
        {
            _longPressRecognizer = new UILongPressGestureRecognizer();
            _longPressRecognizer.AddTarget(() => this.HandleLongClick(_longPressRecognizer));
            _tapGestureRecognizer = new UITapGestureRecognizer(HandleTap);
        }

        /// <summary>
        /// Apply the handler
        /// </summary>
        protected override void OnAttached()
        {
            //because an effect can be detached immediately after attached (happens in listview), only attach the handler one time
            if (!_attached)
            {
                Container.AddGestureRecognizer(_longPressRecognizer);
                Container.AddGestureRecognizer(_tapGestureRecognizer);
                _attached = true;
            }
        }

        /// <summary>
        /// Invoke the command if there is one
        /// </summary>
        private void HandleLongClick(UILongPressGestureRecognizer longPressGestureRecognizer)
        {
            if (longPressGestureRecognizer.State != UIGestureRecognizerState.Began)
            {
                return;
            }

            var command = LongPressedEffect.GetCommand(Element);
            LongPressedEffect.SetCommandParameter(Element, LongPressedEffect.EffectType.LongPress);
            command?.Execute(LongPressedEffect.GetCommandParameter(Element));
        }

        private void HandleTap()
        {
            var command = LongPressedEffect.GetCommand(Element);
            LongPressedEffect.SetCommandParameter(Element, LongPressedEffect.EffectType.RegularTap);
            command?.Execute(LongPressedEffect.GetCommandParameter(Element));
        }

        /// <summary>
        /// Clean the event handler on detach
        /// </summary>
        protected override void OnDetached()
        {
            if (_attached)
            {
                Container.RemoveGestureRecognizer(_longPressRecognizer);
                Container.RemoveGestureRecognizer(_tapGestureRecognizer);
                _attached = false;
            }
        }

    }
}