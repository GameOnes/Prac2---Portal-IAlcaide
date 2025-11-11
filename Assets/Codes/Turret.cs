using UnityEngine;

public class Turret : MonoBehaviour
{
    public LineRenderer m_LaserLineRenderer;
    public float m_MaxDistance = 50.0f;
    public LayerMask m_LayerMask;
    public float m_maxALifeAngle = 15.0f;

    private void Update()
    {
        float l_DotAngle = Vector3.Dot(transform.up,Vector3.up);
        if ((l_DotAngle<Mathf.Cos(m_maxALifeAngle*Mathf.Deg2Rad)))
        {
            m_LaserLineRenderer.gameObject.SetActive(false);    
        }
        else
        {

            m_LaserLineRenderer.gameObject.SetActive(true);
            float l_Distance = m_MaxDistance;
            Ray l_Ray= new Ray(m_LaserLineRenderer.transform.position,m_LaserLineRenderer.transform.forward);
            if (Physics.Raycast(l_Ray,out RaycastHit l_RayCastHit, m_MaxDistance, m_LayerMask.value,QueryTriggerInteraction.Ignore))
            {
                l_Distance = l_RayCastHit.distance;
            }
            Vector3 l_Position = new Vector3(0, 0, l_Distance);
            m_LaserLineRenderer.SetPosition(1, l_Position);
        }
    }
}
