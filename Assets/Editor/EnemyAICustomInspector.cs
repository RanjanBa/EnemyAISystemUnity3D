using UnityEditor;
using UnityEngine;
using AIManager_namespace;

[CustomEditor(typeof(AIManager), true)]
//[CanEditMultipleObjects]
public class EnemyAICustomInspector : Editor
{
    AIManager enemy_char;

    private void OnEnable()
    {
        enemy_char = (AIManager)target;
    }

    public override void OnInspectorGUI()
    {
        //foreach (var item in enemy_char.AIStates)
        //{
        //    Debug.Log(item.ToString());
        //}

        enemy_char.AIIndex = EditorGUILayout.Popup("AIStates", enemy_char.AIIndex, enemy_char.AIStates);
        DrawDefaultInspector();
    }

    private void OnSceneGUI()
    {
        Handles.Label(enemy_char.transform.position + Vector3.up * 1.6f, enemy_char.name);
    }
}
