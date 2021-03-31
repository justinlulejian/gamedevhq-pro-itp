using UnityEngine;
using UnityEditor;

namespace GameDevHQ.Scripts.UI
{
    public class DebugGameModifierWindow : EditorWindow
    {
        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/DebugGameModifierWindow")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            DebugGameModifierWindow window =
                (DebugGameModifierWindow) GetWindow(
                    typeof(DebugGameModifierWindow), true, "DebugGameModifier", false);
            window.Show();
        }
        
        void OnGUI()
        {
            // Logic depends on a debug game object instance that needs to be instantiated so 
            // don't let the button's be pressed until the game is in play mode.
            if (!Application.isPlaying)
            {
                // TODO: What about just simple text? Couldn't find one.
                EditorGUILayout.TextField("Press play to access debug window.");
                return;
            }
            
            GUILayout.FlexibleSpace();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Skip intro"))
            {
                DebugGameModifiers.Instance.SkipIntroOnPlay(true);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Give unlimited war funds"))
            {
               DebugGameModifiers.Instance.UnlimitedWarFunds();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Drain war funds"))
            {
                DebugGameModifiers.Instance.DrainFunds();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Change game speed +"))
            {
                DebugGameModifiers.Instance.ChangeGameSpeed(.25f);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Change game speed -"))
            {
                DebugGameModifiers.Instance.ChangeGameSpeed(-.25f);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Start never ending wave"))
            {
                DebugGameModifiers.Instance.StartNeverEndingWave();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Stop current wave."))
            {
                DebugGameModifiers.Instance.StopCurrentWave();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Place gatling tower on all open spots."))
            {
                DebugGameModifiers.Instance.PlaceTowersOnAllSpots();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset game with same towers placed."))
            {
                DebugGameModifiers.Instance.ResetGameWithCurrentTowerPlacement();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}