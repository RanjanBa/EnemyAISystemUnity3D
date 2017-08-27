using UnityEngine;
using AIManager_namespace;

public class PlayerGunnerCampanion : AIGunnerManager
{
    private void Start()
    {
        Initialized();
        m_CharType = CharacterType.PlayerGunnerCampanion;
    }

    protected override void Initialized()
    {
        base.Initialized();
        m_sceneCharactersManager.AddPlayerToCharaterList(this.gameObject, CharacterType.PlayerGunnerCampanion);
    }

    private void Update()
    {
        m_isGrounded = CheckGroudedOrNot();
        m_animator.SetBool("IsGrounded", m_isGrounded);

        if (m_isGrounded)
        {
            m_currentAIState.UpdateCurrentState();
        }
    }
}
