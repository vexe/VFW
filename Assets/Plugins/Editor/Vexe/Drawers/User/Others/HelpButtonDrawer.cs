using System;
using System.Reflection;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
    public class HelpButtonDrawer : CompositeDrawer<object, HelpButtonAttribute>
    {
        private string buttonLabel = "?";
        private bool showHelp = false;

        public override void OnRightGUI()
        {
            if (attribute.ButtonPosition != "Left")
            {
               showHelp = gui.ToggleButton(showHelp, buttonLabel, buttonLabel, "Help", MiniButtonStyle.Right);     
            }
        }

        public override void OnLeftGUI()
        {
            if (attribute.ButtonPosition == "Left")
            {
                showHelp = gui.ToggleButton(showHelp, buttonLabel, buttonLabel, "Help", Vexe.Editor.MiniButtonStyle.Right);                        
            }
        }

        public override void OnUpperGUI()
        {
            if (attribute.HelpPosition == "Upper")
            {
                if (showHelp)
                {
                    gui.HelpBox(attribute.HelpText);      
                }
            }
        }

        public override void OnLowerGUI()
        {
            if (attribute.HelpPosition != "Upper")
            {
                if (showHelp)
                {
                    gui.HelpBox(attribute.HelpText);
                }
            }
        }

    }
}