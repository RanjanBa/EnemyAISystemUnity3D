using System;
using AIManager_namespace;
using UnityEngine;

public class AIBoxingState : IAIStateManager
{
    private AIBoxerManager m_aiBoxer;
    private float m_sqrOfAttackingRange, m_sqrKickDist, m_sqrPunchDist;
    private float m_sqrDistFromOpponent = float.MaxValue;
    private float m_attackingTimer = 0f;
    private Vector3 m_trackOpponentPosition;
    private float m_thresholdDistance = 0f;

    public AIBoxingState(AIBoxerManager aiBoxer,float m_charRadius, float sqrAttackingRange, float sqrKickDist, float sqrPunchDist)
    {
        m_aiBoxer = aiBoxer;
        m_thresholdDistance = 2f * (m_charRadius + 0.2f) * (m_charRadius + 0.2f);
        m_sqrOfAttackingRange = sqrAttackingRange;
        m_sqrKickDist = sqrKickDist;
        m_sqrPunchDist = sqrPunchDist;
    }

    public void UpdateCurrentState()
    {
        Debug.Log("Boxing State " + m_thresholdDistance);

        UpdateBoxingState();
        ChangeStateConditions();
    }

    private void UpdateBoxingState()
    {
        m_aiBoxer.CheckEveryCharacter();
        m_aiBoxer.m_nearestOpponentVisibleCharacter = m_aiBoxer.FindNearestOpponentGameObjectWithType();

        if (m_aiBoxer.m_nearestOpponentVisibleCharacter.characterGameObject != null)
        {
            m_trackOpponentPosition = m_aiBoxer.m_nearestOpponentVisibleCharacter.characterGameObject.transform.position;
            m_aiBoxer.m_navMeshPath = m_aiBoxer.CalculateNavmeshPath(m_trackOpponentPosition);
            m_sqrDistFromOpponent = Vector3.SqrMagnitude(m_trackOpponentPosition - m_aiBoxer.transform.position);

            if (m_sqrDistFromOpponent < m_sqrOfAttackingRange)
            {
                m_attackingTimer += Time.deltaTime;

                if (m_attackingTimer >= m_aiBoxer.m_AIBoxerField.AttackRate)
                {
                    if (m_sqrDistFromOpponent < m_sqrPunchDist)
                    {
                        m_attackingTimer = 0f;
                        m_aiBoxer.Punch(m_sqrDistFromOpponent);
                    }
                    else if (m_sqrDistFromOpponent < m_sqrKickDist)
                    {
                        m_attackingTimer = 0f;
                        m_aiBoxer.Kick(m_sqrDistFromOpponent);
                    }
                }
            }
        }

        if (m_aiBoxer.m_navMeshPath.corners.Length >= 2 && m_sqrDistFromOpponent > m_thresholdDistance)
        {
            m_aiBoxer.m_currentAnimationState = AnimationState.WalkingAnimation;
            m_aiBoxer.FollowAlongNavMeshPath(m_aiBoxer.m_navMeshPath, Vector3.zero);
        }
        else
        {
            m_aiBoxer.m_currentAnimationState = AnimationState.IdleAnimation;
            m_aiBoxer.ControlMovement(m_aiBoxer.transform.forward, 30f);
        }
    }

    private void ChangeStateConditions()
    {
        if(m_aiBoxer.m_nearestOpponentVisibleCharacter.characterGameObject != null)
        {
            if(m_sqrDistFromOpponent > m_sqrOfAttackingRange)
            {
                ChangeToChaseState();
            }
        }
        else
        {
            ChangeToSearchState();
        }
    }

    public void ChangeToBoxingState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToGunFireState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToInvestigateState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToUnwareState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToPatrolState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToCoverState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToChaseState()
    {
        m_aiBoxer.m_currentAIState = m_aiBoxer.m_chaseAIState;
        if (m_aiBoxer.m_animator.GetLayerWeight(1) > 0.9f)
        {
            Debug.Log("Change to Chase state from boxing state...");
            m_aiBoxer.StartCoroutine(m_aiBoxer.ChangeAnimationLayer(0, 1));
        }
    }

    public void ChangeToSearchState()
    {
        m_aiBoxer.m_currentAIState = m_aiBoxer.m_searchAIState;
        m_aiBoxer.StartCoroutine(m_aiBoxer.ChangeAnimationLayer(0, 1));

        m_aiBoxer.m_mainDestinationPoint = m_trackOpponentPosition;
        m_aiBoxer.m_offsetPosition = m_aiBoxer.m_mainDestinationPoint;
        m_aiBoxer.m_navMeshPath = m_aiBoxer.CalculateNavmeshPath(m_aiBoxer.m_mainDestinationPoint);
        m_aiBoxer.m_invetigate_searchDirection = m_aiBoxer.m_mainDestinationPoint - m_aiBoxer.transform.position;
        if (Mathf.Abs(Vector3.SqrMagnitude(m_aiBoxer.m_invetigate_searchDirection)) <= 0.1f)
        {
            m_aiBoxer.m_invetigate_searchDirection = m_aiBoxer.transform.forward;
        }
        m_aiBoxer.m_invetigate_searchDirection.y = 0f;
    }
}
