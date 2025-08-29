using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class SceneSetup
{
    static SceneSetup()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
    }

    private static void OnSceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
    {
        // Check if the opened scene is the one we want to modify
        if (scene.name.Contains("08-21"))
        {
            // Check if the placer object already exists to avoid duplication
            if (GameObject.Find("KinesVRUIManagerPlacer"))
            {
                return;
            }

            // Create the placer object and add the component
            GameObject placerGO = new GameObject("KinesVRUIManagerPlacer");
            KinesVRUIManagerPlacer placer = placerGO.AddComponent<KinesVRUIManagerPlacer>();

            // Assign prefabs using AssetDatabase
            placer.canvasPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/StarterSamples/Core/DebugUI/Prefabs/CanvasWithDebug.prefab");
            placer.buttonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/StarterSamples/Core/DebugUI/Prefabs/DebugButton.prefab");
            placer.labelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/StarterSamples/Core/DebugUI/Prefabs/DebugLabel.prefab");

            if (placer.canvasPrefab == null || placer.buttonPrefab == null || placer.labelPrefab == null)
            {
                Debug.LogError("Failed to load one or more prefabs in SceneSetup.cs. Check the paths.");
            }
            else
            {
                Debug.Log("KinesVRUIManagerPlacer created and prefabs assigned by SceneSetup.cs.");

                // Mark the scene as dirty and save it to make the changes permanent
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log("Scene saved with KinesVRUIManagerPlacer.");
            }
        }
    }
}
