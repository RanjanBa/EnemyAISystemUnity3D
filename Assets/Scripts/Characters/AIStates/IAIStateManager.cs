
public interface IAIStateManager
{
    void OnStateEnter();
    void OnStateExit();
    void UpdateCurrentState();
    void ChangeToUnwareState();
    void ChangeToPatrolState();
    void ChangeToChaseState();
    void ChangeToBoxingState();
    void ChangeToGunFireState();
    void ChangeToCoverState();
    void ChangeToSearchState();
}
