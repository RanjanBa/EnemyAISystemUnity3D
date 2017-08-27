using System;
using UnityEngine;
using AIManager_namespace;

public class AIInvestigateState<T> : IAIStateManager where T : AIManager
{
    private T m_aiGunnerOrCombat;
    private float m_sqrOfAttackingRange;
    private int m_investigateAngleIndex = -1;

    public AIInvestigateState(T aiManger, float sqrRange)
    {
        m_aiGunnerOrCombat = aiManger;
        m_sqrOfAttackingRange = sqrRange;
    }

    public void UpdateCurrentState()
    {
        Debug.Log("Investigate State ");

        UpdateInvestigateState();
        ChangeStateConditions();
    }

    private void UpdateInvestigateState()
    {
        m_aiGunnerOrCombat.m_currentAnimationState = AnimationState.WalkingAnimation;

        m_aiGunnerOrCombat.CheckEveryCharacter();
        m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter = m_aiGunnerOrCombat.FindNearestOpponentGameObjectWithType();

        if (m_aiGunnerOrCombat.m_canIHearSomething)
        {
            m_aiGunnerOrCombat.m_canIHearSomething = false;
            m_aiGunnerOrCombat.m_mainDestinationPoint = m_aiGunnerOrCombat.m_offsetPosition;
            m_aiGunnerOrCombat.m_currentDestinationPoint = m_aiGunnerOrCombat.transform.position;
            m_aiGunnerOrCombat.m_invetigate_searchDirection = m_aiGunnerOrCombat.m_mainDestinationPoint - m_aiGunnerOrCombat.transform.position;
            if (Mathf.Abs(Vector3.SqrMagnitude(m_aiGunnerOrCombat.m_invetigate_searchDirection)) <= 0.1f)
            {
                m_aiGunnerOrCombat.m_invetigate_searchDirection = m_aiGunnerOrCombat.transform.forward;
            }
            m_aiGunnerOrCombat.m_navMeshPath = m_aiGunnerOrCombat.CalculateNavmeshPath(m_aiGunnerOrCombat.m_mainDestinationPoint);
        }

        float sqrDistance = Vector3.SqrMagnitude(m_aiGunnerOrCombat.transform.position - m_aiGunnerOrCombat.m_mainDestinationPoint);

        if (sqrDistance < 2f * m_aiGunnerOrCombat.m_ThresholdDistance)
        {
            float angle = 0f;
            m_investigateAngleIndex++;
            if (m_investigateAngleIndex < m_aiGunnerOrCombat.m_AIField.SearchOrInvetigateRegions.Length)
            {
                angle = m_aiGunnerOrCombat.GetSearchInvestigateAngle(m_investigateAngleIndex);
                m_aiGunnerOrCombat.m_invetigate_searchDirection = (Vector3.SqrMagnitude(m_aiGunnerOrCombat.m_invetigate_searchDirection) <= 0.01f) ? m_aiGunnerOrCombat.transform.forward : m_aiGunnerOrCombat.m_invetigate_searchDirection;

                Vector3 vec = Quaternion.Euler(0, angle, 0) * m_aiGunnerOrCombat.m_invetigate_searchDirection;
                Vector3 samplePoint = m_aiGunnerOrCombat.m_offsetPosition + Vector3.Normalize(vec) * 10f;

                Debug.DrawRay(m_aiGunnerOrCombat.m_offsetPosition, m_aiGunnerOrCombat.m_invetigate_searchDirection * 10f, Color.yellow, 10f);
                Debug.DrawRay(m_aiGunnerOrCombat.m_offsetPosition, vec, Color.white, 10f);

                Vector3 outDestinationPoint;

                if (m_aiGunnerOrCombat.GetRandomPointInNavmesh(samplePoint, 3f, out outDestinationPoint))
                {
                    m_aiGunnerOrCombat.m_mainDestinationPoint = outDestinationPoint;
                    m_aiGunnerOrCombat.m_navMeshPath = m_aiGunnerOrCombat.CalculateNavmeshPath(outDestinationPoint);
                }
            }
        }

        m_aiGunnerOrCombat.FollowAlongNavMeshPath(m_aiGunnerOrCombat.m_navMeshPath, Vector3.zero);
    }

    private void ChangeStateConditions()
    {
        if(m_investigateAngleIndex >= m_aiGunnerOrCombat.m_AIField.SearchOrInvetigateRegions.Length)
        {
            ChangeToPatrolState();
            return;
        }

        if (m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject != null)
        {
            if (m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyBoxer || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerBoxerCampanion)
            {
                float sqrDist = Vector3.SqrMagnitude(m_aiGunnerOrCombat.transform.position - m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject.transform.position);
                if (sqrDist > m_sqrOfAttackingRange)
                {
                    ChangeToChaseState();
                }
                else
                {
                    ChangeToBoxingState();
                }
            }
            else if (m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyGunner || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerGunnerCampanion)
            {
                ChangeToCoverState();
            }
        }
    }

    public void ChangeToInvestigateState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToSearchState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToUnwareState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToChaseState()
    {
        m_investigateAngleIndex = -1;
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_chaseAIState;
        m_aiGunnerOrCombat.m_mainDestinationPoint = m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject.transform.position;
        m_aiGunnerOrCombat.m_navMeshPath = m_aiGunnerOrCombat.CalculateNavmeshPath(m_aiGunnerOrCombat.m_mainDestinationPoint);
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

        m_investigateAngleIndex = -1;
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_coverAIState;
    }

    public void ChangeToGunFireState()
    {
        m_investigateAngleIndex = -1;
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_gunFireAIState;
        m_aiGunnerOrCombat.m_isInCover = false;
        m_aiGunnerOrCombat.m_animator.SetBool("IsCover", m_aiGunnerOrCombat.m_isInCover);
    }

    public void ChangeToBoxingState()
    {
        m_investigateAngleIndex = -1;
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_boxingAIState;
        m_aiGunnerOrCombat.StartCoroutine(m_aiGunnerOrCombat.ChangeAnimationLayer(1, 0));
    }

    public void ChangeToPatrolState()
    {
        m_investigateAngleIndex = -1;
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_patrolAIState;
        m_aiGunnerOrCombat.m_mainDestinationPoint = m_aiGunnerOrCombat.m_FollowPath.pathTransformPosition[m_aiGunnerOrCombat.m_currentIndexOfGivenPath].position;
        m_aiGunnerOrCombat.m_navMeshPath = m_aiGunnerOrCombat.CalculateNavmeshPath(m_aiGunnerOrCombat.m_mainDestinationPoint);
        if (m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyGunner || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerGunnerCampanion)
        {
            m_aiGunnerOrCombat.UnEquipWeapon(); //This will also change the layer weight of Gun State to simple Locomotion
        }
    }
}
