using System;
using UnityEngine;
using AIManager_namespace;

public class AICoverState<T> : IAIStateManager where T: AIManager
{
    private T m_aiGunnerOrCombat;
    private float m_sqrDistanceFromCoverPosition = float.MaxValue;
    private Vector3 m_trackCharacterPosition;
    private float m_coverTime = 0f;
    private float m_coverTimer = 0f;

    public AICoverState(T aiManager)
    {
        m_aiGunnerOrCombat = aiManager;
        m_coverTime = UnityEngine.Random.Range(m_aiGunnerOrCombat.m_AIField.ChangeCoverPositionTime.MinValue, m_aiGunnerOrCombat.m_AIField.ChangeCoverPositionTime.MaxValue);
    }

    public void UpdateCurrentState()
    {
        Debug.Log("Cover State..." + m_aiGunnerOrCombat.m_CharType.ToString());
        m_aiGunnerOrCombat.m_animator.SetFloat("Strafe", 1f);

        UpdateCoverState();
        ChangeStateConditions();
    }

    private void UpdateCoverState()
    {
        m_aiGunnerOrCombat.CheckEveryCharacter();
        m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter = m_aiGunnerOrCombat.FindNearestOpponentGameObjectWithType();

        if (m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject != null)
        {
            if (!m_aiGunnerOrCombat.m_isInCover)
            {
                m_trackCharacterPosition = m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject.transform.position;
            }
        }

        m_sqrDistanceFromCoverPosition = Vector3.SqrMagnitude(m_aiGunnerOrCombat.transform.position - m_aiGunnerOrCombat.m_mainDestinationPoint);

        if (m_aiGunnerOrCombat.m_isInCover)
        {
            m_aiGunnerOrCombat.m_isInHighCover = m_aiGunnerOrCombat.CheckForLowOrHighCover();
            m_coverTimer += Time.deltaTime;

            Vector3 fromAgentToOpponentDirection = m_trackCharacterPosition - m_aiGunnerOrCombat.transform.position;
            Debug.DrawRay(m_aiGunnerOrCombat.transform.position + Vector3.up * 0.5f, fromAgentToOpponentDirection.normalized * 3f, Color.magenta, 3f);

            RaycastHit hitInfo;
            if (Physics.Raycast(m_aiGunnerOrCombat.transform.position + Vector3.up * 0.5f, fromAgentToOpponentDirection, out hitInfo, 3f, m_aiGunnerOrCombat.m_EnvironmentLayerForCover, QueryTriggerInteraction.Ignore))
            {
                Debug.Log("Does hit the cover layerMask");
                Vector3 dir = -hitInfo.normal;
                Vector3 axisSign = Vector3.Cross(m_aiGunnerOrCombat.transform.forward, dir);
                float angle = Vector3.Angle(m_aiGunnerOrCombat.transform.forward, dir) * (axisSign.y >= 0 ? 1f : -1f);
                m_aiGunnerOrCombat.ControlMovement(m_aiGunnerOrCombat.transform.right, 30f);
                m_aiGunnerOrCombat.ControlRotation(dir);
                m_aiGunnerOrCombat.ControlRotationAnimation(angle);
            }
            else
            {
                m_aiGunnerOrCombat.m_isInCover = false;
                m_aiGunnerOrCombat.m_animator.SetBool("IsCover", false);
                m_aiGunnerOrCombat.Crouch(false);
                FindCoverPosition();
            }
        }
        else
        {
            if (m_sqrDistanceFromCoverPosition <= 2f * m_aiGunnerOrCombat.m_ThresholdDistance)
            {
                m_aiGunnerOrCombat.m_currentAnimationState = AnimationState.IdleAnimation;
                m_aiGunnerOrCombat.m_currentDestinationPoint = m_aiGunnerOrCombat.m_mainDestinationPoint;

                int rand = UnityEngine.Random.Range(1, 101);
                if (!m_aiGunnerOrCombat.m_isInCover && rand > 90)
                {
                    m_aiGunnerOrCombat.m_isInCover = true;
                    m_aiGunnerOrCombat.m_isInHighCover = m_aiGunnerOrCombat.CheckForLowOrHighCover();
                    if (!m_aiGunnerOrCombat.m_isInHighCover)
                    {
                        m_aiGunnerOrCombat.Crouch(true);
                    }
                    m_aiGunnerOrCombat.m_animator.SetBool("IsLeftSide", true);
                    m_aiGunnerOrCombat.m_animator.SetBool("IsCover", true);
                }
                else
                {
                    m_aiGunnerOrCombat.m_isInCover = false;
                    m_aiGunnerOrCombat.m_animator.SetBool("IsCover", false);
                }

            }
            else if (m_aiGunnerOrCombat.m_navMeshPath.corners.Length <= 2)
            {
                m_aiGunnerOrCombat.m_currentDestinationPoint = m_aiGunnerOrCombat.m_mainDestinationPoint;
                m_aiGunnerOrCombat.m_currentAnimationState = AnimationState.RunningAnimation;
                m_aiGunnerOrCombat.FollowAlongNavMeshPath(m_aiGunnerOrCombat.m_navMeshPath, m_trackCharacterPosition, true);
            }
            else
            {
                m_aiGunnerOrCombat.m_currentAnimationState = AnimationState.RunningAnimation;
                m_aiGunnerOrCombat.FollowAlongNavMeshPath(m_aiGunnerOrCombat.m_navMeshPath, m_trackCharacterPosition, true);
            }
        }
    }

