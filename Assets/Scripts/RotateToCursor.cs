using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCursor : MonoBehaviour
{
    Camera m_camera;

    [SerializeField] private LayerMask m_rayLayer;

    public void SetCamera(Camera playerCamera)
    {
        m_camera = playerCamera;
    }

    void FixedUpdate()
    {
        if (m_camera != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 100f;
            mousePos = m_camera.ScreenToWorldPoint(mousePos);
            Debug.DrawRay(m_camera.transform.position, mousePos - m_camera.transform.position, Color.red);

            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, m_rayLayer))
            {
                Vector3 relativePos = hit.point - transform.position;

                // the second argument, upwards, defaults to Vector3.up
                Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                rotation.x = 0;
                rotation.z = 0;
                transform.rotation = rotation;
            }
        }
    }
}