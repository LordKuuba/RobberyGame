using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform m_followTarget;

    [SerializeField] private Vector3 m_cameraPositionOffset;

    [SerializeField] private float m_lerpSpeed;

    void FixedUpdate()
    {
        if(m_followTarget != null)
        {
            // Calculate the target position with the offsets applied
            Vector3 targetPosition = m_followTarget.position + m_cameraPositionOffset;

            // Lerp the camera's position towards the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, m_lerpSpeed * Time.deltaTime);
        }
    }

    public void SetTarget(Transform target)
    {
        m_followTarget = target;
    }
}