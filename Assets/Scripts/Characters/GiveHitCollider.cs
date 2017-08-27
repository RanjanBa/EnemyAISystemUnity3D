using UnityEngine;
using AIManager_namespace;

[RequireComponent(typeof(SphereCollider))]
public class GiveHitCollider : MonoBehaviour {
    public SelectCharacterForm m_CharacterForm;
    public HitColliderType m_HitColliderType;
    public PlayerManager m_playerManager;
    public AIBoxerManager m_AIBoxerManager;

    void Start()
    {
        if (m_CharacterForm == SelectCharacterForm.Player)
        {
            m_playerManager = transform.root.gameObject.GetComponentInChildren<PlayerManager>();
        }
        else if (m_CharacterForm == SelectCharacterForm.AIManager)
        {
            m_AIBoxerManager = transform.root.gameObject.GetComponentInChildren<AIBoxerManager>();
        }

        SphereCollider col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 0.1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(this.enabled == false)
        {
            return;
        }

        if (m_CharacterForm == SelectCharacterForm.Player)
        {
            GameObject gm = other.transform.root.gameObject;
            if (gm.CompareTag("Enemy"))
            {
                AIManager enemyCm = gm.GetComponentInChildren<AIManager>();
                if (enemyCm != null)
                {
                    m_playerManager.GiveDamage(enemyCm, transform.position);
                    this.enabled = false;
                }
            }
        }
        else if(m_CharacterForm == SelectCharacterForm.AIManager)
        {
            GameObject gm = other.transform.root.gameObject;
            if (this.transform.root.gameObject.CompareTag("Enemy"))
            {
                if (gm.CompareTag("PlayerCampanion"))
                {
                    AIManager aiManager = gm.GetComponentInChildren<AIManager>();
                    if (aiManager != null)
                    {
                        m_AIBoxerManager.GiveDamage<AIManager>(aiManager, transform.position);
                        this.enabled = false;
                    }
                }else if (gm.CompareTag("Player"))
                {
                    PlayerManager playerManager = gm.GetComponentInChildren<PlayerManager>();
                    if (playerManager != null)
                    {
                        m_AIBoxerManager.GiveDamage<PlayerManager>(playerManager, transform.position);
                        this.enabled = false;
                    }
                }
            }else if (this.transform.root.gameObject.CompareTag("PlayerCampanion"))
            {
                if (gm.CompareTag("Enemy"))
                {
                    AIManager enemyCm = gm.GetComponentInChildren<AIManager>();
                    if (enemyCm != null)
                    {
                        m_AIBoxerManager.GiveDamage<AIManager>(enemyCm, transform.position);
                        this.enabled = false;
                    }
                }
            }
        }
    }
}
