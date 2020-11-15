using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class is just needed for spawning level objects in editor
/// </summary>
public class LevelContainer : MonoBehaviour
{

    [HideInInspector] 
    public string LevelData = "";
    [HideInInspector]
    public float Scale = 1.0f;
    [HideInInspector]
    public PrefabSet[] PrefabObjects;
    [HideInInspector]
    public LevelObject[] LevelObjects;

    public void PrepareEditor(string jsonData)
    {
        LevelData = jsonData;
        Deserialise(jsonData);
    }

    /// <summary>
    /// Spawns objects in LevelData
    /// </summary>
    public void Spawn()
    {
        foreach(var lvlObject in LevelObjects)
        {
            var foundPrefab = PrefabObjects.FirstOrDefault(i=>i.name == lvlObject.shape);
            if (foundPrefab != null && foundPrefab.prefabObject != null)
            {
                var spawned = Instantiate(foundPrefab.prefabObject, this.transform);
                spawned.name = foundPrefab.name;
                var atachedComponent = spawned.AddComponent<ObjectContainer>();
                atachedComponent.PrefabSet = foundPrefab;
                atachedComponent.ObjectData = lvlObject;
                atachedComponent.transform.rotation = new Quaternion(
                    atachedComponent.transform.rotation.x,
                    atachedComponent.transform.rotation.y,
                    lvlObject.rotation,
                    atachedComponent.transform.rotation.w
                );
                atachedComponent.UpdateWithScale(Scale);

                var renderer = spawned.GetComponent<SpriteRenderer>();
                if(renderer != null)
                {
                    renderer.color = new Color(lvlObject.color[0] / 255f, lvlObject.color[1] / 255f, lvlObject.color[2] / 255f);
                }
            }
            else
            {
                Debug.LogError(string.Format("Could not find prefab for:{0}", lvlObject.shape));
            }                
        }
    }

    /// <summary>
    /// Removes all childs of this object
    /// </summary>
    public void Clear()
    {
        var objectsToClear = gameObject.GetComponentsInChildren<Transform>();
        // Check to see if we have childs for this object cuz
        // I don't know how editor handles exceptions
        if (objectsToClear != null)
        {
            foreach (var child in objectsToClear)
            {
                if (this.gameObject != child.gameObject)
                {
                    DestroyImmediate(child.gameObject, true);
                }
            }
        }
    }

    /// <summary>
    /// Updates positions of objects
    /// </summary>
    public void UpdateSpawned()
    {
        var children = gameObject.GetComponentsInChildren<Transform>();
        if(children != null)
        {
            foreach(var child in children)
            {
                if (child.gameObject == gameObject)
                    continue;

                var container = child.gameObject.GetComponent<ObjectContainer>();
                if(container != null)
                {
                    // Pass only global scale as the item should have recieved references to prefab and level object
                    container.UpdateWithScale(Scale);
                }
            }
        }
    }

    /// <summary>
    /// Deserializes data
    /// </summary>
    /// <param name="json">Json data</param>
    private void Deserialise(string json)
    {
        List<PrefabSet> Prefabs = new List<PrefabSet>();
        //Deserialise data
        LevelObjects = JSONHelper.FromJson<LevelObject>(json);
        // Loop through data and check if we have the item with that name in prefabset
        foreach(var item in LevelObjects)
        {
            if(!Prefabs.Any(i => i.name == item.shape))
            {
                Prefabs.Add(new PrefabSet()
                { 
                    name = item.shape,
                    scale = 1.0f,
                });
            }
        }

        PrefabObjects = Prefabs.ToArray();
    }

}