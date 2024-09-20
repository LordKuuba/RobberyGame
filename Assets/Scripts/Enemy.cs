using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

/*
[System.Serializable]
public struct AIPoints
{
    public Transform m_pointToGo;
    public float m_timeToWait;

    public AIPoints(Transform pointToGo, float timeToWait)
    {
        m_pointToGo = pointToGo;
        m_timeToWait = timeToWait;
    }
}

public class Enemy : Seeker
{
    [Header("Enemy")]
    [Space]

    [SerializeField] private List<AIPoint> patrolPoints = new List<AIPoint>();
    [SerializeField] private float speed = 2.0f;

    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private bool m_hasToRoam;

    private int p_currentPointIndex;
    private bool p_isMoving;
    private float p_timeWaiting;

    protected override void Start()
    {
        base.Start();

        m_warningNeedFollow = true;

        //MoveToPoint(patrolPoints[0].m_pointToGo);
    }
    void Update()
    {
        CheckForPlayer();
        ProcessAI();
    }
    void MoveToPoint(Transform m_positionToGo)
    {
        agent.SetDestination(m_positionToGo.position);
        p_isMoving = true;
    }
    void GetNextPointIndex()
    {
        if(p_currentPointIndex < patrolPoints.Count - 1)
        {
            p_currentPointIndex += 1;
        }
        else
        {
            p_currentPointIndex = 0;
        }
    }
    void ProcessAI()
    {
        if (m_hasToRoam)
        {
            if (!p_isMoving)
            {
                MoveToPoint(patrolPoints[p_currentPointIndex].m_pointToGo);
                GetNextPointIndex();
            }

            if(agent.remainingDistance <= agent.stoppingDistance)
            {
                if(p_timeWaiting < patrolPoints[p_currentPointIndex].m_timeToWait)
                {
                    p_timeWaiting += 1 * Time.deltaTime;
                }
                else
                {
                    p_timeWaiting = 0;
                    p_isMoving = false;
                }
            }
        }
    }
}
*/