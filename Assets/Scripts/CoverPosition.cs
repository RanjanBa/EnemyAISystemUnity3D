using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Collider))]
public class CoverPosition : MonoBehaviour
{
    public Collider m_Collider;
    [Tooltip("For placing the transform in the floor")]
    public LayerMask m_LayerMask;
    //[HideInInspector]
    public bool m_isOccupied = false;

    private void Start()
    {
        m_Collider = GetComponent<Collider>();
        m_Collider.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("CoverPosition");
        m_isOccupied = false;

        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hitInfo, 100f, m_LayerMask))
        {
            transform.position = hitInfo.point;
        }
        else if(Physics.Raycast(transform.position - Vector3.down, Vector3.up, out hitInfo, 100f, m_LayerMask))
        {
            transform.position = hitInfo.point;
        }
        else
        {
            Debug.Log("Cover Position is Detroyed" + this.gameObject.name);
            Debug.DrawRay(transform.position, Vector3.up * 100f, Color.blue, 10f);
            DestroyImmediate(this);
            return;
        }

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.forward, out hitInfo, 5f, m_LayerMask))
        {
            Debug.Log("1 " + hitInfo.collider.name);
            Debug.DrawRay(hitInfo.point, hitInfo.normal * 5f, Color.white, 20f);
            Vector3 pos = hitInfo.point + hitInfo.normal * 0.5f;
            pos.y = 0f;
            Debug.DrawRay(pos, Vector3.up * 5f, Color.white, 20f);
            transform.position = new Vector3(pos.x, transform.position.y, pos.z);
            transform.localRotation = Quaternion.LookRotation(-hitInfo.normal);
        }
        else if (Physics.Raycast(transform.position + Vector3.up * 0.5f, -Vector3.forward, out hitInfo, 5f, m_LayerMask))
        {
            Debug.Log("2 " + hitInfo.collider.name);
            Debug.DrawRay(hitInfo.point, hitInfo.normal * 5f, Color.white, 20f);
            Vector3 pos = hitInfo.point + hitInfo.normal * 0.5f;
            pos.y = 0f;
            Debug.DrawRay(pos, Vector3.up * 5f, Color.white, 20f);
            transform.position = new Vector3(pos.x, transform.position.y, pos.z);
            transform.localRotation = Quaternion.LookRotation(-hitInfo.normal);
        }
        else if (Physics.Raycast(transform.position + Vector3.up * 0.5f, -Vector3.right, out hitInfo, 5f, m_LayerMask))
        {
            Debug.Log("3 " + hitInfo.collider.name);
            Debug.DrawRay(hitInfo.point, hitInfo.normal * 5f, Color.white, 20f);
            Vector3 pos = hitInfo.point + hitInfo.normal * 0.5f;
            pos.y = 0f;
            Debug.DrawRay(pos, Vector3.up * 5f, Color.white, 20f);
            transform.position = new Vector3(pos.x, transform.position.y, pos.z);
            transform.localRotation = Quaternion.LookRotation(-hitInfo.normal);
        }
        else if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.right, out hitInfo, 5f, m_LayerMask))
        {
            Debug.Log("4 " + hitInfo.collider.name);
            Debug.DrawRay(hitInfo.point, hitInfo.normal * 5f, Color.white, 20f);
            Vector3 pos = hitInfo.point + hitInfo.normal * 0.5f;
            pos.y = 0f;
            Debug.DrawRay(pos, Vector3.up * 5f, Color.white, 20f);
            transform.position = new Vector3(pos.x, transform.position.y, pos.z);
            transform.localRotation = Quaternion.LookRotation(-hitInfo.normal);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(transform.position, Vector3.one * 0.5f);
    }
}
