using System;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtensionsWindow: EditorWindow {
    //Go to Tools and you will find this
    //Learn more at https://learn.unity.com/tutorial/editor-scripting
    [MenuItem("Tools/StatsControl")]
    public static void ShowWindow() {
        var window = GetWindow<ExtensionsWindow>();
        window.titleContent = new GUIContent("StatsControl");
        window.Show();
    }

    private void OnGUI() {
    }

    [MenuItem("Tools/Reload/Domain _F1")]
    public static void ReloadDomain() {
        EditorUtility.RequestScriptReload();
    }
    [MenuItem("Tools/Reload/Scene _F2")]
    public static void ReloadScene() {
        var scene = SceneManager.GetActiveScene();
        if (scene != null) {
            var opts = new LoadSceneParameters { };
            EditorSceneManager.LoadSceneInPlayMode(scene.path, opts);
        }
    }
    private void Disableable(Action renderer, bool disabled) {
        EditorGUI.BeginDisabledGroup(false);
        renderer();
        EditorGUI.EndDisabledGroup();
    }
}