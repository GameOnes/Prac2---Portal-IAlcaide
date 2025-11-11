using System.Collections;
using UnityEditorInternal;
using UnityEngine;

public class PortalGate : MonoBehaviour
{
    public Animation m_Animation;
    public AnimationClip m_OpenAnimationClip;
    public AnimationClip m_CloseAnimationClip;
    public AnimationClip m_OpenedAnimationClip;
    public AnimationClip m_ClosedAnimationClip;

    public bool m_IsOpened;
    
    public enum TState  
    {
        OPENED,
        CLOSED,
        OPEN,
        CLOSE
    }

    TState m_State;

    private void Start()
    {
        if(m_IsOpened)
        {
            SetOpenedState();
        }
        else
        {
            SetClosedState();
        }
    }
    void SetOpenedState()
    {
        m_State= TState.OPEN;
        m_Animation.Play(m_OpenedAnimationClip.name);
    }
    void SetClosedState()
    {
        m_State = TState.CLOSED;
        m_Animation.Play(m_ClosedAnimationClip.name);
    }
    public void Open()
    {
        if (m_State == TState.CLOSED)
        {
            m_State = TState.OPEN;
            m_Animation.Play(m_OpenAnimationClip.name);
            StartCoroutine(SetState(m_OpenAnimationClip.length,TState.OPENED));
        }
    }
    public void Close()
    {
        if (m_State == TState.CLOSED)
        {
            m_State = TState.OPEN;
            m_Animation.Play(m_OpenAnimationClip.name);
            StartCoroutine(SetState(m_OpenAnimationClip.length, TState.OPENED));
        }
    }
    IEnumerator SetState(float AnimationTime,TState state)
    {
        yield return new WaitForSeconds(AnimationTime); // yield returns control to the main loop and waits for the specified time
        m_State = state;
    }
}
