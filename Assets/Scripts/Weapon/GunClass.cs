using UnityEngine;

namespace GunClass_namespace
{
    public abstract class GunClass : MonoBehaviour
    {
        [Range(0.0001f, 10f)]
        public float m_GunFireRate;
        public int m_BulletDamage;
        public float m_FiringAccuracy = 0.1f;
        public TypeOfGun m_TypeOfGun;
        public Transform m_LeftHandPosition;
        public Transform m_RightHandPosition;
        public int M_RemainingAmmunition
        {
            get
            {
                return m_remainingAmmunition;
            }
        }

        [SerializeField]
        protected int m_MaggazineTotalBullet = 30;
        [SerializeField]
        private AudioClip m_fireClip;
        [SerializeField]
        private AudioClip m_outOfBulletClip;
        [SerializeField]
        private LayerMask m_bulletHittingLayerMask;
        [SerializeField]
        private Transform m_gunBarrelPos;

        protected int m_remainingAmmunition;
        protected string[] m_opponentCharacterTag;
        protected string m_ownerTag;

        protected AudioSource m_GunAudioSource;

        protected virtual void Initialize()
        {
            m_opponentCharacterTag = new string[2];
            m_opponentCharacterTag[0] = "Player";
            m_opponentCharacterTag[1] = "Enemy";
            m_ownerTag = transform.root.tag;
            m_remainingAmmunition = m_MaggazineTotalBullet;
            m_GunAudioSource = GetComponent<AudioSource>();
        }

        public virtual void Fire(Vector3 direction)
        {
            if (M_RemainingAmmunition <= 0)
            {
                m_GunAudioSource.clip = m_outOfBulletClip;
            }
            else if (M_RemainingAmmunition > 0)
            {
                m_GunAudioSource.clip = m_fireClip;
                
            }

            m_GunAudioSource.Stop();
            m_GunAudioSource.Play();
            if(M_RemainingAmmunition <= 0)
            {
                return;
            }

            direction += (Vector3)Random.insideUnitCircle * m_FiringAccuracy;
            RaycastHit hitInfo;
            if (Physics.Raycast(m_gunBarrelPos.position, direction, out hitInfo, m_bulletHittingLayerMask))
            {
                Debug.DrawRay(m_gunBarrelPos.position, direction * hitInfo.distance, Color.red, 2f);
                for (int i = 0; i < m_opponentCharacterTag.Length; i++)
                {
                    if (m_opponentCharacterTag[i].CompareTo(m_ownerTag) != 0)
                    {
                        if (hitInfo.transform.root.CompareTag(m_opponentCharacterTag[i]))
                        {
                            CharacterManager charManger = hitInfo.transform.root.GetComponent<CharacterManager>();
                            if (charManger != null)
                            {
                                charManger.TakeDamage(m_BulletDamage, DamageType.DamageByGun);
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.DrawRay(m_gunBarrelPos.position, direction * 100f, Color.yellow, 2f);
            }

            m_remainingAmmunition--;
        }

        public virtual void Reload()
        {
            m_remainingAmmunition = m_MaggazineTotalBullet;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, transform.forward * 1000f);
        }
    }
}
