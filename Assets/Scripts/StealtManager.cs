using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StealtManager : NetworkBehaviour
{
    public static StealtManager instance { get; private set; }

    [SyncVar]
    private bool isStealthSuccsesful = true;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public void FailStealth()
    {
        if (isStealthSuccsesful)
        {
            Debug.LogError("Task failed!");

            isStealthSuccsesful = false;
        }
    }
}
