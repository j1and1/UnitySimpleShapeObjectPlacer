using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectContainer : MonoBehaviour
{
    [HideInInspector]
    public PrefabSet PrefabSet;
    [HideInInspector]
    public LevelObject ObjectData;

    public void UpdateWithScale(float globalScale = 1.0f)
    {
        if (ObjectData == null || PrefabSet == null)
        {
            throw new System.Exception("No lvldata availible");
        }

        this.transform.position = new Vector3(ObjectData.x * globalScale, ObjectData.y * globalScale * -1.0f, this.transform.position.z);
        this.transform.localScale = new Vector3(globalScale * PrefabSet.scale * ObjectData.scale, globalScale * PrefabSet.scale * ObjectData.scale, globalScale * PrefabSet.scale * ObjectData.scale);
    }
}