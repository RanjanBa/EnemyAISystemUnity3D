using UnityEngine;
using System.Collections;
using gunClass_namespace;

namespace AIManager_namespace
{
    public abstract class AIGunnerManager : AIManager
    {
        public AIGunnerField m_AIGunnerField;
        [Tooltip("For keeping the Hand gun during hostler")]
        public Transform m_HandGunHoldingPosition;
        [Tooltip("For keeping the Assault Rifle during hostler")]
        public Transform m_AssaultRifleHoldingPosition;
        [Tooltip("Equip hand gun position")]
        public Transform m_EquipingHandGunPosition;
        [Tooltip("Equip Assult Rifle position")]
        public Transform m_EquipingAssultRiflePosition;

        //[HideInInspector]
        public bool m_isReloading = false;
        //[HideInInspector]
        public GunClass[] m_gun;
        //[HideInInspector]
        public GunClass m_activeGun;

        protected bool m_isEquippedWithWeapon = false;

        protected override void Initialized()
        {
            base.Initialized();

            m_gun = GetComponentsInChildren<GunClass>();

            m_activeGun = m_gun[Random.Range(0, m_gun.Length)];

            float sqrRange = m_AIGunnerField.AttackbyGunRange * m_AIGunnerField.AttackbyGunRange;

            m_unwareAIState = new AIUnwareState<AIManager>(this, sqrRange);
            m_investigateAIState = new AIInvestigateState<AIManager>(this, sqrRange);
            m_patrolAIState = new AIPatrolState<AIManager>(this, sqrRange);
            m_chaseAIState = new AIChaseState<AIManager>(this, sqrRange);
            m_searchAIState = new AISearchState<AIManager>(this, sqrRange);
            m_coverAIState = new AICoverState<AIManager>(this);
            m_gunFireAIState = new AIGunFireState(this);

            if(m_unwareAIState == null || m_investigateAIState == null ||
                m_patrolAIState == null || m_chaseAIState == null ||
                m_searchAIState == null || m_gunFireAIState == null)
            {
                Debug.LogError("AI State is null");
            }

            if (AIStates[AIIndex] == "UnwareState")
            {
                m_currentAIState = m_unwareAIState;
            }
            else if (AIStates[AIIndex] == "InvestigateState")
            {
                m_currentAIState = m_investigateAIState;
            }
            else if (AIStates[AIIndex] == "PatrolState")
            {
                m_currentAIState = m_patrolAIState;
            }
            else if (AIStates[AIIndex] == "SearchState")
            {
                m_currentAIState = m_searchAIState;
            }
            else if (AIStates[AIIndex] == "ChaseState")
            {
                m_currentAIState = m_chaseAIState;
            }
            else if (AIStates[AIIndex] == "CoverState")
            {
                m_currentAIState = m_coverAIState;
            }
            else if (AIStates[AIIndex] == "CorrespondingActionState")
            {
                if(m_CharType == CharacterType.EnemyGunner || m_CharType == CharacterType.PlayerGunnerCampanion)
                {
                    m_currentAIState = m_gunFireAIState;
                }
                else
                {
                    m_currentAIState = m_boxingAIState;
                }                
            }
        }

        public void Fire(Vector3 directionOfFire)
        {
            directionOfFire.y = 0f;
            if(m_isEquippedWithWeapon == false || Vector3.Angle(transform.forward, directionOfFire) > 30f)
            {
                return;
            }

            m_animator.SetInteger("ShootID", 0);
            m_animator.SetTrigger("Shoot");
            m_activeGun.Fire(transform.forward);
        }

        public void Reload()
        {
            m_isReloading = true;
            m_animator.SetBool("IsReloading", m_isReloading);
        }

        public override void EquipWithWeapon()
        {
            StartCoroutine(EquipingWeaponIEnumerator());
        }

        public override void UnEquipWeapon()
        {
            m_isEquippedWithWeapon = false;
            m_animator.SetTrigger("UnEquip");
            StartCoroutine(UnEquipingIEnumerator());
        }

