using UnityEngine;

[CreateAssetMenu(menuName = "AIVariableField/AIGunnerField")]
public class AIGunnerField : ScriptableObject
{
    [Range(0, 100f)]
    public float AttackbyGunRange = 30f;
    [Range(0.1f, 20f)]
    public float ChangingFireToCoverStateTime = 5f;
}
