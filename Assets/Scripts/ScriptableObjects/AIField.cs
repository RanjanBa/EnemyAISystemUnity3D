using UnityEngine;

[CreateAssetMenu(menuName = "AIVariableField/AIField")]
public class AIField : ScriptableObject
{
    [Range(15f, 140f)]
    public float ViewAngle = 40f;
    [Range(0f, 100f)]
    public float ViewRange = 20f;
    [Range(0f, 50f)]
    public float ThresoldViewRange = 5f;
    [Tooltip("Time for staying in unware state")]
    public MinMaxValue m_MinMaxUnwareTime = new MinMaxValue(1f, 3f);
    [Tooltip("Time for staying in patrol state")]
    public MinMaxValue m_MinMaxPatrolTime = new MinMaxValue(10f, 20f);
    [Range(-180f, 180f)]
    public float[] SearchOrInvetigateRegions;
    [Range(0f, 30f)]
    public float RangeOfSearchingCoverPos = 10f;
    [Tooltip("Change the cover position to another best coverPosition.")]
    public MinMaxValue ChangeCoverPositionTime = new MinMaxValue(10f, 20f);
}
