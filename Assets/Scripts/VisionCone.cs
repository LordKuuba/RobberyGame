using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    [SerializeField] private Material VisionConeMaterial;
    private float VisionRange;
    private float VisionAngle;
    [SerializeField] private LayerMask VisionObstructingLayer; // layer with objects that obstruct the enemy view, like walls
    [SerializeField] private int VisionConeResolution = 120; // the vision cone will be made up of triangles, higher value = prettier cone

    private float p_visionAngleRadians;
    [SerializeField] private GameObject m_objectToAddMeshRenderer;

    private Mesh VisionConeMesh;
    private MeshFilter MeshFilter_;

    void Start()
    {
        // Add a MeshRenderer and MeshFilter to the object and assign the material
        m_objectToAddMeshRenderer.AddComponent<MeshRenderer>().material = VisionConeMaterial;
        MeshFilter_ = m_objectToAddMeshRenderer.AddComponent<MeshFilter>();

        // Initialize the mesh and assign it to the MeshFilter
        VisionConeMesh = new Mesh();
        MeshFilter_.mesh = VisionConeMesh;
    }

    void Update()
    {
        // Convert vision angle from degrees to radians
        p_visionAngleRadians = VisionAngle * Mathf.Deg2Rad;

        // Update the vision cone every frame
        DrawVisionCone();
    }

    void DrawVisionCone()
    {
        // Initialize arrays for vertices and triangles
        int[] triangles = new int[(VisionConeResolution - 1) * 3];
        Vector3[] Vertices = new Vector3[VisionConeResolution + 1];

        // Set the initial vertex at the origin (relative to the GameObject)
        Vertices[0] = Vector3.zero;

        float Currentangle = -p_visionAngleRadians / 2;
        float angleIncrement = p_visionAngleRadians / (VisionConeResolution - 1);

        for (int i = 0; i < VisionConeResolution; i++)
        {
            // Calculate the direction for this raycast, but now inverted to shoot backwards
            float Sine = Mathf.Sin(Currentangle);
            float Cosine = Mathf.Cos(Currentangle);
            Vector3 RaycastDirection = m_objectToAddMeshRenderer.transform.TransformDirection(new Vector3(Sine, 0, Cosine)); // Inverted direction
            Vector3 VertForward = new Vector3(Sine, 0, Cosine); // Inverted direction

            // Cast a ray in the calculated direction
            if (Physics.Raycast(m_objectToAddMeshRenderer.transform.position, RaycastDirection, out RaycastHit hit, VisionRange, VisionObstructingLayer))
            {
                // If the ray hits an object in the VisionObstructingLayer, set the vertex at the hit point
                Vertices[i + 1] = VertForward * hit.distance;
            }
            else
            {
                // Otherwise, set the vertex at the max vision range
                Vertices[i + 1] = VertForward * VisionRange;
            }

            // Move to the next angle
            Currentangle += angleIncrement;

            // Debugging: visualize the raycast (optional)
            Debug.DrawRay(m_objectToAddMeshRenderer.transform.position, RaycastDirection * VisionRange, Color.red, 0.1f);
        }

        // Create the triangles for the mesh
        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }

        // Clear the previous mesh data, then assign new vertices and triangles
        VisionConeMesh.Clear();
        VisionConeMesh.vertices = Vertices;
        VisionConeMesh.triangles = triangles;

        // Update the mesh in the MeshFilter
        MeshFilter_.mesh = VisionConeMesh;
    }

    public void SetUpVisionConeParams(float g_range, float g_angle)
    {
        VisionRange = g_range;
        VisionAngle = g_angle;
    }
}
