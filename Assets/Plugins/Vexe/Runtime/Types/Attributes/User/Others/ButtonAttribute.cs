using System;

namespace Vexe.Runtime.Types
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = true)]
    public class ButtonAttribute : CompositeAttribute
    {
        public readonly string Method;
        public readonly string DisplayText;
        public readonly string Style;

        public ButtonAttribute(string method)
            : this (method, method)
        {
        }

        public ButtonAttribute(string method, string displayText)
            : this(-1, method, displayText)
        {
        }

        public ButtonAttribute(int id, string method, string displayText)
            : this(id, method, displayText, "Button")
        {
        }

        public ButtonAttribute(string method, string displayText, string style)
            : this(-1, method, displayText, style)
        {
        }

        public ButtonAttribute(int id, string method, string displayText, string style)
        {
            this.id = id;
            this.Method = method;
            this.DisplayText = displayText;
            this.Style = style;
        }
    }
}
