using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Item : Interactable
{
    [Space]
    [SerializeField] private ItemParameters m_itemParameters;
    [SerializeField] private GameObject m_itemModel;
    [SerializeField] private Rigidbody m_itemRigidbody;
    [SerializeField] private Collider m_itemCollider;

    public ItemParameters ItemData => m_itemParameters;

    private Transform p_objectToFollow;
    private bool needToFollow;

    public override void OnInteracted(Player g_playerWhoInteracted)
    {
        if (!g_playerWhoInteracted.PlayerInventory.IsCurrentSlotTaken)
        {
            g_playerWhoInteracted.PlayerInventory.AddItemToCurrentlySelectedSlot(this);
            Debug.Log($"Succsesfully Item ({this.name}) was picked by {g_playerWhoInteracted}");
        }
    }

    void Update()
    {
        if (needToFollow)
        {
            transform.position = p_objectToFollow.position;
            transform.rotation = p_objectToFollow.rotation;
        }
    }

    public void ChangeDrawState(bool newState)
    {
        CmdChangeDrawState(newState);
    }
    [Command(requiresAuthority = false)]
    void CmdChangeDrawState(bool newState)
    {
        RpcChangeDrawState(newState);
    }
    [ClientRpc]
    void RpcChangeDrawState(bool newState)
    {
        m_itemModel.SetActive(newState);

        m_interactable = newState;
    }

    public void ChangePhysicsState(bool newState)
    {
        CmdChangePhysicsState(newState);
    }
    [Command(requiresAuthority = false)]
    void CmdChangePhysicsState(bool newState)
    {
        RpcChangePhysicsState(newState);
    }
    [ClientRpc]
    void RpcChangePhysicsState(bool newState)
    {
        m_itemRigidbody.isKinematic = !newState;
        m_itemCollider.enabled = newState;
    }

    public void SetFollowTarget(Transform objectToTarget)
    {
        needToFollow = true;
        p_objectToFollow = objectToTarget;
    }

    public void ClearFollowTarget()
    {
        CmdClearFollowTarget();
    }

    [Command(requiresAuthority = false)]
    void CmdClearFollowTarget()
    {
        RpcClearFollowTarget();
    }

    [ClientRpc]
    void RpcClearFollowTarget()
    {
        Transform t_objectToFollow = p_objectToFollow;

        needToFollow = false;
        p_objectToFollow = null;

        transform.position = t_objectToFollow.position;
        transform.rotation = t_objectToFollow.rotation;
    }
}
