using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DataBase : MonoBehaviour
{
    public List<ItemData> item = new List<ItemData>();
}

[System.Serializable]

public class ItemData
{
    public int stack;
    public int id;
    public string name;
    public Sprite image;
    
}