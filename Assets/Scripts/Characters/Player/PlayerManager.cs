using UnityEngine;

[RequireComponent(typeof(InputKeyManager))]
public class PlayerManager : CharacterManager
{
    [SerializeField]
    private float m_horSpeed;
    [SerializeField]
    private float m_verSpeed;

    private InputKeyManager m_inputKeyManager;

    private void Start()
    {
        base.Initialized();
        m_sceneCharactersManager.AddPlayerToCharaterList(this.gameObject, CharacterType.Player);
        m_inputKeyManager = GetComponent<InputKeyManager>();
    }

    private void Update()
    {
        m_horSpeed = Input.GetAxis(m_inputKeyManager.m_HorizontalKeys);
        m_verSpeed = Input.GetAxis(m_inputKeyManager.m_VerticalKeys);
    }

    private void FixedUpdate()
    {
        
    }

    public override void GiveDamage<T>(T opponentManager, Vector3 hitPosition)
    {
        Debug.Log("PlayerManager Give Damage");
    }

    public override void TakeDamage(int damage, DamageType damageType)
    {
        Debug.Log("Player is damaged by opponent...");
        m_Health -= damage;
    }

    private void LateUpdate()
    {
        
    }
}
