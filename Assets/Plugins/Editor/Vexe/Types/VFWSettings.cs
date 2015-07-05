using System;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;
using Vexe.Runtime.Serialization;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Types
{
    public class VFWSettings : BaseScriptableObject
    {
        const string kDefaultMemberFormat = "$nicename";
        const string kDefaultSequenceFormat = "$nicename ($count)";
        const string kDefaultDictionaryFormat = "$nicename ($count)";

        [Comment("Omit the type prefix in the member display. Eg m_fValue/_fValue/fValue will be displayed as Value")]
        public bool UseHungarianNotation;

        [EnumMask, Comment("These are the default settings that will be applied to newly instantiated BetterBehaviours and BetterScriptableObjects")]
        public CategoryDisplay DefaultDisplay = CategoryDisplay.BoxedMembersArea;
        public int DefaultSpacing = 10;

        [SerializeField, Hide] string _memberFormat = kDefaultMemberFormat;
        [SerializeField, Hide] string _sequenceFormat = kDefaultSequenceFormat;
        [SerializeField, Hide] string _dictionaryFormat = kDefaultDictionaryFormat;

        [Comment("The following are formatting options for sequences (array/list), dictionaries and general members. Available patterns are $nicename, $name, $type and $nicetype. Note that they might not apply immediately on existing scripts till the next assembly reload")]
        [Show] public string MemberFormat
        {
            get { return GetFormat(ref _memberFormat, kDefaultMemberFormat); }
            set { SetFormat(ref _memberFormat, value); }
        }

        [Show] public string SequenceFormat
        {
            get { return GetFormat(ref _sequenceFormat, kDefaultSequenceFormat); }
            set { SetFormat(ref _sequenceFormat, value); }
        }

        [Show] public string DictionaryFormat
        {
            get { return GetFormat(ref _dictionaryFormat, kDefaultDictionaryFormat); }
            set { SetFormat(ref _dictionaryFormat, value); }
        }

        void SetFormat(ref string format, string value)
        {
            if (!string.IsNullOrEmpty(value) && value != format)
                format = value;
        }

        string GetFormat(ref string format, string defaultFormat)
        {
            if (string.IsNullOrEmpty(format))
                format = defaultFormat;
            return format;
        }

        [Comment("Should postfix the display text of a member with `(Readonly)` if it's annotated with [ReadonlyAttribute]?")]
        public bool DisplayReadonlyIfTrue = true;

        [Show, Comment("Resets all settings to their defaults")]
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
            if (instance == null || !AssetDatabase.Contains(instance))
            {
                var dirs = Directory.GetDirectories("Assets", "Vexe", SearchOption.AllDirectories);
                var editorDir = dirs.FirstOrDefault(x => Directory.GetParent(x).Name == "Editor");
                var prefsDir = Path.Combine(editorDir, "ScriptableAssets");
                if (editorDir == null || !Directory.Exists(prefsDir))
                {
                    Debug.LogError("Unable to create Vfw settings asset at Editor/Vexe/ScriptableAssets (couldn't find folder). Please make sure that path exists 'somewhere' in your project");
                    return instance != null ? instance : instance = CreateInstance<VFWSettings>();
                }

                var path = Path.Combine(prefsDir, "VFWSettings.asset");
                instance = AssetDatabase.LoadAssetAtPath<VFWSettings>(path);
                if (instance == null)
                {
                    instance = ScriptableObject.CreateInstance<VFWSettings>();
                    AssetDatabase.CreateAsset(instance, path);
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    AssetDatabase.Refresh();
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
