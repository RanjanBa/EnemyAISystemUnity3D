using UnityEditor;
using UnityEngine;
using AIManager_namespace;

[CustomEditor(typeof(EnemyBoxer))]
[CanEditMultipleObjects]
public class EnemyBoxerCustomIns : Editor
{
    EnemyBoxer enemy_char;

    private void OnEnable()
    {
        enemy_char = (EnemyBoxer)target;
    }

    public override void OnInspectorGUI()
    {
        enemy_char.AIIndex = EditorGUILayout.Popup("AIStates", enemy_char.AIIndex, enemy_char.AIStates);
        DrawDefaultInspector();
    }

    private void OnSceneGUI()
    {
        Handles.Label(enemy_char.transform.position + Vector3.up * 1.6f, enemy_char.name);
    }
}
