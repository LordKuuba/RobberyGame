using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Task : MonoBehaviour
{
    protected UnityEvent m_callWhenDone = new UnityEvent();

    public void SetCallWhenDone(UnityAction actionToCall)
    {
        m_callWhenDone.AddListener(actionToCall);
    }

    public virtual void CallFunction()
    {
        m_callWhenDone.Invoke();
    }
}