    private void ChangeStateConditions()
    {
        if (m_aiGunnerOrCombat.m_isInCover)
        {
            if (m_coverTimer > m_coverTime)
            {
                if (m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyGunner || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerGunnerCampanion)
                {
                    ChangeToGunFireState();
                }
            }
        }
    }

    private bool FindCoverPosition()
    {
        if (m_aiGunnerOrCombat.CanFindBestCoverPosition(ref m_aiGunnerOrCombat.m_coverPositionScript))
        {
            m_aiGunnerOrCombat.m_coverPositionScript.m_isOccupied = true;
            m_aiGunnerOrCombat.m_mainDestinationPoint = m_aiGunnerOrCombat.m_coverPositionScript.transform.position;
            m_aiGunnerOrCombat.m_navMeshPath = m_aiGunnerOrCombat.CalculateNavmeshPath(m_aiGunnerOrCombat.m_mainDestinationPoint);
            Debug.Log("I can find the cover Position");
            return true;
        }

        Debug.Log("Can't find the cover Position");
        m_aiGunnerOrCombat.m_currentDestinationPoint = m_aiGunnerOrCombat.m_mainDestinationPoint;
        return false;
    }

    public void ChangeToCoverState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToUnwareState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToInvestigateState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToPatrolState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToChaseState()
    {
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_chaseAIState;
        m_aiGunnerOrCombat.m_isInCover = false;
        m_aiGunnerOrCombat.Crouch(false);
        m_aiGunnerOrCombat.m_animator.SetBool("IsCover", false);
        ResetCoverState();
    }

    public void ChangeToBoxingState()
    {
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_boxingAIState;
        m_aiGunnerOrCombat.StartCoroutine(m_aiGunnerOrCombat.ChangeAnimationLayer(1, 0));
    }

    public void ChangeToGunFireState()
    {
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_gunFireAIState;
        ResetCoverState();
    }

    public void ChangeToSearchState()
    {
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_searchAIState;
        m_aiGunnerOrCombat.m_mainDestinationPoint = m_trackCharacterPosition;
        m_aiGunnerOrCombat.m_navMeshPath = m_aiGunnerOrCombat.CalculateNavmeshPath(m_aiGunnerOrCombat.m_mainDestinationPoint);
        m_aiGunnerOrCombat.m_isInCover = false;
        m_aiGunnerOrCombat.Crouch(false);
        m_aiGunnerOrCombat.m_animator.SetBool("IsCover", false);
        ResetCoverState();
    }

    private void ResetCoverState()
    {
        m_coverTimer = 0f;
        m_aiGunnerOrCombat.Crouch(false);
        m_aiGunnerOrCombat.m_isInCover = false;
        m_aiGunnerOrCombat.m_animator.SetBool("IsCover", false);
        m_coverTime = UnityEngine.Random.Range(m_aiGunnerOrCombat.m_AIField.ChangeCoverPositionTime.MinValue, m_aiGunnerOrCombat.m_AIField.ChangeCoverPositionTime.MaxValue);
        m_aiGunnerOrCombat.m_animator.SetFloat("Strafe", 0f);
    }
}
