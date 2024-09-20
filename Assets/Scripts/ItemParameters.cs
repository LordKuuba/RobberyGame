using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Parameters", menuName = "GameData/Items")]
public class ItemParameters : ScriptableObject
{
    [SerializeField] private int m_itemId = default;
    [SerializeField] private string m_itemName = default;

    [SerializeField] private Sprite m_itemUISprite = default;

    public int ItemId => m_itemId;
    public string ItemName => m_itemName;
    public Sprite ItemUISprite => m_itemUISprite;
}
