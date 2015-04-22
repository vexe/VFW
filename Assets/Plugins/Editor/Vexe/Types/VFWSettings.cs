using UnityEditor;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Types
{
    public class VFWSettings : BetterScriptableObject
    {
        [EnumMask, Comment("These are the default settings that will be applied to newly instantiated BetterBehaviours and BetterScriptableObjects")]
        public CategoryDisplay DefaultDisplay = CategoryDisplay.BoxedMembersArea;
        public int DefaultSpacing = 10;

        private const string SettingsPath = "Assets/Plugins/Editor/Vexe/ScriptableAssets/VFWSettings.asset";

        private static VFWSettings _instance;
        public static VFWSettings GetInstance()
        {
            if (_instance == null)
            {
                _instance = AssetDatabase.LoadAssetAtPath(SettingsPath, typeof(VFWSettings)) as VFWSettings;
                if (_instance == null)
                {
                    _instance = CreateInstance<VFWSettings>();
                    AssetDatabase.CreateAsset(_instance, SettingsPath);
                    AssetDatabase.ImportAsset(SettingsPath, ImportAssetOptions.ForceUpdate);
                }
            }
            return _instance;
        }

        public static class MenuItems
        {
            [MenuItem("Tools/Vexe/VFWSettings")]
            public static void SelectSettings()
            {
                Selection.activeObject = VFWSettings.GetInstance();
            }
        }
    }
}