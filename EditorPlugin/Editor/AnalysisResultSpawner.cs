using UnityEditor;
using UnityEngine;

public class AnalysisResultSpawner : EditorWindow
{
    #region Data

    private string JSONString = "";

    #endregion

    [MenuItem("Tools/Spawn Objects From JSON")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AnalysisResultSpawner));
    }

    /// <summary>
    /// This method supposadly shows UI
    /// </summary>
    void OnGUI()
    {
        GUILayout.Label("Import Data", EditorStyles.boldLabel);

        GUILayout.Label("Serialized data(JSON string)", EditorStyles.label);
        JSONString = EditorGUILayout.TextArea(JSONString, GUILayout.Height(200));
        
        if (GUILayout.Button("Analyse JSON"))
        {
            // Spawn container object
            GameObject Empty = new GameObject("LevelObjects");
            // Zero out objects position
            Empty.transform.position = Vector3.zero;
            var LevelContainer = Empty.AddComponent<LevelContainer>();
            LevelContainer.PrepareEditor(JSONString);

            Close();
        }

        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
    }
}
