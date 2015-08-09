using System;
using UnityEngine;

namespace Vexe.Runtime.Extensions
{
    public static class ComponentExtensions 
    {

        // Works just like GetComponent<T>, except also works with interface types
        public static T GetIComponent<T>(this Component c) where T : class 
        {
            return c.gameObject.GetIComponent<T>();
        }

        // Works just like GetComponentInChildren, except also works with interface types
        public static T GetIComponentInChildren<T>(this Component c) where T : class 
        {
            return c.gameObject.GetIComponentInChildren<T>();
        }

        // Works just like GetComponentInParent, except also works with interface types
        public static T GetIComponentInParent<T>(this Component c) where T : class 
        {
            return c.gameObject.GetIComponentInParent<T>();
        }

        // Works just like GetComponents, except also works with interface types
        public static T[] GetIComponents<T>(this Component c) where T : class 
        {
            return c.gameObject.GetIComponents<T>();
        }

        // Works just like GetComponentInChildren, except also works with interface types
        public static T[] GetIComponentsInChildren<T>(this Component c) where T : class 
        {
            return c.gameObject.GetIComponentsInChildren<T>();
        }

        // Works just like GetComponentsInParent, except also works with interface types
        public static T[] GetIComponentsInParent<T>(this Component c) where T : class 
        {
            return c.gameObject.GetIComponentsInParent<T>();
        }

        public static Component GetOrAddComponent(this Component c, Type componentType)
        {
            return c.gameObject.GetOrAddComponent(componentType);
        }

        public static T GetOrAddComponent<T>(this Component c) where T : Component
        {
            return c.gameObject.GetOrAddComponent<T>();
        }	
    }
}