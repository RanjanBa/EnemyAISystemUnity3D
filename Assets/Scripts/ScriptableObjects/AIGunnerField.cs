using UnityEngine;

[CreateAssetMenu(menuName = "AIVariableField/AIGunnerField")]
public class AIGunnerField : ScriptableObject
{
    [Range(0, 100f)]
    public float AttackbyGunRange = 30f;
}
