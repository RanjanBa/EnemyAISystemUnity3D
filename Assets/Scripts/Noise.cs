using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Noise : MonoBehaviour
{
    [Range(1, 50f)]
    public float radius = 1f;
    public Vector3 force;

    public LayerMask plateformLayer;

    private SphereCollider m_sphereCollider;

    private void Start()
    {
        m_sphereCollider = GetComponent<SphereCollider>();
        m_sphereCollider.isTrigger = true;
        m_sphereCollider.radius = radius;
        GetComponent<AudioSource>().maxDistance = radius;
        GetComponent<Rigidbody>().AddForce(force);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, -Vector3.up, out hitInfo, radius, plateformLayer))
            {
                other.GetComponent<AIManager_namespace.AIManager>().HearingPerception(hitInfo.point, radius, this.gameObject.tag);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
