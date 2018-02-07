using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHandIKPositionBehaviour : StateMachineBehaviour
{
    public UseIK[] useIK;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GunClass_namespace.GunClass gunClass = animator.GetComponent<AIManager_namespace.AIGunnerManager>().m_CurrentlyActiveGun;
        if (gunClass.m_TypeOfGun == TypeOfGun.OneHandedGun)
        {
            HandGun hand_gun = gunClass.gameObject.GetComponent<HandGun>();
            foreach (UseIK item in useIK)
            {
                if (item == UseIK.LeftHand)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, hand_gun.m_LeftHandPosition.position);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, hand_gun.m_LeftHandPosition.rotation);
                }
            }
        }
        else
        {
            AssaulfRifle assault_rifle = gunClass.gameObject.GetComponent<AssaulfRifle>();
            foreach (UseIK item in useIK)
            {
                if (item == UseIK.LeftHand)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, assault_rifle.m_LeftHandPosition.position);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, assault_rifle.m_LeftHandPosition.rotation);
                }

                if (item == UseIK.RightHand)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, assault_rifle.m_RightHandPosition.position);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, assault_rifle.m_RightHandPosition.rotation);
                }
            }
        }
    }
}
