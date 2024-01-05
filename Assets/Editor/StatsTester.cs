using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor: Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        PlayerController myScript = (PlayerController)target;
        if (GUILayout.Button("- 10 Health")) {
            myScript.Heal(-10);
            CameraShakeManger.Instance.ShakeCamera(5f, 0.5f);
        }
        if (GUILayout.Button("+ 10 Health")) {
            myScript.Heal(10);
        }
        if (GUILayout.Button("- 10 Def")) {
            myScript.Def(-10);
            CameraShakeManger.Instance.ShakeCamera(5f, 0.5f);
        }
        if (GUILayout.Button("+ 10 Def")) {
            myScript.Def(10);
        }
    }
}
