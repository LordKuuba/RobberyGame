using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CraftItem
{
    public ItemParameters m_itemParameter;
    public GameObject m_itemPrefab;
};

[CreateAssetMenu(fileName = "New Craft Recipe", menuName = "GameData/Craft Recipe")]
public class CraftRecipeData : ScriptableObject
{
    [SerializeField] private CraftItem m_itemToGive;
    [SerializeField] private List<ItemParameters> m_itemsToCraft;

    public CraftItem ItemToGet => m_itemToGive;
    public List<ItemParameters> ItemsToCraft => m_itemsToCraft;
}
