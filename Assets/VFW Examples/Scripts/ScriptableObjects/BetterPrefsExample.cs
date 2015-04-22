using UnityEngine;
using System.Collections;
using System.IO;
using Vexe.Runtime.Types;

namespace VFWExamples
{
    public class BetterPrefsExample : BetterBehaviour
    {
        public BetterPrefs prefs;
        int posKey = "MyPosition".GetHashCode();
        int rotKey = "MyRotation".GetHashCode();
        int sclKey = "MyScale".GetHashCode();

        [Show] void SaveVectorsToPrefs()
        {
            // save position, euler angles and local scale to prefs
            prefs.Vector3s[posKey] = transform.position;
            prefs.Vector3s[rotKey] = transform.localEulerAngles;
            prefs.Vector3s[sclKey]    = transform.localScale;
        }

        [Show] void ReadVectorsFromPrefsAndWriteToFile()
        {
            // read values...
            var pos = prefs.Vector3s[posKey];
            var rot = prefs.Vector3s[rotKey];
            var scl = prefs.Vector3s[sclKey];

            // log them
            LogFormat("Position: {0} - Rotation {1} - Scale {2}", pos, rot, scl);

            // maybe serialize and write them to a file?
            string serializedPos = Serializer.Serialize(pos);
            string serializedRot = Serializer.Serialize(rot);
            string serializedScl = Serializer.Serialize(scl);
            using (var file = File.Open("Assets/Vexe/Runtime/Examples/Assets/Sample.data", FileMode.OpenOrCreate))
            {
                using (var writer = new StreamWriter(file))
                {
                    writer.WriteLine(serializedPos);
                    writer.WriteLine(serializedRot);
                    writer.WriteLine(serializedScl);
                }
            }
        }
    }
}
