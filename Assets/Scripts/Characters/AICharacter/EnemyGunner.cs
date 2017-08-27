using AIManager_namespace;
using UnityEngine;

public class EnemyGunner : AIGunnerManager
{
    private void Start()
    {
        Initialized();
        m_CharType = CharacterType.EnemyGunner;
    }

    protected override void Initialized()
    {
        base.Initialized();
        m_sceneCharactersManager.AddEnemyToCharaterList(this.gameObject, CharacterType.EnemyGunner);
    }

    private void Update()
    {
        m_isGrounded = CheckGroudedOrNot();
        m_animator.SetBool("IsGrounded", m_isGrounded);

        if (m_isGrounded)
        {
            if (m_currentAIState != null)
            {
                m_currentAIState.UpdateCurrentState();
            }
            else
            {
                Debug.LogError("m_currentAIState is null");
            }
        }

        InWhatState();
    }

    private void InWhatState()
    {
        if (m_currentAIState == m_unwareAIState)
        {
            AIIndex = 0;
        }
        else if (m_currentAIState == m_investigateAIState )
        {
            AIIndex = 1;
        }
        else if (m_currentAIState == m_patrolAIState)
        {
            AIIndex = 2;
        }
        else if (m_currentAIState == m_searchAIState)
        {
            AIIndex = 3;
        }
        else if (m_currentAIState == m_chaseAIState)
        {
            AIIndex = 4;
        }
        else if (m_currentAIState == m_coverAIState)
        {
            AIIndex = 5;
        }
        else if (m_currentAIState == m_gunFireAIState)
        {
            AIIndex = 6;
        }
    }

    private void FixedUpdate()
    {
        //if (m_canMove)
        //{
        //    for (int i = 0; i < 3; i++)
        //    {
        //        Vector3 pos = transform.position + (0.2f + i * 0.3f) * transform.up;
        //        RaycastHit hitInfo;
        //        if (Physics.Raycast(pos, transform.forward, out hitInfo, 1f))
        //        {
        //            if (!hitInfo.collider.transform.root.gameObject.isStatic)
        //            {
        //                return;
        //            }

        //            StartCoroutine(FrozeMovementForWaitTimer(2f));
        //            CalculatePath(m_destinationPosition);
        //            if (m_navMeshPath.status == UnityEngine.AI.NavMeshPathStatus.PathPartial || m_navMeshPath.status == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
        //            {
        //                m_destinationPosition = GetNextDestinationPointFromPath();
        //            }
        //            break;
        //        }
        //    }
        //}
    }
}
