using UnityEngine;
using AIManager_namespace;

public class EnemyBoxer : AIBoxerManager
{
    private void Start()
    {
        Initialized();
    }

    protected override void Initialized()
    {
        base.Initialized();
        m_CharType = CharacterType.EnemyBoxer;
        m_sceneCharactersManager.AddEnemyToCharaterList(this.gameObject, CharacterType.EnemyBoxer);
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
        else if (m_currentAIState == m_coverAIState)
        {
            AIIndex = 4;
        }
        else if (m_currentAIState == m_boxingAIState)
        {
            AIIndex = 5;
        }
    }

    private void FixedUpdate()
    {
    }
}