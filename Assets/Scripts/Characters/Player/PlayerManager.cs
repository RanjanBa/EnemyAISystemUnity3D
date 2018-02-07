using GunClass_namespace;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputKeyManager))]
public sealed class PlayerManager : CharacterManager
{
    [Header("Player Manager Variables")]
    public Transform m_MainCameraTransform;
    public Transform m_CameraFreePivot;
    public Transform m_CameraLockPivot;
    public CameraMovementType m_typeOfCameraMovement;
    public Vector3 m_LookAtOffset;

    [Header("For Gun Aiming and Holding")]
    public Transform m_GunAimingHelperBoneTransform;
    public Transform m_GunAimingTransform;
    public Vector3 m_HandGunAimingOffset;
    public Vector3 m_RifleAimingOffset;
    public Transform m_HandGunHoldingTransform;
    public Transform m_RifleHoldingTransform;
    public Vector3 m_HandGunHoldingOffset;
    public Vector3 m_RifleHoldingOffset;
    public List<GunClass> m_Guns;

    //[HideInInspector]
    public bool m_CanUpdateAnimIK;
    //[HideInInspector]
    public bool m_IsAiming;
    [HideInInspector]
    public InputKeyManager m_InputKeyManager;
    [HideInInspector]
    public float m_horSpeed;
    [HideInInspector]
    public float m_verSpeed;
    [HideInInspector]
    public GunClass m_currentlyActiveGun;

    private PlayerSimpleLocomotionState m_simpleLocomotionState;
    private PlayerGunFiringState m_gunFireState;
    private PlayerBoxingState m_boxingState;

    private IPlayerState m_currentState;

    private void Start()
    {
        base.Initialized();
        m_sceneCharactersManager.AddPlayerToCharaterList(this.gameObject, CharacterType.Player);
        m_InputKeyManager = GetComponent<InputKeyManager>();

        GunClass[] guns = GetComponentsInChildren<GunClass>();
        m_Guns = new List<GunClass>();
        for (int i = 0; i < guns.Length; i++)
        {
            m_Guns.Add(guns[i]);
        }
        m_currentlyActiveGun = m_Guns[0];

        if (m_MainCameraTransform == null)
        {
            m_MainCameraTransform = Camera.main.transform;
        }

        m_simpleLocomotionState = new PlayerSimpleLocomotionState(this, m_animator);
        m_gunFireState = new PlayerGunFiringState(this, m_animator);

        if (m_typeOfCameraMovement == CameraMovementType.FreeLook)
        {
            OnStateChanged(m_simpleLocomotionState);
        }
        else
        {
            OnStateChanged(m_gunFireState);
        }
    }

    private void Update()
    {
        m_isGrounded = CheckGroudedOrNot();
        m_horSpeed = Input.GetAxis(m_InputKeyManager.m_HorizontalKeys);
        m_verSpeed = Input.GetAxis(m_InputKeyManager.m_VerticalKeys);

        m_currentState.UpdateState();
    }

    private void OnAnimatorMove()
    {
        m_animator.ApplyBuiltinRootMotion();
        if (m_CanUpdateAnimIK)
        {
            m_currentState.OnAnimationMove();
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (m_CanUpdateAnimIK)
        {
            m_currentState.OnAnimatorIK();
        }
    }

    public void OnStateChanged(IPlayerState state)
    {
        if (m_currentState != null)
        {
            m_currentState.OnStateExit();
        }

        m_currentState = state;
        m_currentState.OnStateEnter();
    }

    public void UpdateMotionAnimation(float speed, float turn, float smoothSpeed, float smoothRotation)
    {
        m_animator.SetFloat("Speed", speed, smoothSpeed, Time.deltaTime);
        m_animator.SetFloat("Turn", turn, smoothRotation, Time.deltaTime);
    }

    public override void GiveDamage<T>(T opponentManager, Vector3 hitPosition)
    {
        Debug.Log("PlayerManager Give Damage");
    }

    public override void TakeDamage(int damage, DamageType damageType)
    {
        Debug.Log("Player is damaged by " + damageType.ToString());
        m_Health -= damage;
    }

    public void Reloading(string reload)
    {
        if(m_gunFireState == null)
        {
            return;
        }

        if (reload == "start")
        {
            m_gunFireState.m_IsReloading = true;
        }
        else if (reload == "exit")
        {
            m_gunFireState.m_IsReloading = false;
            m_currentlyActiveGun.Reload();
        }
    }
}
