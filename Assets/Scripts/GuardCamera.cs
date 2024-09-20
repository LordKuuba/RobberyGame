using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public struct CameraRotationPoint
{
    public float m_angle;
    public float m_timeToMove;
    public float m_timeToWatch;
}

public class GuardCamera : Seeker
{
    [Header("Camera Settings")]
    [Space]
    [SerializeField] private bool m_rotateCamera;
    [SerializeField] private List<CameraRotationPoint> m_rotationPoints = new List<CameraRotationPoint>();
    [SerializeField] private Transform m_rotationPoint;

    private int p_currentRotationPoint = 0;
    private bool p_waitForRotation;
    private float p_rotationWaiting;


    protected override void Start()
    {
        base.Start();

        if (m_rotateCamera)
        {
            p_waitForRotation = true;
        }
    }

    public void Update()
    {
        //Looking And Detecting
        CheckForPlayer();

        //Rotating
        if (p_waitForRotation)
        {
            if(p_rotationWaiting < m_rotationPoints[p_currentRotationPoint].m_timeToWatch)
            {
                p_rotationWaiting += 1 * Time.deltaTime;
            }
            else
            {
                CmdRotateCamera();
            }
        }
    }

    
    [Command(requiresAuthority = false)]
    void CmdRotateCamera()
    {
        RpcRotateCamera();
    }

    [ClientRpc]
    void RpcRotateCamera()
    {
        StartCoroutine(RotateToPoint());
        p_rotationWaiting = 0;
    }

    IEnumerator RotateToPoint()
    {
        p_waitForRotation = false;
        float m_degreeToRotate = m_rotationPoints[p_currentRotationPoint].m_angle;
        float m_timeToRotate = m_rotationPoints[p_currentRotationPoint].m_timeToMove;

        Quaternion initialRotation = m_rotationPoint.localRotation;
        Quaternion finalRotation = Quaternion.Euler(m_rotationPoint.eulerAngles.x, m_degreeToRotate, m_rotationPoint.eulerAngles.z);

        float elapsedTime = 0;

        while(elapsedTime < m_timeToRotate)
        {
            m_rotationPoint.localRotation = Quaternion.Lerp(initialRotation, finalRotation, (elapsedTime/m_timeToRotate));
            elapsedTime += 1 * Time.deltaTime;
            yield return null;
        }

        m_rotationPoint.localRotation = finalRotation;
        GetNextPoint();
        p_waitForRotation = true;
    }

    void GetNextPoint()
    {
        if (p_currentRotationPoint < m_rotationPoints.Count - 1)
        {
            p_currentRotationPoint += 1;
        }
        else
        {
            p_currentRotationPoint = 0;
        }
    }
}