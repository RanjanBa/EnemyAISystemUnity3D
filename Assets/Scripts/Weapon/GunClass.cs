using UnityEngine;

namespace gunClass_namespace
{
    public abstract class GunClass : MonoBehaviour
    {
        [Range(0.0001f, 10f)]
        public float m_GunFireRate;
        public int m_BulletDamage;
        public float m_FiringAccuracy = 0.1f;
        public Transform m_GunBarrelPos;
        public AudioClip m_FireClip;
        public AudioClip m_OutOfBulletClip;
        public TypeOfGun m_typeOfGun;
        public int m_CurrentBulletRound;
        public int m_MaggazineTotalBullet = 30;
        public string[] m_OpponentCharacterTag;
        public LayerMask m_BulletHittingLayerMask;

        protected string m_ownerTag;

        protected AudioSource m_GunAudioSource;

        protected virtual void Initialize()
        {
            m_OpponentCharacterTag = new string[2];
            m_OpponentCharacterTag[0] = "Player";
            m_OpponentCharacterTag[1] = "Enemy";
            m_CurrentBulletRound = m_MaggazineTotalBullet;
            m_GunAudioSource = GetComponent<AudioSource>();
        }

        public virtual void Fire(Vector3 direction)
        {
            direction += (Vector3)Random.insideUnitCircle * m_FiringAccuracy;
            RaycastHit hitInfo;
            if (Physics.Raycast(m_GunBarrelPos.position, direction, out hitInfo, m_BulletHittingLayerMask))
            {
                for (int i = 0; i < m_OpponentCharacterTag.Length; i++)
                {
                    if (m_OpponentCharacterTag[i].CompareTo(m_ownerTag) != 0)
                    {
                        if (hitInfo.transform.root.CompareTag(m_OpponentCharacterTag[i]))
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

            if(m_CurrentBulletRound == 0)
            {
                m_GunAudioSource.clip = m_OutOfBulletClip;
            }
            else if(m_CurrentBulletRound > 0)
            {
                m_GunAudioSource.clip = m_FireClip;
            }

            Debug.DrawRay(m_GunBarrelPos.position, direction * 100f, Color.yellow, 2f);
            m_CurrentBulletRound--;
            m_GunAudioSource.Stop();
            m_GunAudioSource.Play();
        }
    }
}
