using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUISlot : MonoBehaviour
{
    [SerializeField] private Image m_bgImage;
    [SerializeField] private Image m_itemImage;

    [SerializeField] private Color t_selectedColor;
    [SerializeField] private Color t_unselectedColor;

    private bool isSelected;

    public void ChangeUsingState(bool newState)
    {
        isSelected = newState;

        m_bgImage.color = isSelected ? t_selectedColor : t_unselectedColor;
    }

    public void UpdateSlotImage(Sprite m_imageToSet)
    {
        if(m_imageToSet == null)
        {
            m_itemImage.gameObject.SetActive(false);
            m_itemImage.sprite = null;
        }
        else
        {
            m_itemImage.gameObject.SetActive(true);
            m_itemImage.sprite = m_imageToSet;
        }
    }
}
