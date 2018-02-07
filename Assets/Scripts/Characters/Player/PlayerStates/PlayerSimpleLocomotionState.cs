using System;
using UnityEngine;

public class PlayerSimpleLocomotionState : IPlayerState
{
    private PlayerManager m_playerManager;
    private Animator m_playerAnimator;
    private Vector3 m_inputDirection;
    private Vector3 m_playerForwardDir;
    private Transform m_spineBone;
    private Transform m_rightUpperLegBone;
    private bool m_isInMotion;

    public PlayerSimpleLocomotionState(PlayerManager playerManager, Animator animator)
    {
        m_playerManager = playerManager;
        m_playerAnimator = animator;
        m_isInMotion = false;
        m_spineBone = m_playerAnimator.GetBoneTransform(HumanBodyBones.Spine);
        m_rightUpperLegBone = m_playerAnimator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
    }

    public void OnStateEnter()
    {
        m_playerManager.m_MainCameraTransform.SetParent(m_playerManager.m_CameraFreePivot);
        foreach (var gun in m_playerManager.m_Guns)
        {
            if(gun.m_TypeOfGun == TypeOfGun.OneHandedGun)
            {
                gun.transform.SetParent(m_playerManager.m_HandGunHoldingTransform);
            }
            else
            {
                gun.transform.SetParent(m_playerManager.m_RifleHoldingTransform);
            }
        }
    }

    public void UpdateState()
    {
        UpdateSimpleLocomotionState();
    }

    private void UpdateSimpleLocomotionState()
    {
        m_inputDirection = m_playerManager.m_MainCameraTransform.transform.right * m_playerManager.m_horSpeed + m_playerManager.m_MainCameraTransform.transform.forward * m_playerManager.m_verSpeed;
        m_inputDirection.y = 0f;
        m_inputDirection.Normalize();
        m_playerForwardDir = m_playerManager.transform.forward;
        m_playerForwardDir.y = 0f;

        Debug.DrawRay(m_playerManager.transform.position, m_inputDirection * 10f, Color.red);
        Debug.DrawRay(m_playerManager.transform.position, m_playerForwardDir * 10f, Color.yellow);

        float turn = 0;
        float speed = m_inputDirection.magnitude;
        if (speed > 0f)
        {
            turn = Vector3.SignedAngle(m_playerForwardDir, m_inputDirection, Vector3.up);
        }

        if (speed > 0f && Mathf.Abs(turn) < 50f)
        {
            m_isInMotion = true;
        }

        if (!m_isInMotion)
        {
            speed = 0f;
        }

        m_playerManager.UpdateMotionAnimation(speed, turn, 0f, 0f);

        if(m_isInMotion && speed <= 0f)
        {
            m_isInMotion = false;
        }
    }

    public void OnAnimationMove()
    {
        m_playerManager.m_HandGunHoldingTransform.position = m_rightUpperLegBone.position;
        m_playerManager.m_HandGunHoldingTransform.rotation = m_rightUpperLegBone.rotation;
        m_playerManager.m_RifleHoldingTransform.position = m_spineBone.position;
        m_playerManager.m_RifleHoldingTransform.rotation = m_spineBone.rotation;
    }

    public void OnAnimatorIK()
    {
        Debug.Log("Guns " + m_playerManager.m_Guns.Count);
        foreach (var gun in m_playerManager.m_Guns)
        {
            if(gun.m_TypeOfGun == TypeOfGun.OneHandedGun)
            {
                gun.transform.localPosition = m_playerManager.m_HandGunHoldingOffset;
            }
            else
            {
                gun.transform.localPosition = m_playerManager.m_RifleHoldingOffset;
            }
        }
    }

    public void OnStateExit()
    {
        throw new NotImplementedException();
    }
}
