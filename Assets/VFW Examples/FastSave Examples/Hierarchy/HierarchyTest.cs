using System;
using UnityEngine;
using Vexe.FastSave;
using Vexe.Runtime.Types;

namespace FSExamples
{
    public class HierarchyTest : BaseBehaviour
    {
        public GameObject target;

        [HideInInspector]
        public string output;

        [Show] void SaveHierarchy()
        {
            output = target.SaveHierarchyToMemory().GetString();
        }

        [Show] void LoadIntoNewHierarchy()
        {
            Load.HierarchyFromMemory(output.GetBytes(), new GameObject("ROOT"));
        }

        [Show] void LoadIntoTargetHierarchy()
        {
            Load.HierarchyFromMemory(output.GetBytes(), target);
        }
    }

}
