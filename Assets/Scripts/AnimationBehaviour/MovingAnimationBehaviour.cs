using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAnimationBehaviour : StateMachineBehaviour
{
    public SelectCharacterForm m_CharacterForm;
    [Tooltip("Stop the character moving during unmovable animation")]
    public bool m_StopMovementAtBegining;
    [Tooltip("For moving the character")]
    public bool m_StartMovementAtEnd;

    private AnimationState m_prevState;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_StopMovementAtBegining)
        {
            if (m_CharacterForm == SelectCharacterForm.Player)
            {
                PlayerManager playerManager = animator.GetComponent<PlayerManager>();
                if (playerManager != null)
                {
                    Debug.LogWarning("Implement the condition...");
                }
                else
                {
                    Debug.LogError("Error behaviour, playerManger is null");
                }
            }
            else if(m_CharacterForm == SelectCharacterForm.AIManager)
            {
                AIManager_namespace.AIManager aiManager = animator.GetComponentInParent<AIManager_namespace.AIManager>();
                if (aiManager != null)
                {
                    Debug.Log("Call begin behaviour");
                    m_prevState = aiManager.m_currentAnimationState;
                    aiManager.m_currentAnimationState = AnimationState.IdleAnimation;
                }
                else
                {
                    Debug.LogError("Error behaviour aiManager is null");
                }
            }
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_StartMovementAtEnd)
        {
            if (m_CharacterForm == SelectCharacterForm.Player)
            {
                PlayerManager playerManager = animator.GetComponent<PlayerManager>();
                if (playerManager != null)
                {
                    Debug.LogWarning("Implement the condition...");
                }
                else
                {
                    Debug.LogError("Error behaviour, playerManger is null");
                }
            }
            else if (m_CharacterForm == SelectCharacterForm.AIManager)
            {
                AIManager_namespace.AIManager aiManager = animator.GetComponentInParent<AIManager_namespace.AIManager>();
                if (aiManager != null)
                {
                    Debug.Log("Call end behaviour");
                    aiManager.m_currentAnimationState = m_prevState;
                }
                else
                {
                    Debug.LogError("Error behaviour aiManager is null");
                }
            }
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    //OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK(inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}
}