        private IEnumerator EquipingWeaponIEnumerator()
        {
            yield return StartCoroutine(ChangeAnimationLayer(1, 0));

            while (true)
            {
                if (m_activeGun.m_typeOfGun == TypeOfGun.OneHandedGun)
                {
                    float weight = m_animator.GetLayerWeight(2);
                    weight += Time.deltaTime;
                    weight = Mathf.Clamp01(weight);
                    m_animator.SetLayerWeight(2, weight);

                    if(weight >= 1f)
                    {
                        break;
                    }
                }
                else if (m_activeGun.m_typeOfGun == TypeOfGun.TwoHandedGun)
                {
                    float weight = m_animator.GetLayerWeight(3);
                    weight += Time.deltaTime;
                    weight = Mathf.Clamp01(weight);
                    m_animator.SetLayerWeight(3, weight);

                    if (weight >= 1f)
                    {
                        break;
                    }
                }

                yield return null;
            }

            m_animator.SetTrigger("Equip");

            yield return new WaitForSeconds(3f);
            m_isEquippedWithWeapon = true;
            yield return null;
        }

        private IEnumerator UnEquipingIEnumerator()
        {
            yield return new WaitForSeconds(1f);

            while (true)
            {
                if (m_activeGun.m_typeOfGun == TypeOfGun.OneHandedGun)
                {
                    float weight = m_animator.GetLayerWeight(2);
                    weight -= Time.deltaTime;
                    weight = Mathf.Clamp01(weight);
                    m_animator.SetLayerWeight(2, weight);

                    if (weight <= 0f)
                    {
                        break;
                    }
                }
                else if (m_activeGun.m_typeOfGun == TypeOfGun.TwoHandedGun)
                {
                    float weight = m_animator.GetLayerWeight(3);
                    weight -= Time.deltaTime;
                    weight = Mathf.Clamp01(weight);
                    m_animator.SetLayerWeight(3, weight);

                    if (weight <= 0f)
                    {
                        break;
                    }
                }
                yield return null;
            }

            StartCoroutine(ChangeAnimationLayer(0, 1));
            yield return null;
        }

        //Call by animation events
        public void SetParentPositionOfGun(int num)
        {
            if (num == 1)
            {
                m_activeGun.gameObject.transform.SetParent(m_EquipingHandGunPosition);
                m_activeGun.gameObject.transform.localPosition = Vector3.zero;
                m_activeGun.gameObject.transform.localRotation = Quaternion.identity;
            }
            else if (num == 2)
            {
                m_activeGun.gameObject.transform.SetParent(m_EquipingAssultRiflePosition);
                m_activeGun.gameObject.transform.localPosition = Vector3.zero;
                m_activeGun.gameObject.transform.localRotation = Quaternion.identity;
            }
            else if (num == 3)
            {
                if (m_activeGun.m_typeOfGun == TypeOfGun.OneHandedGun)
                {
                    m_activeGun.gameObject.transform.SetParent(m_HandGunHoldingPosition);
                    m_activeGun.gameObject.transform.localPosition = Vector3.zero;
                    m_activeGun.gameObject.transform.localRotation = Quaternion.identity;
                }
                else if (m_activeGun.m_typeOfGun == TypeOfGun.TwoHandedGun)
                {
                    m_activeGun.gameObject.transform.SetParent(m_EquipingHandGunPosition);
                    m_activeGun.gameObject.transform.localPosition = Vector3.zero;
                    m_activeGun.gameObject.transform.localRotation = Quaternion.identity;
                }
            }
            else if (num == 4)
            {
                if (m_activeGun.m_typeOfGun == TypeOfGun.TwoHandedGun)
                {
                    m_activeGun.gameObject.transform.SetParent(m_AssaultRifleHoldingPosition);
                    m_activeGun.gameObject.transform.localPosition = Vector3.zero;
                    m_activeGun.gameObject.transform.localRotation = Quaternion.identity;
                }

            }
        }

        //Call by animation events
        public void Reloading(string reload)
        {
            if (reload == "start")
            {
                m_isReloading = true;
            }
            else if (reload == "exit")
            {
                m_isReloading = false;
                m_animator.SetBool("IsReloading", m_isReloading);
                m_activeGun.m_CurrentBulletRound = m_activeGun.m_MaggazineTotalBullet;
            }
        }
    }
}