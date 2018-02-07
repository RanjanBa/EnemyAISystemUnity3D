using UnityEngine;
using AIManager_namespace;

public class PlayerBoxerCampanion : AIBoxerManager
{
    private void Start()
    {
        Initialized();
        m_CharType = CharacterType.PlayerBoxerCampanion;
    }

    protected override void Initialized()
    {
        base.Initialized();
        m_sceneCharactersManager.AddPlayerToCharaterList(this.gameObject, CharacterType.PlayerBoxerCampanion);
    }

    private void Update()
    {
        m_isGrounded = CheckGroudedOrNot();
        m_animator.SetBool("IsGrounded", m_isGrounded);

        if (m_isGrounded)
        {
            m_currentAIState.UpdateCurrentState();
        }

        InWhatState();
    }

    private void InWhatState()
    {
        if (m_currentAIState == m_unwareAIState)
        {
            AIIndex = 0;
        }
        else if (m_currentAIState == m_patrolAIState)
        {
            AIIndex = 1;
        }
        else if (m_currentAIState == m_searchAIState)
        {
            AIIndex = 2;
        }
        else if (m_currentAIState == m_chaseAIState)
        {
            AIIndex = 3;
        }
        else if (m_currentAIState == m_boxingAIState)
        {
            AIIndex = 4;
        }
    }
}
