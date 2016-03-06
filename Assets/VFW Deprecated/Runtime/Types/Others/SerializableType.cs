using System;
using UnityEngine;

namespace Vexe.Runtime.Types
{ 
    /// <summary>
    /// A wrapper class around System.Type that is serializable by Unity
    /// </summary>
    [Serializable]
    public class SerializableType
    {
        [SerializeField] string _name;

        private Type _value;

        public Type Value
        {
            get
            {
                if(_value == null)
                { 
                    _value = Type.GetType(_name);
                    if (_value == null)
                        Debug.Log("Couldn't load type: " + _name);
                }
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _name = value.AssemblyQualifiedName;
                    _value = value;
                }
            }
        }

        public SerializableType(Type type)
        {
            Value = type;
        }

        public bool HasValidName()
        {
            return !string.IsNullOrEmpty(_name);
        }
    }
}