using UnityEngine;

[CreateAssetMenu(menuName = "AIVariableField/AICombatField")]
public class AICombatField : ScriptableObject
{
    [Range(0, 10f)]
    public float AttackRange = 5f;
    [Range(0, 10f)]
    public float KickRange = 4f;
    [Range(0, 10f)]
    public float PunchRange = 2f;
    [Range(0.1f, 10f)]
    public float AttackRate = 1f;
}
