using UnityEngine;
using System.Collections;
using Vexe.Runtime.Types;

namespace VFWExamples
{
    /// <summary>
    /// There's a couple of ways to specify display options for a vfw object:
    /// 1- Per-instance: By collapsing the script header and using the display popup
    /// 2- Global: By modifyign VFWSettings.cs file
    /// 3- Object Hierarchy: This is what this example demonstrates. Overriding GetDisplayOptions is a mixture between the last two.
    ///                      Say you like the global options, but you want to apply different settings for your 'BaseAIController'
    ///                      and anything that inherits it. You can override GetDisplayOptions in the base controller so that any child
    ///                      class will get the same options. Note that you could still change the display via the popup under the script header.
    ///                      It's just that GetDisplayOptions will act as the inital/default/start options when your object is first created.
    /// </summary>
    public class CustomDisplayOptionsExample : BaseBehaviour
    {
        public int we, are, split; 

        public override CategoryDisplay GetDisplayOptions()
        {
            return base.GetDisplayOptions() | CategoryDisplay.MemberSplitter;
        }
    }
}
