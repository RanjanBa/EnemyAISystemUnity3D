using UnityEngine;

public abstract class CharacterManager : MonoBehaviour
{
    [Header("Character Manager Variables")]
    #region Customizable Public Variables
    [Range(0, 300)]
    public int m_Health = 100;
    public float m_GroundedDistance = 0.2f;
    [Tooltip("Layer Mask for finding cover")]
    public LayerMask m_EnvironmentLayerForCover;
    #endregion Customizable Public Variables

    #region HideInInspector Variables
    //[HideInInspector]
    public bool m_isCrouching;
    //[HideInInspector]
    public bool m_isInCover;
    //[HideInInspector]
    public float m_charHeight;
    //[HideInInspector]
    public float m_charRadius;
    //[HideInInspector]
    public CharacterType m_CharType;
    #endregion HideInInspector Variables

    #region Protected Variables
    protected Animator m_animator;
    protected SceneCharactersManager m_sceneCharactersManager;
    protected bool m_isGrounded;
    protected CharacterController m_charController;
    #endregion Protected Variables

    protected virtual void Initialized()
    {
        m_sceneCharactersManager = GameObject.FindObjectOfType<SceneCharactersManager>();
        m_animator = GetComponent<Animator>();
        m_charController = GetComponent<CharacterController>();
        m_charHeight = m_charController.height;
        m_charRadius = m_charController.radius;
    }

    public bool CheckForLowOrHighCover()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position + new Vector3(0, m_charHeight / 2f, 0), transform.forward, out hitInfo, 3f, m_EnvironmentLayerForCover))
        {
            Debug.DrawRay(transform.position + new Vector3(0, m_charHeight / 2f, 0), transform.forward, Color.red, 3f);      
            if (Physics.Raycast(transform.position + new Vector3(0, m_charHeight, 0), transform.forward, out hitInfo, 3f, m_EnvironmentLayerForCover))
            {
                Debug.DrawRay(transform.position + new Vector3(0, m_charHeight, 0), transform.forward, Color.red, 3f);
                return true;
            }
            return false;
        }
        return false;
    }

    public virtual void GiveDamage<T>(T character, Vector3 hitPosition) where T: CharacterManager
    {
        Debug.Log("Character Manager Give Damage");
    }

    public virtual void TakeDamage(int damage, DamageType damageType)
    {
        Debug.Log("Base class....");
    }

    protected virtual bool CheckGroudedOrNot()
    {
        if (Physics.Raycast(transform.position, Vector3.down, m_GroundedDistance))
        {
            return true;
        }

        return m_charController.isGrounded;
    }
}