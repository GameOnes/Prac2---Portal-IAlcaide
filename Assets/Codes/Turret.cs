
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Turret")]

    bool m_IsDead = false;
 
    public float m_MaxAlifeAngle = 45.0f;
    public float m_DeathForceThreshold = 5f;
    Rigidbody m_Rigidbody;
    bool m_AttachedObject = false;

    [Header("Laser")]
    public LineRenderer m_LineRenderer;
    public float m_MaxDistance = 50.0f;
    public LayerMask m_LayerMask;
    bool m_ReflectingLaser = false;
    bool m_PickingTurret = false;

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
        if (m_IsDead)
        {
            if (m_LineRenderer.enabled)
                m_LineRenderer.enabled = false;
            return;
        }

        if (m_PickingTurret)
        {
            if (m_LineRenderer.enabled)
                m_LineRenderer.enabled = false;
            return;
        }

        UpdateLaser();
        float l_DotAngle = Vector3.Dot(transform.up, Vector3.up);
        if (l_DotAngle < Mathf.Cos(m_MaxAlifeAngle * Mathf.Deg2Rad))
        {
            DisableTurret();
            return;
        }
    }

    public void UpdateLaser()
    {
        if (!m_LineRenderer.enabled)
            m_LineRenderer.enabled = true;
        float l_Distance = m_MaxDistance;
        Ray l_Ray = new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward);
        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_MaxDistance, m_LayerMask.value, QueryTriggerInteraction.Ignore))
        {
            l_Distance = l_RaycastHit.distance;
            if (l_RaycastHit.collider.CompareTag("RefractionCube"))
            {
                RefrectualCube l_RefrectualCube = l_RaycastHit.collider.GetComponent<RefrectualCube>();
                l_RefrectualCube.Reflect();
                m_ReflectingLaser = true;
            }
            if (l_RaycastHit.collider.CompareTag("Player"))
            {
                PlayerController l_Player = l_RaycastHit.collider.gameObject.GetComponentInParent<PlayerController>();
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
        m_LineRenderer.gameObject.SetActive(false);
        m_ReflectingLaser = false;
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.useGravity = true;
    }
}
