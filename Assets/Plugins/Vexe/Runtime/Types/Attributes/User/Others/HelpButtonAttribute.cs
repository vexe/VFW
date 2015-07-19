using System;

namespace Vexe.Runtime.Types
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public class HelpButtonAttribute : CompositeAttribute
    {
        public readonly string HelpText;
        public readonly string Label;
        public readonly string ButtonPosition;
        public readonly string HelpPosition;
       
        /// <summary>
        /// Adds a toggleable help button that shows a help box.
        /// </summary>
        /// <param name="helpText">Text to be displayed</param>
        /// <param name="buttonPosition">"Left" or "Right"</param>
        /// <param name="helpPosition">"Upper" or "Lower"</param>
        public HelpButtonAttribute(string helpText, string buttonPosition = "Right", string helpPosition = "Lower")
        {
            this.HelpText = helpText;
            this.Label = helpText;
            this.HelpPosition = helpPosition;
            this.ButtonPosition = buttonPosition;
        }
    }
}