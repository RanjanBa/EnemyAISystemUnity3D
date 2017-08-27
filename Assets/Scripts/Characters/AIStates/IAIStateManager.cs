
public interface IAIStateManager
{
    void UpdateCurrentState();
    void ChangeToUnwareState();
    void ChangeToInvestigateState();
    void ChangeToPatrolState();
    void ChangeToChaseState();
    void ChangeToBoxingState();
    void ChangeToGunFireState();
    void ChangeToCoverState();
    void ChangeToSearchState();
}
