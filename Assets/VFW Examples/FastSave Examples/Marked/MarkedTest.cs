using System;
using UnityEngine;
using Vexe.FastSave;
using Vexe.Runtime.Types;

namespace FSExamples
{
    public class MarkedTest : BaseBehaviour
    {
        [HideInInspector]
        public string output;

        [Show] void SaveMarked()
        {
            output = Save.MarkedToMemory().GetString();
        }

        [Show] void LoadMarked()
        {
            Load.MarkedFromMemory(output.GetBytes());
        }
    }
}
