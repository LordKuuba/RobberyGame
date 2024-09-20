using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.red;
        Handles.DrawWireArc(fov.cameraWatchPoint.transform.position, Vector3.up, Vector3.forward, 360, fov.FOVRadius);

        Handles.color = Color.green;
        Handles.DrawWireArc(fov.cameraWatchPoint.transform.position, Vector3.up, Vector3.forward, 360, fov.FOVBlindZone);

        Vector3 viewAngle01 = DirectionFromAngle(fov.cameraWatchPoint.transform.eulerAngles.y, -fov.FOVAngle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.cameraWatchPoint.transform.eulerAngles.y, fov.FOVAngle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.cameraWatchPoint.transform.position, fov.cameraWatchPoint.transform.position + viewAngle01 * fov.FOVRadius);
        Handles.DrawLine(fov.cameraWatchPoint.transform.position, fov.cameraWatchPoint.transform.position + viewAngle02 * fov.FOVRadius);

        if (fov.IsPlayerInRange)
        {
            Handles.color = Color.yellow;
            Handles.DrawLine(fov.cameraWatchPoint.transform.position, fov.ClosestPlayer.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}