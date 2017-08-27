using UnityEngine;

public class PunchBehaviour : StateMachineBehaviour
{
    public SelectCharacterForm characterForm;
    public HitColliderType ColliderType;
    public int GiveHitID = 0;
    public int GiveBlockID = 0;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (characterForm == SelectCharacterForm.Player)
        {
            PlayerManager playerManager = animator.GetComponent<PlayerManager>();
            Debug.Log("Not implement " + playerManager);
        }
        else if (characterForm == SelectCharacterForm.AIManager)
        {
            AIManager_namespace.AIBoxerManager aiBoxer = animator.GetComponent<AIManager_namespace.AIBoxerManager>();
            aiBoxer.EnableGiveHitCollider(ColliderType);
            aiBoxer.m_GiveHitID = GiveHitID;
            aiBoxer.m_GiveBlockID = GiveBlockID;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (characterForm == SelectCharacterForm.Player)
        {
            PlayerManager playerManager = animator.GetComponent<PlayerManager>();
            Debug.Log("Not implement " + playerManager);
        }
        else if (characterForm == SelectCharacterForm.AIManager)
        {
            AIManager_namespace.AIBoxerManager aiBoxer = animator.GetComponent<AIManager_namespace.AIBoxerManager>();
            aiBoxer.DisableGiveHitCollider(ColliderType);
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
