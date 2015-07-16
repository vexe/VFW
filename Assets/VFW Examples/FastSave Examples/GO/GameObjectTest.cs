using System;
using UnityEngine;
using Vexe.FastSave;
using Vexe.Runtime.Types;

namespace FSExamples
{
    public class GameObjectTest : BaseBehaviour
    {
        public GameObject target;

        [HideInInspector]
        public string output;

        [Show] void SaveGo()
        {
            output = Save.GameObjectToMemory(target).GetString();
        }

        [Show] void LoadIntoNewGo()
        {
            Load.GameObjectFromMemory(output.GetBytes(), new GameObject());
        }

        [Show] void LoadIntoTargetGo()
        {
            target.LoadFromMemory(output.GetBytes());
        }
    }
}
