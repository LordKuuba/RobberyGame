using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactable : NetworkBehaviour
{
    [SerializeField] private GameObject m_interactionDialoguePopupPrefab;
    [SerializeField] private Vector3 m_interactionDialoguePopupPositionOffset;
    [SerializeField] private KeyCode m_keyToInteract;

    public bool IsInteractable => m_interactable;
    public KeyCode KeyToInteract => m_keyToInteract;

    private bool isInteractionDialogueShowing;

    InteractionDialogue m_interactionDialoguePopup;

    protected bool m_interactable = true;

    private void Start()
    {
        m_keyToInteract = KeyBinds.KeyToInteract;
    }

    public virtual void OnInteracted(Player g_playerWhoInteracted)
    {
        //Debug.Log($"Player {g_playerWhoInteracted.name} interacted with {this.name}");
    }

    public void DisplayKeyBind()
    {
        if(!isInteractionDialogueShowing && m_interactable)
        {
            GameObject spawnedPopup = Instantiate(m_interactionDialoguePopupPrefab);
            spawnedPopup.transform.position = this.transform.position + m_interactionDialoguePopupPositionOffset;

            m_interactionDialoguePopup = spawnedPopup.GetComponent<InteractionDialogue>();
            m_interactionDialoguePopup.Setup(KeyToInteract.ToString(), this.transform, m_interactionDialoguePopupPositionOffset);

            isInteractionDialogueShowing = true;

            //ChangeOutline
        }
    }

    public void DeleteKeyBindDisplay()
    {
        //Destroy and disable
        if (isInteractionDialogueShowing)
        {
            Destroy(m_interactionDialoguePopup.gameObject);
            m_interactionDialoguePopup = null;

            isInteractionDialogueShowing = false;
        }
    }
}
