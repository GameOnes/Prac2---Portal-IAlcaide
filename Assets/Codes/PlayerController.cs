using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector3 m_StartPosition;
    Quaternion m_StartRotation;
    float m_Yaw;
    float m_Pitch;
    public float m_YawSpeed;
    public float m_PitchSpeed;
    public float m_MinPitch;
    public float m_MaxPitch;
    public Transform m_PitchController;
    public bool m_UseInvertedYaw;
    public bool m_UseInvertedPitch;
    public CharacterController m_CharacterController;
    float m_VerticalSpeed = 0.0f;


    bool m_AngleLocked = false;
    public float m_Speed;
    public float m_JumpSpeed;
    public float m_SpeedMultiplier;

    [Header("Camera")]
    public Camera m_Camera;


    [Header("Input")]
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_GrabKeyCode = KeyCode.E;   
    public int m_BlueShootMouseButton = 0;
    public int m_OrangeShootMouseButton = 1;

    [Header("Debug Input")]
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;

    [Header("Animation")]
    public Animation m_Animation;
    public AnimationClip m_IdleAnimationClip;
    public AnimationClip m_ShootAnimationClip;
    public AnimationClip m_CantShootAnimationClip;
    

    [Header("Shoot")]
    public float m_ShootMaxDistance;
    public LayerMask m_ShootLayerMask;

    [Header("Portal")]
    public float m_PortalDistance = 1.5f;
    public float m_MaxAngleToTeleport = 75.0f;
    Vector3 m_MovementDirection;

    [Header("Portals")]
    public Portal m_BluePortal;
    public Portal m_OrangePortal;
    public GameObject m_BluePrev;
    public GameObject m_OrangePrev;

    [Header("Time")]
    public float m_TimeToShoot = 0.5f;
    private float m_currentTime = 0f;
    

    [Header("Object")]
    public ForceMode m_ForceMode;
    public float m_ThrowForce = 10.0f;
    public Transform m_GripTransform;
    Rigidbody m_AttachedObjectRb;
    bool m_AttachingObject;
    bool m_AttachedObject;
    Vector3 m_StartAttachingObjectPos;
    float m_AttachingCurrentTime;
    public float m_AttachingTime = 1.5f;
    public float m_AttachingObjectRotationDistanceLerp = 2.0f;
    public LayerMask m_ValidAttachObjectLayerMask;

    
   

    void Start()
    {

        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        //m_Animation.Play(m_IdleAnimationClip.name);
        m_currentTime += Time.deltaTime;
        //SetIdleAnimation();

        float l_MouseX = Input.GetAxis("Mouse X");
        float l_MouseY = Input.GetAxis("Mouse Y");

        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;

        if (!m_AngleLocked)
        {
            m_Yaw = m_Yaw + l_MouseX * m_YawSpeed * Time.deltaTime * (m_UseInvertedYaw ? -1.0f : 1.0f);
            m_Pitch = m_Pitch + l_MouseY * m_PitchSpeed * Time.deltaTime * (m_UseInvertedPitch ? -1.0f : 1.0f);
            m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);
            transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
            m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);
        }

        Vector3 l_Movement = Vector3.zero;
        float l_YawPiRadians = m_Yaw * Mathf.Deg2Rad;
        float l_Yaw90PiRadians = (m_Yaw + 90.0f) * Mathf.Deg2Rad;
        Vector3 l_ForwardDirection = new Vector3(Mathf.Sin(l_YawPiRadians), 0.0f, Mathf.Cos(l_YawPiRadians));
        Vector3 l_RightDirection = new Vector3(Mathf.Sin(l_Yaw90PiRadians), 0.0f, Mathf.Cos(l_Yaw90PiRadians));

        if (Input.GetKey(m_RightKeyCode))
            l_Movement = l_RightDirection;
        else if (Input.GetKey(m_LeftKeyCode))
            l_Movement = -l_RightDirection;

        if (Input.GetKey(m_UpKeyCode))
            l_Movement += l_ForwardDirection;
        else if (Input.GetKey(m_DownKeyCode))
            l_Movement -= l_ForwardDirection;

        float l_SpeedMultiplier = 1.0f;

        if (Input.GetKey(m_RunKeyCode))
            l_SpeedMultiplier = m_SpeedMultiplier;

        l_Movement.Normalize();
        m_MovementDirection = l_Movement;
        l_Movement *= m_Speed * l_SpeedMultiplier * Time.deltaTime;

        m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if (m_VerticalSpeed < 0.0f && (l_CollisionFlags & CollisionFlags.Below) != 0) //si estoy cayendo y colisiono con el suelo
        {
            m_VerticalSpeed = 0.0f;
            if (Input.GetKeyDown(m_JumpKeyCode))
                m_VerticalSpeed = m_JumpSpeed;
        }
        else if (m_VerticalSpeed > 0.0f && (l_CollisionFlags & CollisionFlags.Above) != 0) //si estoy subiendo y colision con un techo
            m_VerticalSpeed = 0.0f;

        if ( Input.GetMouseButton(m_BlueShootMouseButton) && CanShoot())
        {
            Debug.Log("Show blue portal");
            ShowPortal(m_BluePortal);

        }
        if (Input.GetMouseButtonUp(m_BlueShootMouseButton))
        {
            Debug.Log("Shoot blue");
           // m_BluePrev.SetActive(false);
            m_BluePortal.GetComponent<Collider>().enabled = true;
            Shoot(m_BluePortal);
        }
        if (Input.GetMouseButton(m_OrangeShootMouseButton) && CanShoot())
        {
            Debug.Log("Show orange portal");
            ShowPortal(m_OrangePortal);

        }
        if (Input.GetMouseButtonUp(m_OrangeShootMouseButton))
        {
            Debug.Log("Shoot orange");
            m_OrangePortal.GetComponent<Collider>().enabled = true;
            Shoot(m_OrangePortal);
        }
        
            
        //m_Animation.Play(m_CantShootAnimationClip.name);


        if (CanAttachObject())
            AttachObject();

        if(m_AttachedObjectRb != null)
            UpdateAttachedObject();
    }
    bool CanAttachObject()
    {
        return true;
    }
    void ShowPortal(Portal portalPreview)
    {
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_ShootMaxDistance, m_ShootLayerMask.value))
        {
            if (l_RaycastHit.collider.CompareTag("DrawableWall"))
            {
                Vector3 l_SpawnPortalPos = l_RaycastHit.point + l_RaycastHit.normal * 0.01f; // desplazamos un poco el portal para que no este pegado a la pared
                if (portalPreview.IsValidPosition(l_SpawnPortalPos, l_RaycastHit.normal)) // verificamos si la posicion es valida para el portal
                {
                    Debug.Log("Spawn Portal");
                    //SetShootAnimation();
                    portalPreview.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("Can't Spawn Portal");
                    //CantShootAnimation();
                    portalPreview.gameObject.SetActive(false);
                }

            }
        }  

    }

    bool CanShoot()
    {
        return m_currentTime >= m_TimeToShoot;
    }
   
    void Shoot(Portal _Portal)
    {
        

        m_currentTime = 0f;
        
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_ShootMaxDistance, m_ShootLayerMask.value))
        {
            if (l_RaycastHit.collider.CompareTag("DrawableWall"))
            {
                Vector3 l_SpawnPortalPos= l_RaycastHit.point + l_RaycastHit.normal * 0.01f; // desplazamos un poco el portal para que no este pegado a la pared
                if (_Portal.IsValidPosition(l_SpawnPortalPos, l_RaycastHit.normal)) // verificamos si la posicion es valida para el portal
                {
                    Debug.Log("Spawn Portal");
                    //SetShootAnimation();
                    _Portal.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("Can't Spawn Portal");
                    //CantShootAnimation();
                    _Portal.gameObject.SetActive(false);
                }




            }
        }

    }
    void Teleport(Portal _portal)
    {
       
        Vector3 l_TpPosition = transform.position + m_MovementDirection * m_PortalDistance;
        Vector3 l_LocalPosition = _portal.m_OtherPortalTransform.InverseTransformPoint(l_TpPosition); //convertimos la posicion del jugador al espacio local del portal de origen
        Vector3 l_WorldPosition = _portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition);

        Vector3 l_WorldCamForward = transform.forward; //direccion del jugador en el mundo
        Vector3 l_LocalCamForward = _portal.m_OtherPortalTransform.InverseTransformDirection(l_WorldCamForward); //convertimos la direccion del jugador al espacio local del portal
        l_WorldCamForward = _portal.m_MirrorPortal.transform.TransformDirection(l_LocalCamForward);
        //convertimos la direccion del jugador al espacio mundial del portal espejo ya que tenemos la posición transformada

        //tenemos que transformarlo en quaternion para rotarlo


        m_CharacterController.enabled = false; // deshabilitamos el character controller para evitar problemas al mover el objeto
        transform.position = l_WorldPosition;
        transform.rotation = Quaternion.LookRotation(l_WorldCamForward);
        m_Yaw = transform.rotation.eulerAngles.y;
        m_CharacterController.enabled = true; // habilitamos el character controller

    }

    //void SetIdleAnimation()
    //{
    //    m_Animation.CrossFade(m_IdleAnimationClip.name, 0.1f);

    //}
    //void CantShootAnimation()
    //{
    //    m_Animation.CrossFade(m_CantShootAnimationClip.name, 0.1f);
    //}

    //void SetShootAnimation()
    //{
    //    m_Animation.CrossFade(m_ShootAnimationClip.name, 0.1f);

    //}

  


    void AttachObject()
    {

        if (Input.GetKeyDown(m_GrabKeyCode))
        {
            Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            if(Physics.Raycast(l_Ray, out RaycastHit l_RaycastHit, m_ShootMaxDistance, m_ValidAttachObjectLayerMask.value))
            {
                //if(l_RaycastHit.collider.CompareTag("Cube"))
                //{
                //    AttachObject(l_RaycastHit.rigidbody);
                //}
                if((m_ValidAttachObjectLayerMask.value & (1 << l_RaycastHit.collider.gameObject.layer)) != 0) 
                {
                        AttachObject(l_RaycastHit.rigidbody);
                }
            }
        }
    }
    void AttachObject( Rigidbody _rb)
    {
            
        m_AttachingObject = true;
        m_AttachedObjectRb = _rb;
        m_AttachedObjectRb.GetComponent<CompanionCube>().SetAttachedObject(true);
        m_StartAttachingObjectPos =_rb.transform.position;
        m_AttachingCurrentTime = 0.0f;
        m_AttachedObject = false;
    }
    void UpdateAttachedObject()
    {
        if(m_AttachingObject)
        {
            m_AttachingCurrentTime += Time.deltaTime;
            float l_Pct = Mathf.Min(1.0f, m_AttachingCurrentTime / m_AttachingTime);
            Vector3 l_Position = Vector3.Lerp(m_StartAttachingObjectPos,m_GripTransform.position, l_Pct);
            float l_Distance = Vector3.Distance(l_Position, m_GripTransform.position);
            float l_RotationPct= 1.0f - Mathf.Min(1.0f, l_Distance/m_AttachingObjectRotationDistanceLerp);
            Quaternion l_Rotation = Quaternion.Lerp(transform.rotation, m_GripTransform.rotation, l_RotationPct);
            m_AttachedObjectRb.MovePosition(l_Position);
            m_AttachedObjectRb.MoveRotation(l_Rotation);

            if(l_Pct == 1.0f) //l_Pct = es literalmente el porcentaje de tiempo que ha pasado
            {
                m_AttachingObject = false;
                m_AttachedObject = true;
                m_AttachedObjectRb.transform.SetParent(m_GripTransform);
                m_AttachedObjectRb.transform.localPosition = Vector3.zero;
                m_AttachedObjectRb.transform.localRotation = Quaternion.identity;
                m_AttachedObjectRb.isKinematic = true; // desactivamos la fisica para que no se caiga
            }
        }
        if (Input.GetMouseButtonDown(0)) { ThrowObject(m_ThrowForce); }

        else if (Input.GetMouseButtonDown(1) || Input.GetKeyUp(m_GrabKeyCode)) { ThrowObject(0.0f); }

    }
    void ThrowObject(float Force)
    {
        m_AttachedObjectRb.isKinematic = false;
        m_AttachedObjectRb.AddForce(m_PitchController.forward*Force,m_ForceMode); 
        m_AttachedObjectRb.transform.SetParent(null);
        m_AttachingObject = false;
        m_AttachedObject = false;
        m_AttachedObjectRb.GetComponent<CompanionCube>().SetAttachedObject(false);
        m_AttachedObjectRb = null;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Portal")) // el usa un comparetag pero nunca me funciona 
        {
            Portal l_Portal = other.GetComponent<Portal>();
            if (CanTeleport(l_Portal)) 
                Teleport(l_Portal);

        }

    }
    bool CanTeleport(Portal l_Portal)
    {
       float l_Dotvalue = Vector3.Dot(l_Portal.transform.forward,-m_MovementDirection); // esto nos da el angulo entre la direccion del portal y la direccion del jugador
        return l_Dotvalue > Mathf.Cos(m_MaxAngleToTeleport*Mathf.Deg2Rad); // esto nos dice si el angulo es menor que el angulo maximo permitido y devuelve true o false
    }


    public void Restart()
    {
        //Fade();
        m_CharacterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_CharacterController.enabled = true;
    }
}
