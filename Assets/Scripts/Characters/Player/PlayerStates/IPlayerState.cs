public interface IPlayerState
{
    void OnStateEnter();
    void UpdateState();
    void OnAnimationMove();
    void OnAnimatorIK();
    void OnStateExit();
}
