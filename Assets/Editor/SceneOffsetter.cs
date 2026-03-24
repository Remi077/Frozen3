using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SceneOffsetter : EditorWindow
{
    private Vector3 offset = new Vector3(0.6f, 0f, 0f); // Change the offset as needed

    [MenuItem("Tools/SceneOffsetter")]
    public static void ShowWindow()
    {
        GetWindow<SceneOffsetter>("SceneOffsetter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Offset Top-Level Scene Objects", EditorStyles.boldLabel);
        offset = EditorGUILayout.Vector3Field("Offset Amount", offset);

        if (GUILayout.Button("Offset Root Objects"))
        {
            OffsetRoots();
        }
    }

    private void OffsetRoots()
    {
        // Get all root GameObjects in the active scene
        GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();

        // Record undo so changes can be reverted
        Undo.RecordObjects(roots, "Offset Root Objects");

        foreach (GameObject obj in roots)
        {
            obj.transform.position += offset;
        }

        Debug.Log("Root objects offset by " + offset);
    }
}