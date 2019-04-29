using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ConventionMobile.Droid;
using ConventionMobile;
using System;

[assembly: ResolutionGroupName("ConventionMobile")]
[assembly: ExportEffect(typeof(LongPressedEffect_Android), "LongPressedEffect")]
namespace ConventionMobile.Droid
{
    /// <summary>
    /// Android long pressed effect.
    /// </summary>
    public class LongPressedEffect_Android : PlatformEffect
    {
        private bool _attached;

        /// <summary>
        /// Initializer to avoid linking out
        /// </summary>
        public static void Initialize() { }

        /// <summary>
        /// Apply the handler
        /// </summary>
        protected override void OnAttached()
        {
            //because an effect can be detached immediately after attached (happens in listview), only attach the handler one time.
            if (!_attached)
            {
                if (Control != null)
                {
                    Control.LongClickable = true;
                    Control.Clickable = true;
                    Control.LongClick += Control_LongClick;
                    Control.Click += Control_Click;
                }
                else
                {
                    Container.LongClickable = true;
                    Container.Clickable = true;
                    Container.LongClick += Control_LongClick;
                    Container.Click += Control_Click;
                }
                _attached = true;
            }
        }

        private void Control_Click(object sender, EventArgs e)
        {
            var command = LongPressedEffect.GetCommand(Element);
            LongPressedEffect.SetCommandParameter(Element, LongPressedEffect.EffectType.RegularTap);
            command?.Execute(LongPressedEffect.GetCommandParameter(Element));
        }

        /// <summary>
        /// Invoke the command if there is one
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void Control_LongClick(object sender, Android.Views.View.LongClickEventArgs e)
        {
            var command = LongPressedEffect.GetCommand(Element);
            LongPressedEffect.SetCommandParameter(Element, LongPressedEffect.EffectType.LongPress);
            command?.Execute(LongPressedEffect.GetCommandParameter(Element));
        }

        /// <summary>
        /// Clean the event handler on detach
        /// </summary>
        protected override void OnDetached()
        {
            if (_attached)
            {
                if (Control != null)
                {
                    try
                    {
                        Control.LongClickable = false;
                        Control.Clickable = false;
                        Control.LongClick -= Control_LongClick;
                        Control.Click -= Control_Click;
                    }
                    catch (Exception) { }
                }
                else
                {
                    try
                    {
                        Container.LongClickable = false;
                        Container.Clickable = false;
                        Container.LongClick -= Control_LongClick;
                        Container.Click -= Control_Click;
                    }
                    catch (Exception) { }
                }
                _attached = false;
            }
        }
    }
}



