

//public class Turret : MonoBehaviour
//{
//    public List<Turret> m_Turret;
//    public LineRenderer m_LaserLineRenderer;
//    public float m_MaxDistance = 50.0f;
//    public LayerMask m_LayerMask;
//    public float m_maxALifeAngle = 15.0f;


//    private void Update()
//    {

//        float l_DotAngle = Vector3.Dot(transform.up, Vector3.up);
//        if ((l_DotAngle < Mathf.Cos(m_maxALifeAngle * Mathf.Deg2Rad)))
//        {
//            m_LaserLineRenderer.gameObject.SetActive(false);
//            return;
//        }
//        else
//        {

//            m_LaserLineRenderer.gameObject.SetActive(true);

//            float l_Distance = m_MaxDistance;
//            Ray l_Ray = new Ray(m_LaserLineRenderer.transform.position, m_LaserLineRenderer.transform.forward);

//            if (Physics.Raycast(l_Ray, out RaycastHit l_RayCastHit, m_MaxDistance, m_LayerMask.value, QueryTriggerInteraction.Ignore)) // esto hace el raycast y detecta si choca con algo
//            {
//                l_Distance = l_RayCastHit.distance;

//                PlayerController l_Player = l_RayCastHit.collider.GetComponentInParent<PlayerController>();
//                if (l_Player != null) { l_Player.Restart(); }

//                RefrectualCube l_RefrectualCube = l_RayCastHit.collider.GetComponentInParent<RefrectualCube>();
//                if (l_RefrectualCube != null) { l_RefrectualCube.Reflect(); }

//                Turret l_Turret = l_RayCastHit.collider.GetComponent<Turret>();
//                if (l_Turret != null) { l_Turret.Die(); }

//            }
//            Vector3 l_Position = new Vector3(0, 0, l_Distance);
//            m_LaserLineRenderer.SetPosition(1, l_Position);
//        }
//    }
//    void Die()
//    {
//       for (int i = 0; i < m_Turret.Count; i++)
//        {
//            m_Turret[i].m_LaserLineRenderer.gameObject.SetActive(false);
//            Destroy(m_Turret[i].gameObject);
//        }
//    }
//}
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Turret")]

    bool m_IsDead = false;
    bool m_IsHeld = false;
    public float m_MaxAlifeAngle = 15.0f;
    public float m_DeathForceThreshold = 5f;
    Rigidbody m_Rigidbody;
    bool m_AttachedObject = false;

    [Header("Laser")]
    public LineRenderer m_LineRenderer;
    public float m_MaxDistance = 50.0f;
    public LayerMask m_LayerMask;
    bool m_IsReflecting = false;

    [Header("Teleport")]
    public float m_PortalDistance = 1.5f;
    public float m_MaxAngleToTeleport = 45.0f;

    private float m_CurrentTeleportTime = 0f;
    private float m_TeleportTime = 0.5f;





    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (m_LineRenderer != null)
            m_LineRenderer.gameObject.SetActive(true);
    }

    private void Update()
    {
        m_CurrentTeleportTime += Time.deltaTime;
        UpdateLaser();
        if (m_IsDead) return;
        if (m_IsHeld)
        {
            if (m_LineRenderer.enabled)
                m_LineRenderer.enabled = false;
            return;
        }
        float l_DotAngle = Vector3.Dot(transform.up, Vector3.up);
        if (l_DotAngle < Mathf.Cos(m_MaxAlifeAngle * Mathf.Deg2Rad))
        {
            DisableTurret();
            return;
        }

    }

    public void UpdateLaser()
    {
        m_LineRenderer.gameObject.SetActive(true);
        float l_Distance = m_MaxDistance;
        Ray l_Ray = new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward);
        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxDistance, m_LayerMask.value, QueryTriggerInteraction.Ignore))
        {
            l_Distance = l_RaycastHit.distance;
            if (l_RaycastHit.collider.CompareTag("RefractionCube"))
            {
                //l_RaycastHit.collider.GetComponent<RefractionCube>().Reflect();
            }
            if (l_RaycastHit.collider.CompareTag("Player"))
            {
                PlayerController l_Player = l_RaycastHit.collider.gameObject.GetComponent<PlayerController>();
                l_Player.Restart();
            }
            if (l_RaycastHit.collider.CompareTag("Turret"))
            {
                Turret l_OtherTurrer = l_RaycastHit.collider.gameObject.GetComponent<Turret>();
                l_OtherTurrer.DisableTurret();
            }
        }
        Vector3 l_Position = new Vector3(0.0f, 0.0f, l_Distance);
        m_LineRenderer.SetPosition(1, l_Position);
    }

    public void DisableTurret()
    {
        if (m_IsDead) return;
        m_IsDead = true;

        if (m_LineRenderer != null)
            m_LineRenderer.enabled = false;

        m_Rigidbody.isKinematic = false;
        m_Rigidbody.useGravity = true;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_IsDead) return;

        if (collision.gameObject.CompareTag("Cube") || collision.gameObject.CompareTag("Turret"))
        {
            if (collision.relativeVelocity.magnitude > m_DeathForceThreshold)
            {
                DisableTurret();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            Portal l_Portal = other.GetComponent<Portal>();
            if ((CanTeleport(l_Portal)))
            {
                Teleport(l_Portal);
            }
        }
    }
    bool CanTeleport(Portal _Portal)
    {
        float l_DotValue = Vector3.Dot(_Portal.transform.forward, m_Rigidbody.linearVelocity.normalized);
        return !m_AttachedObject && l_DotValue > Mathf.Cos(m_MaxAngleToTeleport * Mathf.Deg2Rad) && m_CurrentTeleportTime > m_TeleportTime;
    }
    void Teleport(Portal l_Portal)
    {
        Vector3 l_TpPosition = transform.position + m_Rigidbody.linearVelocity.normalized * m_PortalDistance;
        Vector3 l_LocalPosition = -l_Portal.m_OtherPortalTransform.InverseTransformPoint(l_TpPosition);
        Vector3 l_WorldPosition = l_Portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition);
        transform.position = l_Portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition);

        Vector3 l_WorldDirection = transform.forward;
        Vector3 l_LocalDirection = l_Portal.m_OtherPortalTransform.InverseTransformPoint(l_WorldDirection);
        transform.forward = l_Portal.m_MirrorPortal.transform.TransformDirection(l_LocalDirection);

        Vector3 l_localVelocity = l_Portal.m_OtherPortalTransform.InverseTransformDirection(m_Rigidbody.linearVelocity);
        m_Rigidbody.linearVelocity = l_Portal.m_MirrorPortal.transform.TransformDirection(l_localVelocity);
        m_CurrentTeleportTime = 0f;
    }

    public void SetAttachedObject(bool AttachedObject)
    {
        m_AttachedObject = AttachedObject;
    }
}
