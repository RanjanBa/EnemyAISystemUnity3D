using AIManager_namespace;
using UnityEngine;

public class EnemyGunner : AIGunnerManager
{
    private void Start()
    {
        Initialized();
        m_CharType = CharacterType.EnemyGunner;
    }

    protected override void Initialized()
    {
        base.Initialized();
        m_sceneCharactersManager.AddEnemyToCharaterList(this.gameObject, CharacterType.EnemyGunner);
    }

    private void Update()
    {
        m_isGrounded = CheckGroudedOrNot();
        m_animator.SetBool("IsGrounded", m_isGrounded);

        if (m_isGrounded)
        {
            if (m_currentAIState != null)
            {
                m_currentAIState.UpdateCurrentState();
            }
            else
            {
                Debug.LogError("m_currentAIState is null");
            }
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10, DamageType.DamageByGun);
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
        else if (m_currentAIState == m_gunFireAIState)
        {
            AIIndex = 5;
        }
    }
}