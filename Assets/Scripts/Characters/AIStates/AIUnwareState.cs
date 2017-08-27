using AIManager_namespace;
using UnityEngine;

public class AIUnwareState<T> : IAIStateManager where T : AIManager
{
    private T m_aiGunnerOrCombat;
    private float m_sqrOfAttackRange = float.MaxValue;
    private float m_unwareStateTimer = 0f;

    public AIUnwareState(T aiGunnerOrCombat, float sqrOfAttackingRange)
    {
        m_aiGunnerOrCombat = aiGunnerOrCombat;
        m_sqrOfAttackRange = sqrOfAttackingRange;
    }

    public void UpdateCurrentState()
    {
        Debug.Log("Unware State...");

        m_unwareStateTimer += Time.deltaTime;
        m_aiGunnerOrCombat.m_canMove = false;
        UpdateUnwareState();
        ChangeStateConditions();
    }

    private void UpdateUnwareState()
    {
        float m_speed = m_aiGunnerOrCombat.m_animator.GetFloat("Speed");
        m_speed = Mathf.Lerp(m_speed, 0f, Time.deltaTime * 2f);
        m_aiGunnerOrCombat.m_animator.SetFloat("Speed", m_speed);

        m_aiGunnerOrCombat.CheckEveryCharacter();
        m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter = m_aiGunnerOrCombat.FindNearestOpponentGameObjectWithType();
    }

    private void ChangeStateConditions()
    {
        if(m_unwareStateTimer >= m_aiGunnerOrCombat.m_unwareTime)
        {
            ChangeToPatrolState();
            return;
        }

        if (m_aiGunnerOrCombat.m_canIHearSomething)
        {
            m_aiGunnerOrCombat.m_canIHearSomething = false;
            ChangeToInvestigateState();
            return;
        }

        if (m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject != null)
        {
            if (m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyBoxer || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerBoxerCampanion)
            {
                float sqrDist = Vector3.SqrMagnitude(m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject.transform.position - m_aiGunnerOrCombat.transform.position);
                if (sqrDist <= m_sqrOfAttackRange)
                {
                    ChangeToBoxingState();
                    return;
                }
                else
                {
                    ChangeToChaseState();
                    return;
                }
            }
            else if (m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyGunner || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerGunnerCampanion)
            {
                ChangeToCoverState();
                return;
            }
        }
    }

    public void ChangeToUnwareState()
    {
        throw new System.NotImplementedException();
    }

    public void ChangeToSearchState()
    {
        throw new System.NotImplementedException();
    }

    public void ChangeToChaseState()
    {
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_chaseAIState;
        m_aiGunnerOrCombat.m_mainDestinationPoint = m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject.transform.position;
        m_aiGunnerOrCombat.m_navMeshPath = m_aiGunnerOrCombat.CalculateNavmeshPath(m_aiGunnerOrCombat.m_mainDestinationPoint);

        if (m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyGunner || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerGunnerCampanion)
        {
            m_aiGunnerOrCombat.EquipWithWeapon();
        }

        ResetUnwareState();
    }

    public void ChangeToCoverState()
    {
        if (m_aiGunnerOrCombat.CanFindBestCoverPosition(ref m_aiGunnerOrCombat.m_coverPositionScript))
        {
            m_aiGunnerOrCombat.m_coverPositionScript.m_isOccupied = true;
            m_aiGunnerOrCombat.m_mainDestinationPoint = m_aiGunnerOrCombat.m_coverPositionScript.transform.position;
            m_aiGunnerOrCombat.m_navMeshPath = m_aiGunnerOrCombat.CalculateNavmeshPath(m_aiGunnerOrCombat.m_mainDestinationPoint);
            Debug.Log("I can find the cover Position");
        }
        else
        {
            Debug.Log("Can't find the cover Position");
            if (m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyGunner || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerGunnerCampanion)
            {
                ChangeToGunFireState();
                m_aiGunnerOrCombat.m_navMeshPath.ClearCorners();
                m_aiGunnerOrCombat.m_mainDestinationPoint = m_aiGunnerOrCombat.transform.position;
            }
            else
            {
                ChangeToChaseState();
            }
            return;
        }

        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_coverAIState;
        if (m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyGunner || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerGunnerCampanion)
        {
            m_aiGunnerOrCombat.EquipWithWeapon();
        }
        ResetUnwareState();
    }

    public void ChangeToGunFireState()
    {
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_gunFireAIState;
        m_aiGunnerOrCombat.m_isInCover = false;
        m_aiGunnerOrCombat.m_animator.SetBool("IsCover", m_aiGunnerOrCombat.m_isInCover);
        m_aiGunnerOrCombat.EquipWithWeapon();
        ResetUnwareState();
    }

    public void ChangeToBoxingState()
    {
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_boxingAIState;
        m_aiGunnerOrCombat.StartCoroutine(m_aiGunnerOrCombat.ChangeAnimationLayer(1, 0));
        ResetUnwareState();
    }

    public void ChangeToInvestigateState()
    {
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_investigateAIState;
        if (m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyGunner || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerGunnerCampanion)
        {
            m_aiGunnerOrCombat.EquipWithWeapon();
        }
        m_aiGunnerOrCombat.m_mainDestinationPoint = m_aiGunnerOrCombat.m_offsetPosition;
        m_aiGunnerOrCombat.m_navMeshPath = m_aiGunnerOrCombat.CalculateNavmeshPath(m_aiGunnerOrCombat.m_mainDestinationPoint);
        m_aiGunnerOrCombat.m_invetigate_searchDirection = m_aiGunnerOrCombat.m_mainDestinationPoint - m_aiGunnerOrCombat.transform.position;
        if (Mathf.Abs(Vector3.SqrMagnitude(m_aiGunnerOrCombat.m_invetigate_searchDirection)) <= 0.1f)
        {
            m_aiGunnerOrCombat.m_invetigate_searchDirection = m_aiGunnerOrCombat.transform.forward;
        }
        m_aiGunnerOrCombat.m_invetigate_searchDirection.y = 0f;
        ResetUnwareState();
    }

    public void ChangeToPatrolState()
    {
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_patrolAIState;
        m_aiGunnerOrCombat.m_mainDestinationPoint = m_aiGunnerOrCombat.m_FollowPath.pathTransformPosition[m_aiGunnerOrCombat.m_currentIndexOfGivenPath].position;
        m_aiGunnerOrCombat.m_navMeshPath = m_aiGunnerOrCombat.CalculateNavmeshPath(m_aiGunnerOrCombat.m_mainDestinationPoint);
        ResetUnwareState();
    }

    private void ResetUnwareState()
    {
        m_aiGunnerOrCombat.m_canMove = true;
        m_unwareStateTimer = 0f;
        m_aiGunnerOrCombat.m_unwareTime = Random.Range(m_aiGunnerOrCombat.m_AIField.m_MinMaxUnwareTime.MinValue, m_aiGunnerOrCombat.m_AIField.m_MinMaxUnwareTime.MaxValue);
    }
}
