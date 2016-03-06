using System;
using UnityEngine;
using Vexe.Runtime.Helpers;

namespace Vexe.Runtime.Types
{
    public enum RecordValueType
    {
        Int, Float, Bool, String
    }

    // TODO: Show users how to use this
    [Serializable]
    public class DataRecord : SerializableDictionary<string, RecordValue>
    {
    }
    
    [Serializable]
    public struct RecordValue
    {
        // NOTE (Vexe): Should be readonly, but Unity wouldn't be able to serialize 'em then
        [SerializeField] public string ValueString;
        [SerializeField] public float ValueFloat;
        [SerializeField] public RecordValueType Type;

        const float kUnused = -9999f;

        public RecordValue(string value)
        {
            ValueString = value;
            ValueFloat = kUnused;
            Type = RecordValueType.String;
        }

        public RecordValue(int value)
        {
            ValueString = null;
            ValueFloat = value;
            Type = RecordValueType.Int;
        }

        public RecordValue(float value)
        {
            ValueString = null;
            ValueFloat = value;
            Type = RecordValueType.Float;
        }

        public RecordValue(bool value)
        {
            ValueString = null;
            ValueFloat = value ? 1 : 0;
            Type = RecordValueType.Bool;
        }

        public static implicit operator RecordValue(int value) { return new RecordValue(value); }
        public static implicit operator RecordValue(float value) { return new RecordValue(value); }
        public static implicit operator RecordValue(bool value) { return new RecordValue(value); }
        public static implicit operator RecordValue(string value) { return new RecordValue(value); }

        public static implicit operator string (RecordValue value) { return value.ValueString; }
        public static implicit operator int (RecordValue value) { return (int)value.ValueFloat; }
        public static implicit operator float (RecordValue value) { return value.ValueFloat; }
        public static implicit operator bool (RecordValue value) { return (int)value.ValueFloat == 1; }

        public override string ToString()
        {
            if (Type == RecordValueType.String)
                return ValueString;
            return ValueFloat.ToString();
        }
    }

#if UNITY_EDITOR
    [Serializable]
    public class EditorRecord : SerializableDictionary<int, RecordValue>
    {
        new public RecordValue this[int key]
        {
            set
            {
                base[key] = value;
            }
            get
            {
                RecordValue result;

                if (!TryGetValue(key, out result))
                {
                    result = 0;
                    base[key] = 0;
                }

                return result;
            }
        }

        public void SetV2(int key, Vector2 value)
        {
            int keyX = RuntimeHelper.CombineHashCodes(key, "x");
            int keyY = RuntimeHelper.CombineHashCodes(key, "y");
            this[keyX] = value.x;
            this[keyY] = value.y;
        }

        public Vector2 GetV2(int key, Vector2 defaultValue = new Vector2())
        {
            int keyX = RuntimeHelper.CombineHashCodes(key, "x");

            Vector2 result;

            RecordValue x;
            if (!TryGetValue(keyX, out x))
            {
                result = defaultValue;
            }
            else
            {
                int keyY = RuntimeHelper.CombineHashCodes(key, "y");
                result = new Vector2(x, this[keyY]);
            }

            return result;
        }

        public void SetV3(int key, Vector3 value)
        {
            int keyX = RuntimeHelper.CombineHashCodes(key, "x");
            int keyY = RuntimeHelper.CombineHashCodes(key, "y");
            int keyZ = RuntimeHelper.CombineHashCodes(key, "z");
            this[keyX] = value.x;
            this[keyY] = value.y;
            this[keyZ] = value.z;
        }

        public Vector3 GetV3(int key, Vector3 defaultValue = new Vector3())
        {
            int keyX = RuntimeHelper.CombineHashCodes(key, "x");

            Vector3 result;

            RecordValue x;
            if (!TryGetValue(keyX, out x))
            {
                result = defaultValue;
            }
            else
            {
                int keyY = RuntimeHelper.CombineHashCodes(key, "y");
                int keyZ = RuntimeHelper.CombineHashCodes(key, "z");
                result = new Vector3(x, this[keyY], this[keyZ]);
            }

            return result;
        }
    }
#endif

}