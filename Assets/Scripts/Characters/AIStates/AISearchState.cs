using AIManager_namespace;
using UnityEngine;

public class AISearchState<T> : IAIStateManager where T : AIManager
{
    private T m_aiGunnerOrCombat;
    private float m_sqrOfAttackingRange;
    private int m_searchAngleIndex = -1;

    public Vector3 m_offsetPosition;
    public Vector3 m_invetigate_searchDirection;

    public AISearchState(T aiManager, float sqrAttackingRange)
    {
        m_aiGunnerOrCombat = aiManager;
        m_sqrOfAttackingRange = sqrAttackingRange;
    }

    public void OnStateEnter()
    {
        Debug.Log("Enter Search State...");
        m_aiGunnerOrCombat.m_currentAnimationState = AnimationState.WalkingAnimation;
    }

    public void UpdateCurrentState()
    {
        Debug.Log("Search State " + m_searchAngleIndex);

        UpdateSearchState();
        ChangeStateConditions();
    }
    
    private void UpdateSearchState()
    {
        if (m_aiGunnerOrCombat.m_canIHearSomething)
        {
            m_aiGunnerOrCombat.m_canIHearSomething = false;
            m_offsetPosition = m_aiGunnerOrCombat.m_mainDestinationPoint;
            m_invetigate_searchDirection = m_aiGunnerOrCombat.m_mainDestinationPoint - m_aiGunnerOrCombat.transform.position;
            if (Mathf.Abs(Vector3.SqrMagnitude(m_invetigate_searchDirection)) <= 0.1f)
            {
                m_invetigate_searchDirection = m_aiGunnerOrCombat.transform.forward;
            }
        }

        m_aiGunnerOrCombat.CheckEveryCharacterForInVisualRange();
        m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter = m_aiGunnerOrCombat.FindNearestOpponentGameObjectWithType();

        float sqrDistance = Vector3.SqrMagnitude(m_aiGunnerOrCombat.transform.position - m_aiGunnerOrCombat.m_mainDestinationPoint);

        if (sqrDistance < 2f * m_aiGunnerOrCombat.m_ThresholdDistance)
        {
            float angle = 0f;
            m_searchAngleIndex++;

            if (m_searchAngleIndex < m_aiGunnerOrCombat.m_AIField.SearchRegions.Length)
            {
                angle = m_aiGunnerOrCombat.GetSearchInvestigateAngle(m_searchAngleIndex);

                if (Vector3.SqrMagnitude(m_invetigate_searchDirection) <= 0.01f)
                {
                    Debug.LogError("SqrMagnitude of m_invetigate_searchDirection is less than 0.01f");
                }

                Vector3 vec = Quaternion.Euler(0, angle, 0) * m_invetigate_searchDirection;
                Vector3 samplePoint = m_offsetPosition + Vector3.Normalize(vec) * 10f;
                Vector3 outDestinationPoint;

                if (m_aiGunnerOrCombat.GetRandomPointInNavmesh(samplePoint, 5f, out outDestinationPoint))
                {
                    m_aiGunnerOrCombat.m_navMeshPath = m_aiGunnerOrCombat.CalculateNavmeshPath(outDestinationPoint);
                    m_aiGunnerOrCombat.m_mainDestinationPoint = outDestinationPoint;
                }
            }
        }

        m_aiGunnerOrCombat.FollowAlongNavMeshPath(m_aiGunnerOrCombat.m_navMeshPath, Vector3.zero);
    }

    private void ChangeStateConditions()
    {
        if(m_searchAngleIndex >= m_aiGunnerOrCombat.m_AIField.SearchRegions.Length)
        {
            ChangeToPatrolState();
            return;
        }

        if (m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject != null)
        {
            if (m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyBoxer || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerBoxerCampanion)
            {
                float sqrDist = Vector3.SqrMagnitude(m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject.transform.position - m_aiGunnerOrCombat.transform.position);

                if (sqrDist < m_sqrOfAttackingRange)
                {
                    ChangeToBoxingState();
                }
                else
                {
                    ChangeToChaseState();
                }
            }
            else
            {
                ChangeToCoverState();
            }
        }
    }

    public void ChangeToSearchState()
    {
        throw new System.NotImplementedException();
    }

    public void ChangeToChaseState()
    {
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

        if (m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyGunner || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerGunnerCampanion)
        {
            m_aiGunnerOrCombat.m_gunFireAIState.UnEquipWeapon(); //This will also change the layer weight of Gun State to simple Locomotion
        }

        m_aiGunnerOrCombat.ChangeAIState(m_aiGunnerOrCombat.m_patrolAIState);
    }

    public void ChangeToUnwareState()
    {
        m_aiGunnerOrCombat.m_navMeshPath.ClearCorners();

        if (m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyGunner || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerGunnerCampanion)
        {
            m_aiGunnerOrCombat.m_gunFireAIState.UnEquipWeapon();
        }

        m_aiGunnerOrCombat.ChangeAIState(m_aiGunnerOrCombat.m_unwareAIState);
    }

    public void OnStateExit()
    {
        ResetSearchState();
        Debug.Log("Exit Search State...");
    }

    private void ResetSearchState()
    {
        m_searchAngleIndex = -1;
    }
}
