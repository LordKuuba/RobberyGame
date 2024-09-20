using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInteraction : MonoBehaviour
{
    [SerializeField] private float m_detectionRadius;
    [SerializeField] private LayerMask m_interactableLayerMask;
    private Vector3 _position;
    private Collider _closestCollider;

    [SerializeField] private Player player;

    private Interactable p_closestInteractable;
    private Interactable p_lastClosestInteractable;

    private bool interactionInProcess;

    void Update()
    {
        _position = transform.position;
        var newClosestCollider = FindClosestItemInSphere(_position, m_detectionRadius, m_interactableLayerMask);
        if (newClosestCollider != _closestCollider)
        {
            _closestCollider = newClosestCollider;

            p_lastClosestInteractable = p_closestInteractable;
            p_closestInteractable = _closestCollider?.GetComponent<Interactable>();

            p_closestInteractable?.DisplayKeyBind();
            p_lastClosestInteractable?.DeleteKeyBindDisplay();

        }
        if (p_closestInteractable != null)
        {
            KeyCode keyNeeded = p_closestInteractable.KeyToInteract;

            if (Input.GetKeyDown(keyNeeded) && !interactionInProcess)
            {
                p_closestInteractable.OnInteracted(player);
            }
        }
    }

    public void ChangeInteractingState(bool newState)
    {
        interactionInProcess = newState;
    }

    Collider FindClosestItemInSphere(Vector3 center, float radius, LayerMask layerMask)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius, layerMask);
        Collider closestCollider = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (Collider hitCollider in hitColliders)
        {
            float dSqrToTarget = (hitCollider.transform.position - center).sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr && hitCollider.GetComponent<Interactable>().IsInteractable)
            {
                closestDistanceSqr = dSqrToTarget;
                closestCollider = hitCollider;
            }
        }

        return closestCollider;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_detectionRadius);
    }
}
