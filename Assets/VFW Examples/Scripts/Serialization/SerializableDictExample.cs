using System;
using UnityEngine;
using Vexe.Runtime.Types;

// Use SerializableDictionary in BaseBehaviour - when you don't want custom serialization but
// you still want to be able to serialize dictionaries
public class SerializableDictExample : BaseBehaviour
{
    public Lookup lookup = new Lookup();

    [Button("AddItem")]
    public string addItem;

    [Display(Dict.HideElementLabel | Dict.HideKeyNameField | Dict.AddToLast)]
    public Items items = new Items();

    [Serializable]
    public class Lookup : SerializableDictionary<string, GameObject> { }

    [Serializable]
    public class Items : SerializableDictionary<string, Item> 
    {
        
    }

    [Serializable]
    public class Item
    {
        public string name;
        public int durabilty;
        public float price;
        public bool isRare;

        public Item(string _name)
        {
            name = _name;
            durabilty = UnityEngine.Random.Range(0,9);
            price = UnityEngine.Random.Range(0,9f);
            isRare = false;

        }

        public override string ToString()
        {
            return name;
        }
    }

    public void AddItem()
    {
        Item _newItem = new Item(addItem);
        items.Add(_newItem.name, _newItem);
    }
}
