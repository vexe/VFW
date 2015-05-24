using UnityEngine;

namespace Vexe.Runtime.Extensions
{
    public static class TransformExtensions
    {
        public static void Activate(this Transform t)
        {
            t.gameObject.Activate();
        }

        public static void Deactivate(this Transform t)
        {
            t.gameObject.Deactivate();
        }

        /// <summary>
        /// Sets localPosition to Vector3.zero, localRotation to Quaternion.identity, and localScale to Vector3.one
        /// </summary>
        public static void Reset(this Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }
    }
}
