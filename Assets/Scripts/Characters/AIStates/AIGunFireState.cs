using System;
using UnityEngine;
using System.Collections.Generic;
using AIManager_namespace;

public class AIGunFireState : IAIStateManager
{
    private AIGunnerManager m_aiGunner;
    private Vector3 m_trackCharacterPosition = Vector3.zero;
    private float m_firingTimer = 0f;
    private float m_timerForStateChange = 0f;
    private float m_sqrDistanceFromMainDest = float.MaxValue;

    private float m_timer = 0f;

    public AIGunFireState(AIGunnerManager aiGunner)
    {
        m_aiGunner = aiGunner;
    }

    public void OnStateEnter()
    {
        Debug.Log("Enter GunFire State...");
        m_aiGunner.m_AIVisibleTime = 0.1f;
        m_aiGunner.m_currentAnimationState = AnimationState.WalkingAnimation;
        m_aiGunner.UpdateStafeLocomotion(1f);
    }

    public void UpdateCurrentState()
    {
        //Debug.Log(m_aiGunner.gameObject.ToString() + " is in GunFire State...");
        
        UpdateGunFireState();
        ChangeStateConditions();
    }

    private void UpdateGunFireState()
    {
        m_firingTimer += Time.deltaTime;
        m_timerForStateChange += Time.deltaTime;

        m_aiGunner.CheckEveryCharacterForInVisualRange();
        m_aiGunner.m_nearestOpponentVisibleCharacter = m_aiGunner.FindNearestOpponentGameObjectWithType();

        if (m_aiGunner.m_nearestOpponentVisibleCharacter.characterGameObject != null)
        {
            m_trackCharacterPosition = m_aiGunner.m_nearestOpponentVisibleCharacter.characterGameObject.transform.position;
        }

        if (m_aiGunner.m_coverPositionScript != null)
        {
            Vector3 fromAgentToOpponentDirection = m_trackCharacterPosition - m_aiGunner.transform.position;
            Debug.DrawRay(m_aiGunner.m_coverPositionScript.transform.position + Vector3.up * 0.5f, fromAgentToOpponentDirection.normalized * 3f, Color.magenta, 3f);
            if (!Physics.Raycast(m_aiGunner.m_coverPositionScript.transform.position + Vector3.up * 0.5f, fromAgentToOpponentDirection, 3f, m_aiGunner.m_EnvironmentLayerForCover, QueryTriggerInteraction.Ignore))
            {
                m_aiGunner.Crouch(false);
                ChangeToCoverState();
            }
            else
            {
                Debug.Log("Does hit the cover layerMask");

                if (m_aiGunner.m_navMeshPath.corners.Length <= 2)
                {
                    m_aiGunner.m_currentDestinationPoint = m_aiGunner.m_mainDestinationPoint;
                }
                else if (m_aiGunner.m_currentIndexOfCalculatedNavmeshPath >= m_aiGunner.m_navMeshPath.corners.Length - 1)
                {
                    m_aiGunner.m_currentIndexOfCalculatedNavmeshPath = m_aiGunner.m_navMeshPath.corners.Length - 1;
                    m_aiGunner.m_currentDestinationPoint = m_aiGunner.m_navMeshPath.corners[m_aiGunner.m_currentIndexOfCalculatedNavmeshPath];
                }
            }
        }
        else
        {
            m_aiGunner.m_mainDestinationPoint = m_aiGunner.transform.position;
        }

        if (m_firingTimer >= m_aiGunner.m_CurrentlyActiveGun.m_GunFireRate && m_aiGunner.m_CurrentlyActiveGun.M_RemainingAmmunition > 0)
        {
            m_aiGunner.Fire(m_trackCharacterPosition);
            m_firingTimer = 0f;
        }
        else if (m_aiGunner.m_CurrentlyActiveGun.M_RemainingAmmunition <= 0 && m_aiGunner.m_IsReloading == false)
        {
            m_aiGunner.Reload();
        }

        m_sqrDistanceFromMainDest = Vector3.SqrMagnitude(m_aiGunner.m_mainDestinationPoint - m_aiGunner.transform.position);
        if (m_sqrDistanceFromMainDest <= 2f * m_aiGunner.m_ThresholdDistance)
        {
            m_aiGunner.m_currentAnimationState = AnimationState.IdleAnimation;
        }
        else
        {
            m_aiGunner.m_currentAnimationState = AnimationState.RunningAnimation;
        }

        m_aiGunner.FollowAlongNavMeshPath(m_aiGunner.m_navMeshPath, m_trackCharacterPosition, true);
        Debug.DrawRay(m_trackCharacterPosition, Vector3.up * 30f, Color.magenta);
    }

    private void ChangeStateConditions()
    {
        if(m_aiGunner.m_deltaChangeInHealth > 30)
        {
            ChangeToCoverState();
            m_aiGunner.ResetDeltaChangeInHealth();
            return;
        }

        m_timer += Time.deltaTime;
        if (m_timer > 5f)
        {
            m_timer = 0f;
            if (m_aiGunner.m_nearestOpponentVisibleCharacter.characterGameObject == null)
            {
                ChangeToSearchState();
                return;
            }
        }
    }

    public void ChangeToGunFireState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToChaseState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToBoxingState()
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

    public void ChangeToSearchState()
    {
        m_aiGunner.m_mainDestinationPoint = m_trackCharacterPosition;
        m_aiGunner.m_searchAIState.m_offsetPosition = m_aiGunner.m_mainDestinationPoint;
        m_aiGunner.m_navMeshPath = m_aiGunner.CalculateNavmeshPath(m_aiGunner.m_mainDestinationPoint);
        m_aiGunner.m_searchAIState.m_invetigate_searchDirection = m_aiGunner.m_mainDestinationPoint - m_aiGunner.transform.position;
        if (Mathf.Abs(Vector3.SqrMagnitude(m_aiGunner.m_searchAIState.m_invetigate_searchDirection)) <= 0.1f)
        {
            m_aiGunner.m_searchAIState.m_invetigate_searchDirection = m_aiGunner.transform.forward;
        }

        //m_aiGunner.m_searchAIState.m_invetigate_searchDirection.y = 0f;

        m_aiGunner.ChangeAIState(m_aiGunner.m_searchAIState);
    }

    public void ChangeToCoverState()
    {
        if (m_aiGunner.CanFindBestCoverPosition(ref m_aiGunner.m_coverPositionScript))
        {
            m_aiGunner.m_coverPositionScript.m_isOccupied = true;
            m_aiGunner.m_mainDestinationPoint = m_aiGunner.m_coverPositionScript.transform.position;
            m_aiGunner.m_navMeshPath = m_aiGunner.CalculateNavmeshPath(m_aiGunner.m_mainDestinationPoint);
            Debug.Log("I can find the cover Position");
        }
        else
        {
            Debug.Log("Can't find the cover Position");
            m_aiGunner.m_navMeshPath.ClearCorners();
            m_aiGunner.m_mainDestinationPoint = m_aiGunner.transform.position;
            return;
        }

        m_aiGunner.ChangeAIState(m_aiGunner.m_coverAIState);
    }

    public void OnStateExit()
    {
        ResetGunFireState();
        m_aiGunner.UpdateStafeLocomotion(0f);
    }

    public void EquipWeapon()
    {
        m_aiGunner.EquipWithWeapon();
    }

    public void UnEquipWeapon()
    {
        m_aiGunner.UnEquipWeapon();
    }

    private void ResetGunFireState()
    {
        m_firingTimer = 0f;
        m_timerForStateChange = 0f;
    }
}
