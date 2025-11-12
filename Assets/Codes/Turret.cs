using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public List<Turret> m_Turret;
    public LineRenderer m_LaserLineRenderer;
    public float m_MaxDistance = 50.0f;
    public LayerMask m_LayerMask;
    public float m_maxALifeAngle = 15.0f;
    

    private void Update()
    {
        if(m_LaserLineRenderer==null)
        {
            return;
        }

        float l_DotAngle = Vector3.Dot(transform.up, Vector3.up);
        if ((l_DotAngle < Mathf.Cos(m_maxALifeAngle * Mathf.Deg2Rad)))
        {
            m_LaserLineRenderer.gameObject.SetActive(false);
            return;
        }
        else
        {

            m_LaserLineRenderer.gameObject.SetActive(true);
            
            float l_Distance = m_MaxDistance;
            Ray l_Ray = new Ray(m_LaserLineRenderer.transform.position, m_LaserLineRenderer.transform.forward);

            if (Physics.Raycast(l_Ray, out RaycastHit l_RayCastHit, m_MaxDistance, m_LayerMask.value, QueryTriggerInteraction.Ignore)) // esto hace el raycast y detecta si choca con algo
            {
                l_Distance = l_RayCastHit.distance;

                PlayerController l_Player = l_RayCastHit.collider.GetComponentInParent<PlayerController>();
                if (l_Player != null) { l_Player.Restart(); }
                else
                {
                    RefrectualCube l_RefrectualCube = l_RayCastHit.collider.GetComponentInParent<RefrectualCube>();
                    if (l_RefrectualCube != null) { l_RefrectualCube.Reflect(); }
                    else
                    {
                        Turret l_Turret = l_RayCastHit.collider.GetComponentInParent<Turret>();
                        if (l_Turret != null) { l_Turret.Die(); }                

                    }
                }          
                
            }
            Vector3 l_Position = new Vector3(0, 0, l_Distance);
            m_LaserLineRenderer.SetPosition(1, l_Position);
        }
    }
    void Die()
    {
       for (int i = 0; i < m_Turret.Count; i++)
        {
            m_Turret[i].m_LaserLineRenderer.gameObject.SetActive(false);
            Destroy(m_Turret[i].gameObject);
        }
    }
}
