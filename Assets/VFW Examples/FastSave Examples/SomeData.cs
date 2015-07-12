using System;
using System.Collections.Generic;
using UnityEngine;
using Vexe.Runtime.Types;

namespace FSExamples
{
    public class SomeData : BaseBehaviour
    {
        public Container container;
        public string stringField;
        public int[] intArray;
        public List<float> floatList;
        public MeshRenderer testRenderer;
        public GameObject testGameObject;
        public GameObject testPrefab;

        [Serializable]
        public class Container : SerializableDictionary<string, int> { }
    }
}
