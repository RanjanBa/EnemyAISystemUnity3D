using AIManager_namespace;
using UnityEngine;

public class AIChaseState<T> : IAIStateManager where T : AIManager
{
    private T m_aiGunnerOrCombat;
    private float m_sqrOfAttackingRange;

    // For Calculating navmeshPath after some frames
    private int m_frameCount = 0;
    private int m_frameForCalculatingNavmesh = 30;

    public AIChaseState(T aiManager, float sqrRange)
    {
        m_aiGunnerOrCombat = aiManager;
        m_sqrOfAttackingRange = sqrRange;
    }

    public void OnStateEnter()
    {
        Debug.Log("Enter Chase state...");
        m_aiGunnerOrCombat.m_currentAnimationState = AnimationState.RunningAnimation;
    }

    public void UpdateCurrentState()
    {
        Debug.Log("Chase State");
        m_frameCount++;
        UpdateChaseState();
        ChangeStateConditions();
    }

    private void UpdateChaseState()
    {
        m_aiGunnerOrCombat.CheckEveryCharacterForInVisualRange();
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
        else
        {
            ChangeToGunFireState();
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

    public void ChangeToPatrolState()
    {
        throw new System.NotImplementedException();
    }

    public void ChangeToCoverState()
    {
        throw new System.NotImplementedException();
    }

    public void ChangeToGunFireState()
    {
        m_aiGunnerOrCombat.ChangeAIState(m_aiGunnerOrCombat.m_gunFireAIState);
    }

    public void ChangeToBoxingState()
    {
        m_aiGunnerOrCombat.StartCoroutine(m_aiGunnerOrCombat.ChangeAnimationLayer(1, 0));
        m_aiGunnerOrCombat.ChangeAIState(m_aiGunnerOrCombat.m_boxingAIState);
    }

    public void ChangeToSearchState()
    {
        m_aiGunnerOrCombat.m_searchAIState.m_offsetPosition = m_aiGunnerOrCombat.m_mainDestinationPoint;
        m_aiGunnerOrCombat.m_searchAIState.m_invetigate_searchDirection = m_aiGunnerOrCombat.m_mainDestinationPoint - m_aiGunnerOrCombat.transform.position;
        if (Mathf.Abs(Vector3.SqrMagnitude(m_aiGunnerOrCombat.m_searchAIState.m_invetigate_searchDirection)) <= 0.1f)
        {
            m_aiGunnerOrCombat.m_searchAIState.m_invetigate_searchDirection = m_aiGunnerOrCombat.transform.forward;
        }

        //m_aiGunnerOrCombat.m_searchAIState.m_invetigate_searchDirection.y = 0f;

        m_aiGunnerOrCombat.ChangeAIState(m_aiGunnerOrCombat.m_searchAIState);
    }

    public void OnStateExit()
    {
        Debug.Log("Exit Chase State...");
    }
}
