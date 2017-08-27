using System.Collections.Generic;
using UnityEngine;
using AIManager_namespace;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class CoverFinder : MonoBehaviour
{
    //[HideInInspector]
    public List<CoverPosition> m_coverPositionScripts;

    private AIManager m_aiManager;

    private void Start()
    {
        m_aiManager = GetComponentInParent<AIManager>();
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = m_aiManager.m_AIField.RangeOfSearchingCoverPos;
        sphereCollider.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("CoverFinder");

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        
        m_coverPositionScripts = new List<CoverPosition>();
    }

    private void OnTriggerEnter(Collider other)
    {
        CoverPosition coverPos = other.GetComponent<CoverPosition>();
        if (coverPos != null)
        {
            if (!m_coverPositionScripts.Contains(coverPos))
            {
                m_coverPositionScripts.Add(coverPos);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CoverPosition coverPos = other.GetComponent<CoverPosition>();
        if (coverPos != null)
        {
            m_coverPositionScripts.Remove(coverPos);
        }
        else
        {
            Debug.Log(other.name);
        }
    }
}
