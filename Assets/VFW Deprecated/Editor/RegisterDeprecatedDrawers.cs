using UnityEditor;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
    [InitializeOnLoad]
    public static class RegisterDeprecatedDrawers
    {
        static RegisterDeprecatedDrawers()
        {
            MemberDrawersHandler.Mapper.Insert<IBaseDelegate, DelegateDrawer>(true)
                                       .Insert<uDelegate, uDelegateDrawer>()
                                       .Insert<SelectObjAttribute, SelectObjDrawer>();
        }
    }
}