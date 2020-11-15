using UnityEngine;

[System.Serializable]
public class PrefabSet
{
    public string name; //name that will be populated from json
    public float scale = 1.0f; //scale that will be adjusted from UI
    public GameObject prefabObject; //this is where the prefabs will be added
}