using AIManager_namespace;
using UnityEngine;

public class AIChaseState<T> : IAIStateManager where T : AIManager
{
    private T m_aiGunnerOrCombat;
    private float m_sqrOfAttackingRange;

    private int m_frameCount = 0;
    private int m_frameForCalculatingNavmesh = 30;

    public AIChaseState(T aiManager, float sqrRange)
    {
        m_aiGunnerOrCombat = aiManager;
        m_sqrOfAttackingRange = sqrRange;
    }

    public void UpdateCurrentState()
    {
        Debug.Log("Chase State " + m_aiGunnerOrCombat.m_canMove);
        m_frameCount++;
        UpdateChaseState();
        ChangeStateConditions();
    }

    private void UpdateChaseState()
    {
        m_aiGunnerOrCombat.m_currentAnimationState = AnimationState.RunningAnimation;

        m_aiGunnerOrCombat.CheckEveryCharacter();
        m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter = m_aiGunnerOrCombat.FindNearestOpponentGameObjectWithType();

        if (m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject != null)
        {
            m_aiGunnerOrCombat.m_mainDestinationPoint = m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject.transform.position;
        }

        if (m_frameCount >= m_frameForCalculatingNavmesh)
        {
            m_aiGunnerOrCombat.m_navMeshPath = m_aiGunnerOrCombat.CalculateNavmeshPath(m_aiGunnerOrCombat.m_mainDestinationPoint);
            m_frameCount = 0;
        }
        m_aiGunnerOrCombat.FollowAlongNavMeshPath(m_aiGunnerOrCombat.m_navMeshPath, Vector3.zero);
    }

    private void ChangeStateConditions()
    {
        if(m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject == null)
        {
            ChangeToSearchState();
            return;
        }

        if (m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyBoxer || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerBoxerCampanion)
        {
            float sqrDist = Vector3.SqrMagnitude(m_aiGunnerOrCombat.transform.position - m_aiGunnerOrCombat.m_mainDestinationPoint);
            if (sqrDist <= m_sqrOfAttackingRange)
            {
                ChangeToBoxingState();
                return;
            }
        }
    }

    public void ChangeToChaseState()
    {
        throw new System.NotImplementedException();
    }

    public void ChangeToUnwareState()
    {
        throw new System.NotImplementedException();
    }

    public void ChangeToInvestigateState()
    {
        throw new System.NotImplementedException();
    }

    public void ChangeToPatrolState()
    {
        throw new System.NotImplementedException();
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
            return;
        }

        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_coverAIState;
    }

    public void ChangeToGunFireState()
    {
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_gunFireAIState;
        m_aiGunnerOrCombat.m_isInCover = false;
        m_aiGunnerOrCombat.m_animator.SetBool("IsCover", m_aiGunnerOrCombat.m_isInCover);
    }

    public void ChangeToBoxingState()
    {
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_boxingAIState;
        m_aiGunnerOrCombat.StartCoroutine(m_aiGunnerOrCombat.ChangeAnimationLayer(1, 0));
    }

    public void ChangeToSearchState()
    {
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_searchAIState;
        m_aiGunnerOrCombat.m_offsetPosition = m_aiGunnerOrCombat.m_mainDestinationPoint;
        m_aiGunnerOrCombat.m_invetigate_searchDirection = m_aiGunnerOrCombat.m_mainDestinationPoint - m_aiGunnerOrCombat.transform.position;
        if (Mathf.Abs(Vector3.SqrMagnitude(m_aiGunnerOrCombat.m_invetigate_searchDirection)) <= 0.1f)
        {
            m_aiGunnerOrCombat.m_invetigate_searchDirection = m_aiGunnerOrCombat.transform.forward;
        }
        m_aiGunnerOrCombat.m_invetigate_searchDirection.y = 0f;
    }
}
