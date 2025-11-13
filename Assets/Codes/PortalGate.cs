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
    public void Opening()
    {
        if (m_State == TState.CLOSED)
        {
            m_State = TState.OPEN;
            m_Animation.Play(m_OpenAnimationClip.name);
            StartCoroutine(SetState(m_OpenAnimationClip.length,TState.OPENED));
        }
    }
    public void Closing()
    {
       if(m_State == TState.OPENED)
       {
            m_State = TState.CLOSE;
            m_Animation.Play(m_CloseAnimationClip.name);
            StartCoroutine(SetState(m_CloseAnimationClip.length, TState.CLOSED)); 
       }
    }
    IEnumerator SetState(float AnimationTime,TState state)
    {
        yield return new WaitForSeconds(AnimationTime); 
        m_State = state;
    }
}
