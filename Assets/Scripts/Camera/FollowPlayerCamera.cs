using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerCamera : MonoBehaviour
{
    public float m_MouseSensitivity = 5f;
    public Vector2 m_PitchMinMax = new Vector2(-40, 85);
    public float m_RotationSmoothTime = 1f;
    public float m_CameraFollowSpeed = 5f;
    public float m_FreeLookRadius = 4f;
    public float m_LockLookRadius = 1f;
    public float m_LockLookAimRadius = 1f;
    public float m_LockLookRightDistance = 1f;

    private Vector3 m_rotationSmoothVelocity;
    private Vector3 m_currentVelocity;
    private float m_mouseX, m_pitch;
    private PlayerManager m_playerManager;
    private InputKeyManager m_inputKeyManager;

    private void Start()
    {
        m_playerManager = GetComponent<PlayerManager>();
        m_inputKeyManager = GetComponent<InputKeyManager>();
    }

    private void Update()
    {
        m_mouseX += Input.GetAxis(m_inputKeyManager.m_MouseX) * m_MouseSensitivity;
        m_pitch -= Input.GetAxis(m_inputKeyManager.m_MouseY) * m_MouseSensitivity;
        m_pitch = Mathf.Clamp(m_pitch, m_PitchMinMax.x, m_PitchMinMax.y);

        if (m_playerManager.m_typeOfCameraMovement == CameraMovementType.FreeLook)
        {
            UpdateFreeLookCamera();
        }
        else
        {
            UpdateLockLookCamera();
        }

        m_playerManager.m_MainCameraTransform.transform.localEulerAngles = Vector3.zero;
    }

    private void UpdateFreeLookCamera()
    {
        m_currentVelocity = Vector3.SmoothDamp(m_currentVelocity, new Vector3(m_pitch, m_mouseX), ref m_rotationSmoothVelocity, m_RotationSmoothTime);
        m_playerManager.m_CameraFreePivot.eulerAngles = m_currentVelocity;
        m_playerManager.m_MainCameraTransform.transform.position = m_playerManager.m_CameraFreePivot.position - m_playerManager.m_CameraFreePivot.forward * m_FreeLookRadius;
    }

    private void UpdateLockLookCamera()
    {
        m_currentVelocity = new Vector3(m_pitch, m_mouseX);
        m_playerManager.m_CameraLockPivot.eulerAngles = m_currentVelocity;
        float lockRadius = m_playerManager.m_IsAiming ? m_LockLookAimRadius : m_LockLookRadius;
        m_playerManager.m_MainCameraTransform.transform.position = m_playerManager.m_CameraLockPivot.position - m_playerManager.m_CameraLockPivot.forward * lockRadius + m_playerManager.m_CameraLockPivot.right * m_LockLookRightDistance;
    }
}
