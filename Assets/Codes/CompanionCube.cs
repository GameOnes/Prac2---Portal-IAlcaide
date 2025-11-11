using UnityEngine;

public class CompanionCube : MonoBehaviour
{
    Rigidbody m_RigidBody;
    public float m_PortalDistance = 1.5f;
    public float m_MaxAngleToTeleport = 45.0f;
    bool m_AttachedObject = false;

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Portal"))
        {
           Portal l_Portal = other.GetComponent<Portal>();
            if ((CanTeleport(l_Portal)))
            {
                Debug.Log("Teleporting Cube");

                Teleport(l_Portal);
            }
        }
    }
    bool CanTeleport(Portal _Portal)
    {
       float l_DotValue = Vector3.Dot(_Portal.transform.forward, -m_RigidBody.linearVelocity.normalized);
        return !m_AttachedObject && l_DotValue > Mathf.Cos(m_MaxAngleToTeleport * Mathf.Deg2Rad);
    }
    void Teleport(Portal _Portal)
    {
        Vector3 l_Direction = m_RigidBody.linearVelocity.normalized;
        Vector3 l_WorldPosition = transform.position + l_Direction * m_PortalDistance;
        Vector3 l_LocalPosition = _Portal.m_OtherPortalTransform.InverseTransformPoint(l_WorldPosition);
        transform.position = _Portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition);

        Vector3 l_WorldDirection = transform.forward;
        Vector3 l_LocalDirection = _Portal.m_OtherPortalTransform.InverseTransformPoint(l_WorldDirection);
        transform.forward= _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalDirection);

        Vector3 l_localVelocity = _Portal.m_OtherPortalTransform.InverseTransformDirection(m_RigidBody.linearVelocity);
        m_RigidBody.linearVelocity = _Portal.m_MirrorPortal.transform.TransformDirection(l_localVelocity);
        float l_Scale = _Portal.m_MirrorPortal.transform.localScale.x / _Portal.transform.localScale.x;
        m_RigidBody.transform.localScale= Vector3.one * l_Scale*m_RigidBody.transform.localScale.x;
    }
    public void SetAttachedObject(bool AttachedObject)
    {
        m_AttachedObject = AttachedObject;
    }
   
}
