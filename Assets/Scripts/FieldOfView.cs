using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private float radius;
    [Range(0, 360)]
    private float angle;
    private float m_blindZoneRadius;

    private GameObject playerRef;
    private Player playerPRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public Transform cameraWatchPoint;

    private bool canSeePlayer;
    public bool IsPlayerInRange => canSeePlayer;

    public GameObject ClosestPlayer => playerRef;
    public float FOVRadius => radius;
    public float FOVAngle => angle;
    public float FOVBlindZone => m_blindZoneRadius;

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(cameraWatchPoint.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform closestTarget = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider collider in rangeChecks)
            {
                Transform target = collider.transform;
                Vector3 directionToTarget = (target.position - cameraWatchPoint.position).normalized;

                if (Vector3.Angle(cameraWatchPoint.forward, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(cameraWatchPoint.position, target.position);

                    if (distanceToTarget < closestDistance && !Physics.Raycast(cameraWatchPoint.position, directionToTarget, distanceToTarget, obstructionMask) && distanceToTarget > m_blindZoneRadius)
                    {
                        closestDistance = distanceToTarget;
                        closestTarget = target;

                        playerRef = closestTarget.gameObject;
                    }
                }
            }

            if (closestTarget != null)
            {
                canSeePlayer = true;
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
        }
    }

    public void SetupParameters(float g_radius, float g_angle, float g_blindZoneRadius)
    {
        radius = g_radius;
        angle = g_angle;
        m_blindZoneRadius = g_blindZoneRadius;
    }

}