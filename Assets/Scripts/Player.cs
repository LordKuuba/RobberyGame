using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [Space]
    [SerializeField] private PlayerMovement m_playerMovement;
    [SerializeField] private Inventory m_playerInventory;
    [SerializeField] private GameObject m_cameraPrefab;
    [SerializeField] private RotateToCursor m_playerRotation;
    [SerializeField] private KeyInteraction m_playerKeyInteraction;
    [SerializeField] private CraftMenu m_playerCraftMenu;

    [SerializeField] private Canvas m_playerCanvas;

    public Inventory PlayerInventory => m_playerInventory;
    public Canvas PlayerCanvas => m_playerCanvas;

    private CameraFollow p_playerCamera;

    private void Start()
    {
        if (isLocalPlayer)
        {
            GameObject playerCameraSpawned = Instantiate(m_cameraPrefab);
            p_playerCamera = playerCameraSpawned.GetComponent<CameraFollow>();

            p_playerCamera.SetTarget(this.transform);

            m_playerRotation.SetCamera(p_playerCamera.GetComponent<Camera>());

            m_playerMovement.ChangeInputState(true);
        }
        else if (!isLocalPlayer)
        {
            m_playerRotation.enabled = false;
            m_playerKeyInteraction.enabled = false;

            m_playerCanvas.gameObject.SetActive(false);
        }
    }

    public void ChangeMovementInputState(bool newState)
    {
        m_playerMovement.ChangeInputState(newState);
    }

    public void ChangeInteractingState(bool newState)
    {
        m_playerKeyInteraction.ChangeInteractingState(newState);
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            m_playerInventory.UpdateCurrentSlotInput();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                m_playerInventory.ReleaseCurrentlySelectedItem();
            }
        }
    }
}
