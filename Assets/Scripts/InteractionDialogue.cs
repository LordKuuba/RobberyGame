using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionDialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_interactionKeyText;
    [SerializeField] private Vector3 m_followObjectPositionOffset;

    private string p_interactionText;

    private Transform p_objectToFollow;

    private void Update()
    {
        if (p_objectToFollow != null)
        {
            Vector3 targetPosition = p_objectToFollow.position + m_followObjectPositionOffset;

            // Lerp the camera's position towards the target position
            transform.position = targetPosition;
        }
    }

    public void Setup(string textToDisplay, Transform objectToFollow, Vector3 objectFollowOffset)
    {
        p_interactionText = textToDisplay;
        m_interactionKeyText.text = p_interactionText;

        p_objectToFollow = objectToFollow;

        m_followObjectPositionOffset = objectFollowOffset;
    }
}
