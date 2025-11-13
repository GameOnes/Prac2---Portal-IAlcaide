using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public PortalGate m_PortalGate;
    public BoxCollider m_BoxCollider;
    private int m_CubeCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cube") || other.CompareTag("RefractionCube")|| other.CompareTag("Player"))
        {
            
            m_CubeCount++;
            if (m_CubeCount >= 1)
            {
                Debug.Log("Enter Door Button");
                m_BoxCollider.enabled = false;
                m_PortalGate.Opening();
            }
            m_PortalGate.m_IsOpened = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cube") || other.CompareTag("RefractionCube") || other.CompareTag("Player"))
        {
            m_CubeCount--;
            if (m_CubeCount <= 0)
            {
                Debug.Log("Exit Door Button");
                m_CubeCount = 0;
                
                m_BoxCollider.enabled = true;
                m_PortalGate.Closing();
            }

            m_PortalGate.m_IsOpened = false;
        }
    }
}
