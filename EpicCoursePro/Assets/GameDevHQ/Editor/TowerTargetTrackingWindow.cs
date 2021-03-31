using System;

namespace GameDevHQ.Scripts.UI
{
    using UnityEngine;
    using UnityEditor;

    // TODO: Unfinished window that would track all tower targets and those in attack radius while
    // game is playing. Clicking on targets would highlight them in the scene.
    public class TowerTargetTrackingWindow : EditorWindow
    {
        string myString = "Hello Towers";
        bool groupEnabled = true;
        bool myBool = false;
        float myFloat = 1.23f;

        private void OnEnable()
        {
            titleContent = new GUIContent(typeof(TowerTargetTrackingWindow).ToString());
        }

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/TowerTargetTrackingWindow")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            TowerTargetTrackingWindow window =
                (TowerTargetTrackingWindow) EditorWindow.GetWindow(
                    typeof(TowerTargetTrackingWindow));
            window.Show();
        }

        void OnGUI()
        {
            // Selection.activeGameObject = GameObject.Find("Mech1");
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            myString = EditorGUILayout.TextField("Text Field", myString);

            groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            myBool = EditorGUILayout.Toggle("Toggle", myBool);
            myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
            EditorGUILayout.EndToggleGroup();

            if (myBool)
            {
                GameObject[] gos = GameObject.FindGameObjectsWithTag("Enemy");
                Selection.activeGameObject = gos[Random.Range(0, gos.Length - 1)];
            }
        }
    }
}