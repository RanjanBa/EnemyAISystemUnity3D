using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunFiringState : IPlayerState
{
    public bool m_IsReloading;

    private PlayerManager m_playerManager;
    private Animator m_playerAnimator;
    private Vector3 m_inputDirection;
    private Transform m_rightShoulderBone;
    private float m_gunFireTimer = 0f;
    private int index = 0;

    public PlayerGunFiringState(PlayerManager playerManager, Animator animator)
    {
        m_playerManager = playerManager;
        m_playerAnimator = animator;
        m_rightShoulderBone = m_playerAnimator.GetBoneTransform(HumanBodyBones.RightShoulder);
    }

    public void OnStateEnter()
    {
        m_playerManager.m_MainCameraTransform.SetParent(m_playerManager.m_CameraLockPivot);
        m_IsReloading = false;
    }

    public void OnStateExit()
    {
        
    }

    public void UpdateState()
    {
        m_gunFireTimer += Time.deltaTime;
        InputHandler();
        UpdateGunFireState();
    }

    private void UpdateGunFireState()
    {
        UpdateAnimation();
    }

    private void InputHandler()
    {
        if(m_IsReloading)
        {
            return;
        }

        if(Input.GetButtonDown(m_playerManager.m_InputKeyManager.m_SwitchWeapon))
        {
            m_playerManager.m_currentlyActiveGun.gameObject.SetActive(false);
            index = (index + 1) % m_playerManager.m_Guns.Count;
            m_playerManager.m_currentlyActiveGun = m_playerManager.m_Guns[index];
            m_playerManager.m_currentlyActiveGun.gameObject.SetActive(true);
        }

        if(Input.GetButtonDown(m_playerManager.m_InputKeyManager.m_MouseAim))
        {
            Debug.Log("Aiming");
        }

        if(Input.GetButtonDown(m_playerManager.m_InputKeyManager.m_MouseFire))
        {
            Fire();
        }

        float value = Input.GetAxis(m_playerManager.m_InputKeyManager.m_MouseFireAndAim);
        Debug.Log("Firing " + value);
        if ((value < -0.5f) && m_gunFireTimer >= m_playerManager.m_currentlyActiveGun.m_GunFireRate)
        {
            Fire();
        }

        if(Input.GetButtonDown(m_playerManager.m_InputKeyManager.m_Reload))
        {
            Reload();
        }
    }

    private void Fire()
    {
        m_gunFireTimer = 0f;
        m_playerManager.m_currentlyActiveGun.Fire(m_playerManager.m_currentlyActiveGun.transform.forward);
    }

    private void Reload()
    {
        bool isOneHanded = m_playerManager.m_currentlyActiveGun.m_TypeOfGun == TypeOfGun.OneHandedGun ? true : false;
        m_playerManager.m_currentlyActiveGun.Reload();
        m_playerAnimator.SetBool("IsOneHanded", isOneHanded);
        m_playerAnimator.SetTrigger("Reload");
    }

    private void UpdateAnimation()
    {
        m_inputDirection = m_playerManager.transform.right * m_playerManager.m_horSpeed + m_playerManager.transform.forward * m_playerManager.m_verSpeed;
        m_inputDirection.y = 0f;
        m_inputDirection.Normalize();

        float speed = m_inputDirection.magnitude;

        Vector3 aimingPosition = m_playerManager.m_MainCameraTransform.position + m_playerManager.m_MainCameraTransform.forward * 50f;
        Debug.DrawRay(aimingPosition, Vector3.up * 10f, Color.blue);

        aimingPosition.y = 0f;

        Vector3 dirToAimingPos = aimingPosition - m_playerManager.transform.position;

        Debug.DrawRay(m_playerManager.transform.position, m_inputDirection * 10f, Color.red);
        Debug.DrawRay(m_playerManager.transform.position, m_playerManager.transform.forward * 10f, Color.yellow);

        float turn = 0f;
        if (speed <= 0f)
        {
            turn = Vector3.SignedAngle(m_playerManager.transform.forward, dirToAimingPos, Vector3.up);
            turn = turn / 15f;
        }
        else
        {
            turn = Vector3.SignedAngle(m_playerManager.transform.forward, m_inputDirection, Vector3.up);
            if(turn > 170f)
            {
                turn *= -1;
            }
            turn = turn / 45f;
        }

        m_playerManager.UpdateMotionAnimation(speed * 4f, turn, 0.2f, 0.1f);
        ApplyExtraRotation(dirToAimingPos);
    }

    private void ApplyExtraRotation(Vector3 direction)
    {
        m_playerManager.transform.rotation = Quaternion.Lerp(m_playerManager.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime);
    }

    public void OnAnimationMove()
    {
        m_playerManager.m_GunAimingHelperBoneTransform.position = m_rightShoulderBone.position;
        m_playerManager.m_GunAimingHelperBoneTransform.rotation = m_rightShoulderBone.rotation;
    }

    public void OnAnimatorIK()
    {
        if (m_IsReloading)
        {
            UpdateHandIK(AvatarIKGoal.RightHand, m_playerManager.m_currentlyActiveGun.m_RightHandPosition, 1f);
            return;
        }
        else
        {
            UpdateHandIK(AvatarIKGoal.LeftHand, m_playerManager.m_currentlyActiveGun.m_LeftHandPosition, 1f);
            UpdateHandIK(AvatarIKGoal.RightHand, m_playerManager.m_currentlyActiveGun.m_RightHandPosition, 1f);
        }

        Vector3 gunHoldingOffset = m_playerManager.m_currentlyActiveGun.m_TypeOfGun == TypeOfGun.OneHandedGun ? m_playerManager.m_HandGunAimingOffset : m_playerManager.m_RifleAimingOffset;
        m_playerManager.m_GunAimingTransform.transform.localPosition = gunHoldingOffset;

        Vector3 aimingPosition = m_playerManager.m_MainCameraTransform.position + m_playerManager.m_MainCameraTransform.forward * 50f;
        m_playerManager.m_GunAimingTransform.transform.LookAt(aimingPosition);

        m_playerAnimator.SetLookAtWeight(1f, 1f, 1f, 1f, 0.5f);
        m_playerAnimator.SetLookAtPosition(aimingPosition + m_playerManager.m_LookAtOffset);
    }

    public void UpdateHandIK(AvatarIKGoal goal, Transform handTransform, float weight)
    {
        m_playerAnimator.SetIKPositionWeight(goal, weight);
        m_playerAnimator.SetIKPosition(goal, handTransform.position);

        m_playerAnimator.SetIKRotationWeight(goal, weight);
        m_playerAnimator.SetIKRotation(goal, handTransform.rotation);
    }
}
