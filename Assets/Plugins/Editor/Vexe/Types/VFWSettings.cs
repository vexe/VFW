using UnityEditor;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Types
{
    public class VFWSettings : BetterScriptableObject
    {
        const string kDefaultMemberFormat = "$nicename";
        const string kDefaultSequenceFormat = "$nicename ($nicetype)";
        const string kDefaultDictionaryFormat = "$nicename ($nicetype)";
        [EnumMask, Comment("These are the default settings that will be applied to newly instantiated BetterBehaviours and BetterScriptableObjects")]
        public CategoryDisplay DefaultDisplay = CategoryDisplay.BoxedMembersArea;
        public int DefaultSpacing = 10;

        [Serialize, Hide] string _memberFormat = kDefaultMemberFormat;
        [Serialize, Hide] string _sequenceFormat = kDefaultSequenceFormat;
        [Serialize, Hide] string _dictionaryFormat = kDefaultDictionaryFormat;

        [Comment("The following are formatting options for sequences (array/list), dictionaries and general members. Available patterns are $nicename, $name, $type and $nicetype")]
        [Show] public string MemberFormat
        {
            get { return _memberFormat; }
            set
            {
                if (!string.IsNullOrEmpty(value) && value != _memberFormat)
                    _memberFormat = value;
            }
        }

        [Show] public string SequenceFormat
        {
            get { return _sequenceFormat; }
            set
            {
                if (!string.IsNullOrEmpty(value) && value != _sequenceFormat)
                    _sequenceFormat = value;
            }
        }

        [Show] public string DictionaryFormat
        {
            get { return _dictionaryFormat; }
            set
            {
                if (!string.IsNullOrEmpty(value) && value != _dictionaryFormat)
                    _dictionaryFormat = value;
            }
        }

        [Comment("Should postfix the display text of a member with `(Readonly)` if it's annotated with [ReadonlyAttribute]?")]
        public bool DisplayReadonlyIfTrue = true;

        private const string SettingsPath = "Assets/Plugins/Editor/Vexe/ScriptableAssets/VFWSettings.asset";

        [Show]
        public void Reset()
        {
            _memberFormat = kDefaultMemberFormat;
            _sequenceFormat = kDefaultSequenceFormat;
            _dictionaryFormat = kDefaultDictionaryFormat;

            DefaultDisplay = CategoryDisplay.BoxedMembersArea;
            DefaultSpacing = 10;
            DisplayReadonlyIfTrue = true;
        }

        static VFWSettings instance;
        public static VFWSettings GetInstance()
        {
            if (instance == null)
            {
                instance = AssetDatabase.LoadAssetAtPath(SettingsPath, typeof(VFWSettings)) as VFWSettings;
                if (instance == null)
                {
                    instance = CreateInstance<VFWSettings>();
                    AssetDatabase.CreateAsset(instance, SettingsPath);
                    AssetDatabase.ImportAsset(SettingsPath, ImportAssetOptions.ForceUpdate);
                }
            }
            return instance;
        }

        public static class MenuItems
        {
            [MenuItem("Tools/Vexe/VFW Settings")]
            public static void SelectSettings()
            {
                Selection.activeObject = VFWSettings.GetInstance();
            }
        }
    }
}