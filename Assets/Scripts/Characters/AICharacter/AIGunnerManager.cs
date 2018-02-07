using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GunClass_namespace;

namespace AIManager_namespace
{
    public abstract class AIGunnerManager : AIManager
    {
#region AIGunner Variables
        [Header("AI Gunner Variables")]
        [Tooltip("Enemy gunners properties...")]
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
        public bool m_IsReloading = false;
        //[HideInInspector]
        public List<GunClass> m_Guns;
        //[HideInInspector]
        public GunClass m_CurrentlyActiveGun;
        public bool m_IsEquippedWithWeapon = false;

#endregion AIGunner Variables

        protected override void Initialized()
        {
            GunClass[] guns = GetComponentsInChildren<GunClass>();
            foreach (GunClass item in guns)
            {
                m_Guns.Add(item);
            }

            m_CurrentlyActiveGun = m_Guns[Random.Range(0, m_Guns.Count)];

            float sqrRange = m_AIGunnerField.AttackbyGunRange * m_AIGunnerField.AttackbyGunRange;

            m_unwareAIState = new AIUnwareState<AIManager>(this, sqrRange);
            m_patrolAIState = new AIPatrolState<AIManager>(this, sqrRange);
            m_chaseAIState = new AIChaseState<AIManager>(this, sqrRange);
            m_searchAIState = new AISearchState<AIManager>(this, sqrRange);
            m_coverAIState = new AICoverState<AIManager>(this);
            m_gunFireAIState = new AIGunFireState(this);

            if(m_unwareAIState == null || m_patrolAIState == null ||
                m_chaseAIState == null || m_searchAIState == null ||
                m_gunFireAIState == null)
            {
                Debug.LogError("AI State is null");
            }

            base.Initialized();
        }

        public void Fire(Vector3 opponentPosition)
        {
            if(m_IsEquippedWithWeapon == false || Vector3.Angle(transform.forward, opponentPosition - m_CurrentlyActiveGun.transform.position) > 30f)
            {
                return;
            }

            m_animator.SetInteger("ShootID", 0);
            m_animator.SetTrigger("Shoot");
            m_CurrentlyActiveGun.Fire(transform.forward);
        }

        public void Reload()
        {
            m_IsReloading = true;
            m_animator.SetBool("IsReloading", m_IsReloading);
        }

        public virtual void EquipWithWeapon()
        {
            StartCoroutine(EquipingWeaponIEnumerator());
        }

        public virtual void UnEquipWeapon()
        {
            m_IsEquippedWithWeapon = false;
            m_animator.SetTrigger("UnEquip");
            StartCoroutine(UnEquipingIEnumerator());
        }

        private void OnAnimatorMove()
        {
            
        }

        private IEnumerator EquipingWeaponIEnumerator()
        {
            yield return StartCoroutine(ChangeAnimationLayer(1, 0));

            while (true)
            {
                if (m_CurrentlyActiveGun.m_TypeOfGun == TypeOfGun.OneHandedGun)
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
                else if (m_CurrentlyActiveGun.m_TypeOfGun == TypeOfGun.TwoHandedGun)
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
            m_IsEquippedWithWeapon = true;
            yield return null;
        }

        private IEnumerator UnEquipingIEnumerator()
        {
            yield return new WaitForSeconds(1f);

            while (true)
            {
                if (m_CurrentlyActiveGun.m_TypeOfGun == TypeOfGun.OneHandedGun)
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
                else if (m_CurrentlyActiveGun.m_TypeOfGun == TypeOfGun.TwoHandedGun)
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
                m_CurrentlyActiveGun.gameObject.transform.SetParent(m_EquipingHandGunPosition);
                m_CurrentlyActiveGun.gameObject.transform.localPosition = Vector3.zero;
                m_CurrentlyActiveGun.gameObject.transform.localRotation = Quaternion.identity;
            }
            else if (num == 2)
            {
                m_CurrentlyActiveGun.gameObject.transform.SetParent(m_EquipingAssultRiflePosition);
                m_CurrentlyActiveGun.gameObject.transform.localPosition = Vector3.zero;
                m_CurrentlyActiveGun.gameObject.transform.localRotation = Quaternion.identity;
            }
            else if (num == 3)
            {
                if (m_CurrentlyActiveGun.m_TypeOfGun == TypeOfGun.OneHandedGun)
                {
                    m_CurrentlyActiveGun.gameObject.transform.SetParent(m_HandGunHoldingPosition);
                    m_CurrentlyActiveGun.gameObject.transform.localPosition = Vector3.zero;
                    m_CurrentlyActiveGun.gameObject.transform.localRotation = Quaternion.identity;
                }
                else if (m_CurrentlyActiveGun.m_TypeOfGun == TypeOfGun.TwoHandedGun)
                {
                    m_CurrentlyActiveGun.gameObject.transform.SetParent(m_EquipingHandGunPosition);
                    m_CurrentlyActiveGun.gameObject.transform.localPosition = Vector3.zero;
                    m_CurrentlyActiveGun.gameObject.transform.localRotation = Quaternion.identity;
                }
            }
            else if (num == 4)
            {
                if (m_CurrentlyActiveGun.m_TypeOfGun == TypeOfGun.TwoHandedGun)
                {
                    m_CurrentlyActiveGun.gameObject.transform.SetParent(m_AssaultRifleHoldingPosition);
                    m_CurrentlyActiveGun.gameObject.transform.localPosition = Vector3.zero;
                    m_CurrentlyActiveGun.gameObject.transform.localRotation = Quaternion.identity;
                }

            }
        }

        //Call by animation events
        public void Reloading(string reload)
        {
            if (reload == "start")
            {
                m_IsReloading = true;
            }
            else if (reload == "exit")
            {
                m_IsReloading = false;
                m_animator.SetBool("IsReloading", m_IsReloading);
                m_CurrentlyActiveGun.Reload();
            }
        }
    }
}