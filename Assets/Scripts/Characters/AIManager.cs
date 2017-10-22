using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

namespace AIManager_namespace
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class AIManager : CharacterManager
    {
        [HideInInspector]
        public int AIIndex = 0;
        [HideInInspector]
        public string[] AIStates = new string[7] { "UnwareState", "InvestigateState", "PatrolState", "SearchState", "ChaseState", "CoverState", "CorrespondingActionState" };
        public bool DebugRayOfEyeRange = false;

        #region Customizable Public Variables
        [Tooltip("Layer mask for checking visibility of the characters in range")]
        public LayerMask m_CheckVisibilityLayerMask;
        public float m_SpeedOfCharacter = 2.8f;
        public float m_RotationSpeed = 1f;
        [Tooltip("For changing next destination point")]
        [Range(0, 2f)]
        public float m_ThresholdDistance = 0.2f;
        [Tooltip("AI variables")]
        public AIField m_AIField;
        public float m_coolDownRate = 1f;
        public float m_AIVisibleTime = 3f;
        public Path m_FollowPath;
        #endregion Customizable Public Variables

        #region HideInInspector Public Variables
        //[HideInInspector]
        public AnimationState m_currentAnimationState;
        //[HideInInspector]
        public float m_unwareTime = 1f;
        //[HideInInspector]
        public float m_patrolTime = 10f;
        //[HideInInspector]
        public CharacterTypeOfGameObject m_nearestOpponentVisibleCharacter;
        //[HideInInspector]
        public int m_deltaChangeInHealth { get; private set; }
        [HideInInspector]
        public Vector3 m_invetigate_searchDirection;
        [HideInInspector]
        public bool m_canMove;
        [HideInInspector]
        public bool m_canIHearSomething = false;        
        [HideInInspector]
        public NavMeshPath m_navMeshPath;
        [HideInInspector]
        public Vector3 m_mainDestinationPoint;
        [HideInInspector]
        public Vector3 m_currentDestinationPoint;
        [HideInInspector]
        public int m_currentIndexOfCalculatedNavmeshPath = 0;
        [HideInInspector]
        public int m_currentIndexOfGivenPath = 0;
        [HideInInspector]
        public Vector3 m_offsetPosition;
        //[HideInInspector]
        public CoverPosition m_coverPositionScript;
        //[HideInInspector]
        public float m_coverFireTime = 5f;
        //[HideInInspector]
        public float m_coverFireTimer = 0f;
        //[HideInInspector]
        public List<CharacterTypeAndTimerOfGameObject> m_charWithTypeAndTimer = new List<CharacterTypeAndTimerOfGameObject>();
        //[HideInInspector]
        public List<CharacterTypeOfGameObject> m_visibledCharWithType = new List<CharacterTypeOfGameObject>();
        #endregion HideInInspector Public Variables

        #region Uncustomizable Public Variables
        public IAIStateManager m_currentAIState;
        public AIUnwareState<AIManager> m_unwareAIState;
        public AIInvestigateState<AIManager> m_investigateAIState;
        public AIPatrolState<AIManager> m_patrolAIState;
        public AIChaseState<AIManager> m_chaseAIState;
        public AISearchState<AIManager> m_searchAIState;
        public AICoverState<AIManager> m_coverAIState;
        public AIGunFireState m_gunFireAIState;
        public AIBoxingState m_boxingAIState;
        #endregion Uncustomizable Public Variables

        #region Private Variables
        private CoverFinder m_coverFinder;
        private EyeVisualPerception m_eyeVisualPerception;
        private NavMeshAgent m_navMeshAgent;
        #endregion Private Variables

        protected override void Initialized()
        {
            base.Initialized();
            m_eyeVisualPerception = GetComponentInChildren<EyeVisualPerception>();
            if(m_eyeVisualPerception == null)
            {
                Debug.LogError("There is no eye of the character");
            }
            m_coverFinder = GetComponentInChildren<CoverFinder>();
            m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_navMeshPath = new NavMeshPath();
            m_animator.applyRootMotion = false;
            m_currentIndexOfGivenPath = 0;
            m_mainDestinationPoint = m_FollowPath.pathTransformPosition[m_currentIndexOfGivenPath].position;
            transform.position = m_mainDestinationPoint;
            m_currentDestinationPoint = m_mainDestinationPoint;
            m_navMeshPath = CalculateNavmeshPath(m_mainDestinationPoint);
            m_offsetPosition = m_mainDestinationPoint;
            m_unwareTime = Random.Range(m_AIField.m_MinMaxUnwareTime.MinValue, m_AIField.m_MinMaxUnwareTime.MaxValue);
            m_patrolTime = Random.Range(m_AIField.m_MinMaxPatrolTime.MinValue, m_AIField.m_MinMaxPatrolTime.MaxValue);
        }

        public float GetSearchInvestigateAngle(int index)
        {
            if(index >= m_AIField.SearchOrInvetigateRegions.Length)
            {
                Debug.LogWarning("index of search or investigate angle is out of range...");
                index -= 1;
            }

            return m_AIField.SearchOrInvetigateRegions[index];
        }

        public void Crouch(bool isCrouch)
        {
            m_isCrouching = isCrouch;
            if (isCrouch)
            {
                m_charController.height = m_charHeight / 2f;
                m_charController.center = new Vector3(0f, m_charController.height / 2f, 0f);
            }
            else
            {
                m_charController.height = m_charHeight;
                m_charController.center = new Vector3(0f, m_charController.height / 2f, 0f);
            }
            m_animator.SetBool("IsCrouch", isCrouch);
        }

        public void FollowAlongNavMeshPath(NavMeshPath navmeshPath, Vector3 rotationTowards, bool manuallyControlRotation = false)
        {
            float sqrDistance = Vector3.SqrMagnitude(transform.position - m_currentDestinationPoint);

            if (sqrDistance < m_ThresholdDistance)
            {
                m_currentDestinationPoint = GetNextPointFromCalculatedNavmeshPath(navmeshPath);
            }

            Vector3 directionToDestPoint = m_currentDestinationPoint - transform.position;

            if (directionToDestPoint.sqrMagnitude > 1f)
            {
                directionToDestPoint.Normalize();
            }

            Vector3 axisSignDest = Vector3.Cross(transform.forward, directionToDestPoint);
            float angleToDest = Vector3.Angle(transform.forward, directionToDestPoint) * (axisSignDest.y >= 0 ? 1f : -1f);

            if (manuallyControlRotation == true && m_nearestOpponentVisibleCharacter.characterGameObject != null)
            {
                Vector3 directionToOpponenet = rotationTowards - transform.position;
                Vector3 axisSignOppo = Vector3.Cross(transform.forward, directionToOpponenet);
                float angleToOppo = Vector3.Angle(transform.forward, directionToOpponenet) * (axisSignOppo.y >= 0 ? 1f : -1f);

                Debug.DrawRay(transform.position + Vector3.up, directionToOpponenet, Color.black);
                Debug.DrawRay(transform.position + Vector3.up * 0.5f, directionToDestPoint * 10f);

                ControlMovement(directionToDestPoint, angleToOppo);
                ControlRotation(directionToOpponenet);
                if (Mathf.Abs(angleToOppo) < 30f && Vector3.SqrMagnitude(m_mainDestinationPoint - transform.position) > 2f * m_ThresholdDistance)
                {
                    ControlRotationAnimation(angleToDest);
                }
                else
                {
                    ControlRotationAnimation(angleToOppo);
                }
            }
            else {
                ControlMovement(directionToDestPoint, angleToDest);
                ControlRotation(directionToDestPoint);
                ControlRotationAnimation(angleToDest);
            }
        }

        public void ControlMovement(Vector3 moveDirection, float thesholdAngle)
        {
            float speed = 0f;

            if (m_currentAnimationState == AnimationState.WalkingAnimation)
            {
                speed = 0.5f;
            }
            else if (m_currentAnimationState == AnimationState.RunningAnimation)
            {
                speed = 1f;
            }

            if(m_canMove == false)
            {
                speed = 0f;
            }

            if (Mathf.Abs(thesholdAngle) < 30f)
            {
                ControlMovementAnimation(speed);

                if (m_currentAIState == m_boxingAIState)
                {
                    m_charController.Move(moveDirection * Time.deltaTime * m_SpeedOfCharacter * 0.5f * m_animator.GetFloat("Speed"));
                }
                else
                {
                    m_charController.Move(moveDirection * Time.deltaTime * m_SpeedOfCharacter * m_animator.GetFloat("Speed"));
                }                
            }
            else
            {
                ControlMovementAnimation(0f);
            }
        }

        public void ControlRotation(Vector3 directionOfRotation)
        {
            if (Vector3.SqrMagnitude(directionOfRotation) >= 0.01f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(directionOfRotation), Time.deltaTime * m_RotationSpeed);
            }
        }

        private void ControlMovementAnimation(float speed)
        {
            float speed_m = m_animator.GetFloat("Speed");
            if(speed_m > speed)
            {
                speed_m -= Time.deltaTime;
            }
            else if(speed_m < speed)
            {
                speed_m += Time.deltaTime;
            }

            speed_m = Mathf.Clamp01(speed_m);

            m_animator.SetFloat("Speed", speed_m);
        }        

        public void ControlRotationAnimation(float angle)
        {
            m_animator.SetFloat("Turn", angle);
        }

        public IEnumerator ChangeAnimationLayer(int increasingLayerIndex, int decreasingLayerIndex)
        {
            m_canMove = false;

            while (true)
            {
                float layerWeight = m_animator.GetLayerWeight(increasingLayerIndex);
                layerWeight += Time.deltaTime;
                layerWeight = Mathf.Clamp01(layerWeight);
                m_animator.SetLayerWeight(increasingLayerIndex, layerWeight);

                if (layerWeight >= 1f)
                {
                    break;
                }
                yield return null;
            }

            while (true)
            {
                float layerWeight = m_animator.GetLayerWeight(decreasingLayerIndex);
                layerWeight -= Time.deltaTime;
                layerWeight = Mathf.Clamp01(layerWeight);
                m_animator.SetLayerWeight(decreasingLayerIndex, layerWeight);

                if (layerWeight <= 0f)
                {
                    break;
                }
                yield return null;
            }

            m_canMove = true;
            yield return null;
        }

        public NavMeshPath CalculateNavmeshPath(Vector3 targetPos)
        {
            NavMeshPath path = new NavMeshPath();
            m_currentIndexOfCalculatedNavmeshPath = 0;
            if (m_navMeshAgent.CalculatePath(targetPos, path))
            {
                m_currentDestinationPoint = transform.position;
                return path;
            }

            return null;          
        }

        public Vector3 GetNextDestinationPointFromGivenPath()
        {
            m_currentIndexOfGivenPath = (m_currentIndexOfGivenPath + 1) % m_FollowPath.pathTransformPosition.Length;
            Vector3 nextPoint = m_FollowPath.pathTransformPosition[m_currentIndexOfGivenPath].position;
            return nextPoint;
        }

        public bool GetRandomPointInNavmesh(Vector3 centerTarget, float range, out Vector3 outDestinationPoint)
        {
            outDestinationPoint = Vector3.zero;

            NavMeshHit navMeshHit;

            if(NavMesh.SamplePosition(centerTarget, out navMeshHit, range, NavMesh.AllAreas))
            {
                outDestinationPoint = navMeshHit.position;
                return true;
            }

            return false;
        }

        public CharacterTypeOfGameObject FindNearestOpponentGameObjectWithType()
        {
            float shortestSqrDist = float.MaxValue;
            CharacterTypeOfGameObject nearestChar = new CharacterTypeOfGameObject();

            foreach (CharacterTypeOfGameObject item in m_visibledCharWithType)
            {
                if (m_CharType == CharacterType.EnemyBoxer || m_CharType == CharacterType.EnemyGunner)
                {
                    if (item.TypeOfCharacter == CharacterType.Player || item.TypeOfCharacter == CharacterType.PlayerBoxerCampanion || item.TypeOfCharacter == CharacterType.PlayerGunnerCampanion)
                    {
                        if (nearestChar.characterGameObject == null)
                        {
                            nearestChar = item;
                            shortestSqrDist = Vector3.SqrMagnitude(transform.position - nearestChar.characterGameObject.transform.position);
                            continue;
                        }
                        else
                        {
                            float sqrDist = Vector3.SqrMagnitude(transform.position - item.characterGameObject.transform.position);

                            if (sqrDist < shortestSqrDist)
                            {
                                shortestSqrDist = sqrDist;
                                nearestChar = item;
                            }
                        }
                    }
                }
                else if (m_CharType == CharacterType.PlayerBoxerCampanion || m_CharType == CharacterType.PlayerGunnerCampanion)
                {
                    if (item.TypeOfCharacter == CharacterType.EnemyBoxer || item.TypeOfCharacter == CharacterType.EnemyGunner)
                    {
                        if (nearestChar.characterGameObject == null)
                        {
                            nearestChar = item;
                            shortestSqrDist = Vector3.SqrMagnitude(transform.position - nearestChar.characterGameObject.transform.position);
                            continue;
                        }
                        else
                        {
                            float sqrDist = Vector3.SqrMagnitude(transform.position - item.characterGameObject.transform.position);

                            if (sqrDist < shortestSqrDist)
                            {
                                shortestSqrDist = sqrDist;
                                nearestChar = item;
                            }
                        }
                    }
                }
            }

            return nearestChar;
        }

        public bool CanFindBestCoverPosition(ref CoverPosition coverPositionScript)
        {
            if (m_coverFinder == null || m_coverFinder.m_coverPositionScripts.Count < 1 || m_nearestOpponentVisibleCharacter.characterGameObject == null)
            {
                coverPositionScript = null;
                return false;                
            }
            bool canFindCover = false;
            Vector3 opponentPosition = m_nearestOpponentVisibleCharacter.characterGameObject.transform.position;
            float shortestSqrDistToCoverPos = float.MaxValue;

            foreach (CoverPosition item in m_coverFinder.m_coverPositionScripts)
            {
                if (item.m_isOccupied)
                {
                    continue;
                }

                Vector3 dirToOpponent = opponentPosition - item.transform.position;
                Debug.DrawRay(item.transform.position, dirToOpponent, Color.white, 10f);
                RaycastHit hitInfo;
                if(Physics.Raycast(item.transform.position + Vector3.up * 0.5f, dirToOpponent, out hitInfo, 3f, m_EnvironmentLayerForCover, QueryTriggerInteraction.Ignore))
                {
                    if (hitInfo.collider.gameObject.isStatic)
                    {
                        float sqrDist = Vector3.SqrMagnitude(transform.position - item.transform.position);
                        float sqrDistFromPointToOpponent = dirToOpponent.sqrMagnitude;

                        if (sqrDist < shortestSqrDistToCoverPos && sqrDistFromPointToOpponent <= m_AIField.ViewRange * m_AIField.ViewRange)
                        {
                            shortestSqrDistToCoverPos = sqrDist;
                            coverPositionScript = item;
                            canFindCover = true;
                        }
                    }
                }
            }

            if(canFindCover == false)
            {
                coverPositionScript = null;
            }

            return canFindCover;
        }

        public void HearingPerception(Vector3 noisePosition, float range, string tag)
        {
            m_canIHearSomething = false;
            Vector3 dir = noisePosition - transform.position;
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, dir, out hitInfo, range))
            {
                if (hitInfo.collider.CompareTag(tag))
                {
                    m_canIHearSomething = true;
                    Debug.Log("I can straight hear you..");
                }
            }
            else
            {
                NavMeshPath path = new NavMeshPath();

                if (m_navMeshAgent.CalculatePath(noisePosition, path))
                {
                    float length = 0f;

                    for (int i = 0; i < path.corners.Length - 1; i++)
                    {
                        length = Vector3.Magnitude(path.corners[i] - path.corners[i + 1]);
                        Debug.DrawRay(path.corners[i], Vector3.up * 10, Color.red);

                        if (i == path.corners.Length - 2)
                        {
                            Debug.DrawRay(path.corners[i + 1], Vector3.up * 10, Color.red);
                        }
                    }

                    if (length <= range)
                    {                       
                        m_canIHearSomething = true;
                        Debug.Log("I can hear you..");
                    }
                }
            }

            if (m_canIHearSomething)
            {
                m_offsetPosition = noisePosition;
                if(m_currentAIState == m_investigateAIState)
                {
                    Debug.LogError("YOU HAVE TO IMPLEMENT...");
                }
            }
        }

        public void CheckEveryCharacter()
        {
            foreach (CharacterTypeAndTimerOfGameObject item in m_charWithTypeAndTimer)
            {
                if (item.CharacterGameObjectWithType.TypeOfCharacter == CharacterType.Player)
                {
                    VisualPerception(item, "Player");
                }
                else if (item.CharacterGameObjectWithType.TypeOfCharacter == CharacterType.PlayerBoxerCampanion || item.CharacterGameObjectWithType.TypeOfCharacter == CharacterType.PlayerGunnerCampanion)
                {
                    VisualPerception(item, "PlayerCampanion"); ;
                }
                else if (item.CharacterGameObjectWithType.TypeOfCharacter == CharacterType.EnemyBoxer || item.CharacterGameObjectWithType.TypeOfCharacter == CharacterType.EnemyGunner)
                {
                    VisualPerception(item, "Enemy");
                }
            }
        }

        private void VisualPerception(CharacterTypeAndTimerOfGameObject item, string tag)
        {
            Vector3 dir = item.CharacterGameObjectWithType.characterGameObject.transform.position - transform.position;

            bool isVisible = CheckVisibility(dir, tag);

            if (isVisible)
            {
                if (item.VisibilityTimer >= m_AIVisibleTime)
                {
                    item.VisibilityTimer = m_AIVisibleTime;
                    return;
                }

                float vt = item.IncrementOrDecrementTimer(Time.deltaTime);

                if (m_visibledCharWithType.Contains(item.CharacterGameObjectWithType))
                {
                    return;
                }

                if (vt >= m_AIVisibleTime || dir.sqrMagnitude <= m_AIField.ThresoldViewRange * m_AIField.ThresoldViewRange)
                {
                    item.VisibilityTimer = m_AIVisibleTime;
                    m_visibledCharWithType.Add(item.CharacterGameObjectWithType);
                }
            }
            else
            {
                if (item.VisibilityTimer <= 0)
                {
                    item.VisibilityTimer = 0f;
                    return;
                }

                float vt = item.IncrementOrDecrementTimer(-Time.deltaTime);

                if (vt <= 0f)
                {
                    CharacterTypeOfGameObject chType = item.CharacterGameObjectWithType;
                    m_visibledCharWithType.Remove(chType);
                }
            }
        }

        private bool CheckVisibility(Vector3 dir, string tagOfGm)
        {
            float angle = Vector3.Angle(dir, transform.forward);
            if (angle <= m_AIField.ViewAngle)
            {
                RaycastHit hitInfo;
                for (int i = 0; i < 3; i++)
                {
                    if (Physics.Raycast(m_eyeVisualPerception.transform.position, dir - Vector3.up * i/3f, out hitInfo, m_AIField.ViewRange, m_CheckVisibilityLayerMask, QueryTriggerInteraction.Ignore))
                    {
                        if (hitInfo.transform.CompareTag(tagOfGm))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private Vector3 GetNextPointFromCalculatedNavmeshPath(NavMeshPath navmeshPath)
        {
            Vector3 nextDestination = transform.position;
            int pathIndex = navmeshPath.corners.Length;
            if (pathIndex >= 2 && navmeshPath.status != NavMeshPathStatus.PathInvalid)
            {
                m_currentIndexOfCalculatedNavmeshPath = m_currentIndexOfCalculatedNavmeshPath + 1;
                if(m_currentIndexOfCalculatedNavmeshPath <= navmeshPath.corners.Length - 1)
                {
                    nextDestination = navmeshPath.corners[m_currentIndexOfCalculatedNavmeshPath];
                }                
            }

            return nextDestination;
        }

        public override void TakeDamage(int damage, DamageType damageType)
        {
            if(damageType == DamageType.DamageByGun)
            {
                m_deltaChangeInHealth += damage;
            }
        }

        public virtual void EquipWithWeapon()
        {
            Debug.Log("Base EquipWeapon Class...");
        }

        public virtual void UnEquipWeapon()
        {
            Debug.Log("Base UnEquipWeapon Class...");
        }

        public void ResetDeltaChangeInHealth()
        {
            m_deltaChangeInHealth = 0;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_currentDestinationPoint, 0.25f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(m_mainDestinationPoint, Vector3.one * 2f);
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(m_offsetPosition, 0.5f);

            Gizmos.color = Color.red;
            if (m_navMeshPath != null)
            {
                for (int i = 1; i < m_navMeshPath.corners.Length; i++)
                {
                    Gizmos.DrawLine(m_navMeshPath.corners[i], m_navMeshPath.corners[i - 1]);
                    Gizmos.DrawRay(m_navMeshPath.corners[i - 1], Vector3.up * 2f);
                    if (i == m_navMeshPath.corners.Length - 1)
                    {
                        Gizmos.DrawRay(m_navMeshPath.corners[i], Vector3.up * 2f);
                    }
                }
            }

            if (m_nearestOpponentVisibleCharacter.characterGameObject != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(m_nearestOpponentVisibleCharacter.characterGameObject.transform.position, Vector3.one);
            }

            Gizmos.color = Color.white;
            if (m_coverPositionScript)
            {
                Gizmos.DrawCube(m_coverPositionScript.transform.position, Vector3.one);
            }
        }
    }
}
