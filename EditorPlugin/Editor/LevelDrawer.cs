using UnityEditor;
using UnityEngine;

// IngredientDrawer
[CustomEditor(typeof(LevelContainer))]
public class LevelDrawer : Editor
{
    // Draw the property inside the given rect
    public override void OnInspectorGUI()
    {
        var lvlContainer = target as LevelContainer;
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);

        bool needsUpdate = false;

        float globalScale = EditorGUILayout.Slider("Global scale", lvlContainer.Scale, 0f, 5f);
        if(globalScale != lvlContainer.Scale)
        {
            lvlContainer.Scale = globalScale;
            needsUpdate = true;
        }

        GUILayout.Label("Prefab list", EditorStyles.boldLabel);
        // Populate prefab list
        foreach (var prefabItem in lvlContainer.PrefabObjects)
        {
            EditorGUILayout.BeginFadeGroup(1.0f);

            string title = string.Format("Prefab settings for {0}:", prefabItem.name);
            
            GUILayout.Label(title, EditorStyles.label);
            float newScale = EditorGUILayout.Slider("Prefab scale", prefabItem.scale, 0f, 500f);
            if (prefabItem.scale != newScale)
            {
                prefabItem.scale = newScale;
                needsUpdate = true;
            }

            var prefabObject = EditorGUILayout.ObjectField("Prefab", prefabItem.prefabObject, typeof(GameObject), false) as GameObject;
            if (prefabObject != prefabItem.prefabObject)
            {
                // for some reason objects are not identical?
                prefabItem.prefabObject = prefabObject;
            }

            EditorGUILayout.EndFadeGroup();
        }

        if (lvlContainer.gameObject.transform.childCount > 0)
        {
            if (needsUpdate)
            {
                lvlContainer.UpdateSpawned();
            }

            if (GUILayout.Button("Clear objects"))
            {
                lvlContainer.Clear();
            }
        }
        else
        {
            if (GUILayout.Button("Spawn objects"))
            {
                lvlContainer.Spawn();
            }
        }
    }
}
