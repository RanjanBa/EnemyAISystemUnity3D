using UnityEngine;
using AIManager_namespace;

[ExecuteInEditMode]
[RequireComponent(typeof(SphereCollider))]
public class EyeVisualPerception : MonoBehaviour
{
    private SphereCollider m_sphereCollider;
    private AIManager m_aiManager;

    void Start()
    {
        m_sphereCollider = GetComponent<SphereCollider>();
        m_aiManager = GetComponentInParent<AIManager>();
        m_sphereCollider.isTrigger = true;
        m_sphereCollider.radius = m_aiManager.m_AIField.ViewRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == this.transform.root.gameObject)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            CharacterTypeOfGameObject ch = new CharacterTypeOfGameObject(other.gameObject, CharacterType.Player);
            CharacterTypeAndTimerOfGameObject chType = new CharacterTypeAndTimerOfGameObject(ch, 0f);
            m_aiManager.m_charWithTypeAndTimer.Add(chType);
        }
        else if (other.CompareTag("PlayerCampanion"))
        {
            if (other.gameObject.GetComponent<PlayerBoxerCampanion>() != null)
            {
                CharacterTypeOfGameObject ch = new CharacterTypeOfGameObject(other.gameObject, CharacterType.PlayerBoxerCampanion);
                CharacterTypeAndTimerOfGameObject chType = new CharacterTypeAndTimerOfGameObject(ch, 0f);
                m_aiManager.m_charWithTypeAndTimer.Add(chType);
            }
            else
            {
                CharacterTypeOfGameObject ch = new CharacterTypeOfGameObject(other.gameObject, CharacterType.PlayerGunnerCampanion);
                CharacterTypeAndTimerOfGameObject chType = new CharacterTypeAndTimerOfGameObject(ch, 0f);
                m_aiManager.m_charWithTypeAndTimer.Add(chType);
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            if (other.gameObject.GetComponent<EnemyGunner>() != null)
            {
                CharacterTypeOfGameObject ch = new CharacterTypeOfGameObject(other.gameObject, CharacterType.EnemyGunner);
                CharacterTypeAndTimerOfGameObject chType = new CharacterTypeAndTimerOfGameObject(ch, 0f);
                m_aiManager.m_charWithTypeAndTimer.Add(chType);
            }
            else
            {
                CharacterTypeOfGameObject ch = new CharacterTypeOfGameObject(other.gameObject, CharacterType.EnemyBoxer);
                CharacterTypeAndTimerOfGameObject chType = new CharacterTypeAndTimerOfGameObject(ch, 0f);
                m_aiManager.m_charWithTypeAndTimer.Add(chType);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == this.transform.root.gameObject)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            foreach (CharacterTypeAndTimerOfGameObject item in m_aiManager.m_charWithTypeAndTimer)
            {
                if (item.CharacterGameObjectWithType.characterGameObject == other.gameObject)
                {
                    m_aiManager.m_charWithTypeAndTimer.Remove(item);
                    m_aiManager.m_visibledCharWithType.Remove(item.CharacterGameObjectWithType);
                    break;
                }
            }

        }
        else if (other.CompareTag("PlayerCampanion"))
        {
            foreach (CharacterTypeAndTimerOfGameObject item in m_aiManager.m_charWithTypeAndTimer)
            {
                if (item.CharacterGameObjectWithType.characterGameObject == other.gameObject)
                {
                    m_aiManager.m_charWithTypeAndTimer.Remove(item);
                    m_aiManager.m_visibledCharWithType.Remove(item.CharacterGameObjectWithType);
                    break;
                }
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            foreach (CharacterTypeAndTimerOfGameObject item in m_aiManager.m_charWithTypeAndTimer)
            {
                if (item.CharacterGameObjectWithType.characterGameObject == other.gameObject)
                {
                    m_aiManager.m_charWithTypeAndTimer.Remove(item);
                    m_aiManager.m_visibledCharWithType.Remove(item.CharacterGameObjectWithType);
                    break;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(m_aiManager == null)
        {
            m_aiManager = GetComponentInParent<AIManager>();
            return;
        }

        if (!m_aiManager.DebugRayOfEyeRange)
        {
            return;
        }

        Gizmos.DrawWireSphere(transform.position, m_aiManager.m_AIField.ThresoldViewRange);

        Vector3 cros = Vector3.Cross(transform.forward, Vector3.up);
        cros.Normalize();

        float dist = m_aiManager.m_AIField.ViewRange * Mathf.Cos(Mathf.Deg2Rad * m_aiManager.m_AIField.ViewAngle);

        Vector3 center = transform.position + transform.forward * dist;
        Vector3 right = Vector3.zero;
        Vector3 left = Vector3.zero;
        Vector3 up = Vector3.zero;
        Vector3 down = Vector3.zero;

        if (m_aiManager.m_AIField.ViewAngle <= 90f)
        {
            right = Vector3.Normalize(transform.forward - cros * Mathf.Tan(Mathf.Deg2Rad * m_aiManager.m_AIField.ViewAngle)) * m_aiManager.m_AIField.ViewRange;
            left = Vector3.Normalize(transform.forward + cros * Mathf.Tan(Mathf.Deg2Rad * m_aiManager.m_AIField.ViewAngle)) * m_aiManager.m_AIField.ViewRange;
            up = Vector3.Normalize(transform.forward + transform.up * Mathf.Tan(Mathf.Deg2Rad * m_aiManager.m_AIField.ViewAngle)) * m_aiManager.m_AIField.ViewRange;
            down = Vector3.Normalize(transform.forward - transform.up * Mathf.Tan(Mathf.Deg2Rad * m_aiManager.m_AIField.ViewAngle)) * m_aiManager.m_AIField.ViewRange;
        }
        else
        {
            right = Vector3.Normalize(-transform.forward - cros * Mathf.Tan(Mathf.Deg2Rad * m_aiManager.m_AIField.ViewAngle)) * m_aiManager.m_AIField.ViewRange;
            left = Vector3.Normalize(-transform.forward + cros * Mathf.Tan(Mathf.Deg2Rad * m_aiManager.m_AIField.ViewAngle)) * m_aiManager.m_AIField.ViewRange;
            up = Vector3.Normalize(-transform.forward + transform.up * Mathf.Tan(Mathf.Deg2Rad * m_aiManager.m_AIField.ViewAngle)) * m_aiManager.m_AIField.ViewRange;
            down = Vector3.Normalize(-transform.forward - transform.up * Mathf.Tan(Mathf.Deg2Rad * m_aiManager.m_AIField.ViewAngle)) * m_aiManager.m_AIField.ViewRange;
        }

        float radius = Vector3.Distance(center, transform.position + up);

        Vector3 firstPos = transform.position + up;

        float angle = 0;
        int perAngle = 10;

        Gizmos.color = Color.yellow;

        for (int i = 0; i <= 36; i++)
        {
            angle = i * perAngle;
            float base_dist = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float perp_dist = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
            Vector3 lastPos = center + transform.up * base_dist + transform.right * perp_dist;
            if (i != 0)
                Gizmos.DrawLine(firstPos, lastPos);
            firstPos = lastPos;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, right);
        Gizmos.DrawRay(transform.position, left);
        Gizmos.DrawRay(transform.position, up);
        Gizmos.DrawRay(transform.position, down);

        Gizmos.color = Color.yellow;

        perAngle = 10;

        angle = 90 - m_aiManager.m_AIField.ViewAngle;

        for (int i = 0; i <= 100; i++)
        {
            float base_dist = Mathf.Cos(angle * Mathf.Deg2Rad) * m_aiManager.m_AIField.ViewRange;
            float perp_dist = Mathf.Sin(angle * Mathf.Deg2Rad) * m_aiManager.m_AIField.ViewRange;
            Vector3 lastPos = transform.position + transform.right * base_dist + transform.forward * perp_dist;
            if (i != 0)
                Gizmos.DrawLine(firstPos, lastPos);
            firstPos = lastPos;

            if (angle >= 90 + m_aiManager.m_AIField.ViewAngle)
            {
                break;
            }

            angle = angle + perAngle;

            if (angle > 90 + m_aiManager.m_AIField.ViewAngle)
            {
                angle = 90 + m_aiManager.m_AIField.ViewAngle;
            }
        }

        angle = 90 - m_aiManager.m_AIField.ViewAngle;

        for (int i = 0; i <= 100; i++)
        {
            float base_dist = Mathf.Cos(angle * Mathf.Deg2Rad) * m_aiManager.m_AIField.ViewRange;
            float perp_dist = Mathf.Sin(angle * Mathf.Deg2Rad) * m_aiManager.m_AIField.ViewRange;
            Vector3 lastPos = transform.position + transform.up * base_dist + transform.forward * perp_dist;
            if (i != 0)
                Gizmos.DrawLine(firstPos, lastPos);
            firstPos = lastPos;

            if (angle >= 90 + m_aiManager.m_AIField.ViewAngle)
            {
                break;
            }

            angle = angle + perAngle;

            if (angle > 90 + m_aiManager.m_AIField.ViewAngle)
            {
                angle = 90 + m_aiManager.m_AIField.ViewAngle;
            }
        }
    }
}
