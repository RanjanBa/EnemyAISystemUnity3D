using AIManager_namespace;
using UnityEngine;

public class AIUnwareState<T> : IAIStateManager where T : AIManager
{
    private T m_aiGunnerOrCombat;
    private float m_sqrOfAttackRange = float.MaxValue;
    private float m_unwareStateTimer = 0f;
    private float m_unwareTime;

    public AIUnwareState(T aiGunnerOrCombat, float sqrOfAttackingRange)
    {
        m_aiGunnerOrCombat = aiGunnerOrCombat;
        m_sqrOfAttackRange = sqrOfAttackingRange;
        m_unwareTime = Random.Range(m_aiGunnerOrCombat.m_AIField.m_MinMaxUnwareTime.MinValue, m_aiGunnerOrCombat.m_AIField.m_MinMaxUnwareTime.MaxValue);
    }

    public void OnStateEnter()
    {
        Debug.Log("Enter of unwareState...");
        m_aiGunnerOrCombat.m_currentAnimationState = AnimationState.IdleAnimation;
    }

    public void UpdateCurrentState()
    {
        m_unwareStateTimer += Time.deltaTime;
        UpdateUnwareState();
        ChangeStateConditions();
    }

    private void UpdateUnwareState()
    {
        m_aiGunnerOrCombat.CheckEveryCharacterForInVisualRange();
        m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter = m_aiGunnerOrCombat.FindNearestOpponentGameObjectWithType();
    }

    private void ChangeStateConditions()
    {
        if(m_unwareStateTimer >= m_unwareTime)
        {
            ChangeToPatrolState();
            return;
        }

        if (m_aiGunnerOrCombat.m_canIHearSomething)
        {
            m_aiGunnerOrCombat.m_canIHearSomething = false;
            ChangeToSearchState();
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

        if (m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyGunner || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerGunnerCampanion)
        {
            m_aiGunnerOrCombat.m_gunFireAIState.EquipWeapon();
        }
        m_aiGunnerOrCombat.ChangeAIState(m_aiGunnerOrCombat.m_coverAIState);
    }

    public void ChangeToGunFireState()
    {
        m_aiGunnerOrCombat.m_isInCover = false;
        m_aiGunnerOrCombat.UpdateCoverAnimation(false, false);
        m_aiGunnerOrCombat.m_gunFireAIState.EquipWeapon();
        m_aiGunnerOrCombat.ChangeAIState(m_aiGunnerOrCombat.m_gunFireAIState);
    }

    public void ChangeToBoxingState()
    {
        m_aiGunnerOrCombat.StartCoroutine(m_aiGunnerOrCombat.ChangeAnimationLayer(1, 0));
        m_aiGunnerOrCombat.ChangeAIState(m_aiGunnerOrCombat.m_boxingAIState);
    }

    public void ChangeToPatrolState()
    {
        m_aiGunnerOrCombat.m_mainDestinationPoint = m_aiGunnerOrCombat.m_FollowPath.pathTransformPosition[m_aiGunnerOrCombat.m_currentIndexOfGivenPath].position;
        m_aiGunnerOrCombat.m_navMeshPath = m_aiGunnerOrCombat.CalculateNavmeshPath(m_aiGunnerOrCombat.m_mainDestinationPoint);
        m_aiGunnerOrCombat.ChangeAIState(m_aiGunnerOrCombat.m_patrolAIState);
    }

    public void OnStateExit()
    {
        ResetUnwareState();
        Debug.Log("Exit of unwareState...");
    }

    private void ResetUnwareState()
    {
        m_unwareStateTimer = 0f;
        m_unwareTime = Random.Range(m_aiGunnerOrCombat.m_AIField.m_MinMaxUnwareTime.MinValue, m_aiGunnerOrCombat.m_AIField.m_MinMaxUnwareTime.MaxValue);
    }
}
