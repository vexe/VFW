using System;

namespace Vexe.Runtime.Types
{
    /// <summary>
    /// Used in category definitions to determine what type of members we want to included in a category
    /// Values are chosen so that we can cast back and forth between System.Reflection.MemberTypes
    /// </summary>
    [Flags]
    public enum CategoryMemberType
    {
        None     = 0,
        Field    = 4,
        Method   = 8,
        Property = 16,
        All      = 28
    }
}
