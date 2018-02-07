using UnityEngine;
using AIManager_namespace;

public class AIPatrolState<T> : IAIStateManager where T : AIManager
{
    private T m_aiGunnerOrCombat;
    private float m_sqrOfAttackingRange = float.MaxValue;
    private float m_patrolTimer = 0f;
    private float m_patrolTime;

    public AIPatrolState(T aiManager, float sqrOfRange)
    {
        m_aiGunnerOrCombat = aiManager;
        m_sqrOfAttackingRange = sqrOfRange;
        m_patrolTime = Random.Range(m_aiGunnerOrCombat.m_AIField.m_MinMaxPatrolTime.MinValue, m_aiGunnerOrCombat.m_AIField.m_MinMaxPatrolTime.MaxValue);
    }

    public void OnStateEnter()
    {
        Debug.Log("Enter patrol state...");
        m_aiGunnerOrCombat.m_currentAnimationState = AnimationState.WalkingAnimation;
    }

    public void UpdateCurrentState()
    {
        m_patrolTimer += Time.deltaTime;

        UpdatePatrolState();
        ChangeStateConditions();
    }
    
    private void UpdatePatrolState()
    {
        m_aiGunnerOrCombat.m_currentAnimationState = AnimationState.WalkingAnimation;
        float sqrDistance = Vector3.SqrMagnitude(m_aiGunnerOrCombat.transform.position - m_aiGunnerOrCombat.m_mainDestinationPoint);

        if (sqrDistance < 2f * m_aiGunnerOrCombat.m_ThresholdDistance)
        {
            m_aiGunnerOrCombat.m_mainDestinationPoint = m_aiGunnerOrCombat.GetNextDestinationPointFromGivenPath();
            m_aiGunnerOrCombat.m_navMeshPath = m_aiGunnerOrCombat.CalculateNavmeshPath(m_aiGunnerOrCombat.m_mainDestinationPoint);
        }

        if (m_aiGunnerOrCombat.m_navMeshPath != null)
        {
            m_aiGunnerOrCombat.FollowAlongNavMeshPath(m_aiGunnerOrCombat.m_navMeshPath, Vector3.zero);
        }

        m_aiGunnerOrCombat.CheckEveryCharacterForInVisualRange();
        m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter = m_aiGunnerOrCombat.FindNearestOpponentGameObjectWithType();
    }

    private void ChangeStateConditions()
    {
        if (m_aiGunnerOrCombat.m_canIHearSomething)
        {
            ChangeToSearchState();
            m_aiGunnerOrCombat.m_canIHearSomething = false;
            return;
        }

        if(m_patrolTimer >= m_patrolTime && Vector3.SqrMagnitude(m_aiGunnerOrCombat.transform.position - m_aiGunnerOrCombat.m_mainDestinationPoint) < 2f * m_aiGunnerOrCombat.m_ThresholdDistance)
        {
            ChangeToUnwareState();
            return;
        }

        if (m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject != null)
        {
            float sqrDist = Vector3.SqrMagnitude(m_aiGunnerOrCombat.transform.position - m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject.transform.position);
            if (m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyBoxer || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerBoxerCampanion)
            {
                if (sqrDist > m_sqrOfAttackingRange)
                {
                    ChangeToChaseState();
                }
                else
                {
                    ChangeToBoxingState();
                }
            }
            else if(m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyGunner || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerGunnerCampanion)
            {
                if (sqrDist <= m_aiGunnerOrCombat.m_AIField.RangeOfSearchingCoverPos)
                {
                    ChangeToCoverState();
                }
                else
                {
                    ChangeToGunFireState();
                }
            }
        }
    }

    public void ChangeToPatrolState()
    {
        throw new System.NotImplementedException();
    }

    public void ChangeToSearchState()
    {
        m_aiGunnerOrCombat.m_searchAIState.m_offsetPosition = m_aiGunnerOrCombat.m_mainDestinationPoint;
        m_aiGunnerOrCombat.m_searchAIState.m_invetigate_searchDirection = m_aiGunnerOrCombat.m_mainDestinationPoint - m_aiGunnerOrCombat.transform.position;
        if (Mathf.Abs(Vector3.SqrMagnitude(m_aiGunnerOrCombat.m_searchAIState.m_invetigate_searchDirection)) <= 0.1f)
        {
            m_aiGunnerOrCombat.m_searchAIState.m_invetigate_searchDirection = m_aiGunnerOrCombat.transform.forward;
        }

        m_aiGunnerOrCombat.ChangeAIState(m_aiGunnerOrCombat.m_searchAIState);
    }

    public void ChangeToChaseState()
    {
        m_aiGunnerOrCombat.m_mainDestinationPoint = m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject.transform.position;
        m_aiGunnerOrCombat.m_navMeshPath = m_aiGunnerOrCombat.CalculateNavmeshPath(m_aiGunnerOrCombat.m_mainDestinationPoint);

        if(m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyGunner || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerGunnerCampanion)
        {
            m_aiGunnerOrCombat.m_gunFireAIState.EquipWeapon();
        }

        m_aiGunnerOrCombat.ChangeAIState(m_aiGunnerOrCombat.m_chaseAIState);
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
                m_aiGunnerOrCombat.m_navMeshPath.ClearCorners();
                m_aiGunnerOrCombat.m_mainDestinationPoint = m_aiGunnerOrCombat.transform.position;
                ChangeToGunFireState();
            }
            else
            {
                ChangeToChaseState();
            }

            return;
        }

        if (m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyGunner || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerGunnerCampanion)
        {
            m_aiGunnerOrCombat.m_gunFireAIState.EquipWeapon();
        }
        m_aiGunnerOrCombat.ChangeAIState(m_aiGunnerOrCombat.m_coverAIState);
    }

    public void ChangeToGunFireState()
    {
        m_aiGunnerOrCombat.m_gunFireAIState.EquipWeapon();
        m_aiGunnerOrCombat.ChangeAIState(m_aiGunnerOrCombat.m_gunFireAIState);
    }

    public void ChangeToBoxingState()
    {
        m_aiGunnerOrCombat.ChangeAIState(m_aiGunnerOrCombat.m_boxingAIState);
        m_aiGunnerOrCombat.StartCoroutine(m_aiGunnerOrCombat.ChangeAnimationLayer(1, 0));
    }

    public void ChangeToUnwareState()
    {
        m_aiGunnerOrCombat.m_navMeshPath.ClearCorners();
        m_aiGunnerOrCombat.ChangeAIState(m_aiGunnerOrCombat.m_unwareAIState);
    }

    public void OnStateExit()
    {
        ResetPatrolState();
        Debug.Log("Exit patrol state...");
    }

    private void ResetPatrolState()
    {
        m_patrolTimer = 0f;
        m_patrolTime = Random.Range(m_aiGunnerOrCombat.m_AIField.m_MinMaxPatrolTime.MinValue, m_aiGunnerOrCombat.m_AIField.m_MinMaxPatrolTime.MaxValue);
    }
}
