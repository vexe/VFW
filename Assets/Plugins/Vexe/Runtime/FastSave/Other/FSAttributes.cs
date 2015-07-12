using System;
using UnityEngine;

namespace Vexe.FastSave
{
    /// <summary>
    /// Annotate a member (field/auto-property) to mark it for saving
    /// </summary>
    public class SaveAttribute : Attribute { }

    /// <summary>
    /// Annotate a member (field/auto-property) to ignore saving it
    /// </summary>
    public class DontSaveAttribute : Attribute { }
}