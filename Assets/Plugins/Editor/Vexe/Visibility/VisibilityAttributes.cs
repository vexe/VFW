using System;
using UnityEngine;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Visibility
{
    public class VisibilityAttributes
    {
        public Type[] Show;
        public Type[] Hide;

        public static readonly VisibilityAttributes Default = new VisibilityAttributes()
        {
            Show = new[]
            {
                typeof(ShowAttribute)
            },

            Hide = new[]
            {
                typeof(HideInInspector),
                typeof(HideAttribute)
            },
        };
    }
}