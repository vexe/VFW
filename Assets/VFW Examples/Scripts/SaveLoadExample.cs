using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Vexe.Runtime.Serialization;
using Vexe.Runtime.Types;

/// <summary>
/// Please read the "How do you serialize a BetterBehaviour to file?" part of the documentation first!
/// </summary>
public class SaveLoadExample : BetterBehaviour
{
    static string dirPath = Path.Combine("Assets", "Temp");
    static string filePath = Path.Combine(dirPath, "test.txt");

    public List<string> someList;
    public int someInt;
    public float someFloat;

    [Show] void Save()
    {
        // no need to check if directory exists, CreateDirectory creates the directory only if it doesn't exist
        Directory.CreateDirectory(dirPath);
        using (var writer = new StreamWriter(File.Open(filePath, FileMode.OpenOrCreate)))
        {
            string serialized = Serializer.Serialize(BehaviourData);
            writer.WriteLine(serialized);
        }
    }

    [Show] void Load()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File to load doesn't exist. Did you save first?");
            return;
        }

        using (var reader = new StreamReader(File.OpenRead(filePath)))
        {
            string serialized = reader.ReadLine();
            BehaviourData = Serializer.Deserialize<SerializationData>(serialized);
            Serializer.DeserializeDataIntoTarget(this, BehaviourData);
        }
    }
}