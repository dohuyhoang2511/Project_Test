using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelButtonGenerate))]
public class LevelButtonGenerate_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LevelButtonGenerate script = (LevelButtonGenerate)target;

        GUILayout.Space(10);
        GUILayout.Label("Use functions below for generating level buttons");
        GUILayout.Space(10);
        if (GUILayout.Button("Generate Level"))
        {
            script.GenerateLevel();
        }
        if (GUILayout.Button("Clear All Level"))
        {
            script.ClearAllLevel();
        }
        if (GUILayout.Button("Random Stage Unlock"))
        {
            script.RandomStageUnlock();
        }
        if (GUILayout.Button("Reset Stages"))
        {
            script.ResetStages();
        }
        if (GUILayout.Button("Clear Data"))
        {
            script.ClearData();
        }
    }
}

