using Vexe.Runtime.Types;

namespace Vexe.Editor.Types
{
    // Configures VFW display options etc.
    public static class VFWSettings
    {
        // Omit the type prefix in the member display. 
        // Eg m_fValue/_fValue/fValue will be displayed as Value
        public static bool UseHungarianNotation = false;

        // These are the default settings that will be applied to newly instantiated BaseBehaviours and BaseScriptableObjects
        public static CategoryDisplay DefaultDisplay = CategoryDisplay.BoxedMembersArea;
        public static int DefaultSpacing = 10;

        // The following are formatting options for sequences (array/list), dictionaries and general members. 
        // Available patterns are $nicename, $name, $type and $nicetype. public static string CurrentMemberFormat = kDefaultMemberFormat;
        public static string DefaultMemberFormat = "$nicename";
        public static string DefaultSequenceFormat = "$nicename ($count)";
        public static string DefaultDictionaryFormat = "$nicename ($count)";
    }
}
