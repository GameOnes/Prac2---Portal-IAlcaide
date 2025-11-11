using UnityEngine;

public class CompanionSpawner : MonoBehaviour
{
   [Header("Spawn Settings")]
   public GameObject m_companionCubePrefab;
   public Transform m_spawnTransform;
   public LayerMask includeLayers;

   [Header("Time")]
   public float m_timeBetweenSpawns = 2f;
   private float m_currentTime = 0f;

    
    private void Update()
    {
        m_currentTime += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (((1 << other.gameObject.layer) & includeLayers) == 0) return; // Ignore included layers

        if(CanSpawn())
        {
            Spawn();
            m_currentTime = 0f;
        }

    }
    void Spawn()
    {
        GameObject l_GameObject = GameObject.Instantiate(m_companionCubePrefab);
        l_GameObject.transform.position = m_spawnTransform.position;
        l_GameObject.transform.rotation = m_spawnTransform.rotation;
    }
    private bool CanSpawn()
    {
        return m_currentTime >= m_timeBetweenSpawns;
    }
}
