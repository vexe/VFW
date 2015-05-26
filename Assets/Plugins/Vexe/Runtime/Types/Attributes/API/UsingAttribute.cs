using System;

namespace Vexe.Runtime.Types
{
    /// <summary>
    /// Fetches attributes from 'Path' and dynamically adds them to the annotated member
    /// Path must 'either'
    /// 1- Follow the format "X.Y" where "X" is some type, and "Y" is a static Attribute[] field in X
    ///    e.g. MyClass.MyAttributes (it doesn't matter if X is static or not, nor what access modifiers are applied on Y)
    /// 2- Or just "X" where X is a static Attribute[] field in a class marked with [AttributeContainer]
    /// See ImportExample.cs
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class UsingAttribute : CompositeAttribute
    {
        public readonly string Path;

        public UsingAttribute(string usingPath)
        {
            this.Path = usingPath;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AttributeContainer : Attribute
    {
    }
}