using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CraftMenu : NetworkBehaviour
{
    [Header("Recipe Menu")]
    [SerializeField] private GameObject m_craftRecipeUIPrefab;
    [SerializeField] private Transform m_craftRecipesTransform;

    private List<CraftRecipeUi> m_listOfCraftRecipeUI = new List<CraftRecipeUi>();

    LevelCraftingList m_levelCraftingList;

    [Space]
    [Header("Interactable Menu")]
    [SerializeField] private GameObject m_displaySlotPrefab;
    [SerializeField] private Transform m_inputIngridientsParent;

    [SerializeField] private InventoryUISlot m_outputItemSlot;

    private List<InventoryUISlot> m_listOfAllInputSlots = new List<InventoryUISlot>();
    private int maxAmountOfIngridients = 0;

    [SerializeField] private Inventory m_playersInventory;

    private CraftRecipeData m_selectedRecipe;

    [SyncVar]
    private GameObject m_objectToSpawn;

    private void Start()
    {
        m_levelCraftingList = LevelCraftingList.instance;

        for(int i = 0; i < m_levelCraftingList.ListOfCraftinRecipes.Count; i++)
        {
            GameObject spawnedPrefab = Instantiate(m_craftRecipeUIPrefab, m_craftRecipesTransform);
            CraftRecipeUi spawnedPrefabComponent = spawnedPrefab.GetComponent<CraftRecipeUi>();

            spawnedPrefabComponent.SetupInformation(
                m_levelCraftingList.ListOfCraftinRecipes[i],
                UpdateCraftInformation
            );

            m_listOfCraftRecipeUI.Add(spawnedPrefabComponent);

            if (maxAmountOfIngridients < m_levelCraftingList.ListOfCraftinRecipes[i].ItemsToCraft.Count)
            {
                maxAmountOfIngridients = m_levelCraftingList.ListOfCraftinRecipes[i].ItemsToCraft.Count;
            }
        }


        for(int i = 0; i < maxAmountOfIngridients; i++)
        {
            GameObject spawnedPrefab = Instantiate(m_displaySlotPrefab, m_inputIngridientsParent);
            spawnedPrefab.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            m_listOfAllInputSlots.Add(spawnedPrefab.GetComponent<InventoryUISlot>());
        }
    }

    [Command(requiresAuthority = false)]
    public void UpdateCraftInformation(CraftRecipeData m_dataToVisualize)
    {
        int amountOfNeededItems = m_dataToVisualize.ItemsToCraft.Count;
        int slotCounter = amountOfNeededItems;

        m_selectedRecipe = m_dataToVisualize;

        m_objectToSpawn = m_selectedRecipe.ItemToGet.m_itemPrefab;

        for (int i = 0; i < m_listOfAllInputSlots.Count; i++)
        {
            if(slotCounter <= 0)
            {
                m_listOfAllInputSlots[i].gameObject.SetActive(false);
            }
            else
            {
                m_listOfAllInputSlots[i].gameObject.SetActive(true);
                m_listOfAllInputSlots[i].UpdateSlotImage(m_dataToVisualize.ItemsToCraft[i].ItemUISprite);
            }
            slotCounter -= 1;
        }

        m_outputItemSlot.UpdateSlotImage(m_dataToVisualize.ItemToGet.m_itemParameter.ItemUISprite);
    }

    public void Craft()
    {
        bool canCraft = true;

        if( m_selectedRecipe != null)
        {
            for (int i = 0; i < m_selectedRecipe.ItemsToCraft.Count; i++)
            {
                //Check if can craft
            }

            if (canCraft)
            {
                for (int i = 0; i < m_playersInventory.ListOfAllItems.Count; i++)
                {                       
                    if (m_playersInventory.ListOfAllItems[i] == null)
                    {
                        SpawnItem(m_playersInventory, i);
                        return;
                    }
                }
            }
        }
    }
    [Command(requiresAuthority = false)]
    void SpawnItem(Inventory g_inventory, int g_slotIndex)
    {
        var spawnedItemPrefab = Instantiate(m_objectToSpawn);
        spawnedItemPrefab.transform.position = g_inventory.PlayerHand.position;

        NetworkServer.Spawn(spawnedItemPrefab, connectionToClient);

        RpcSpawnItem(g_inventory, spawnedItemPrefab.GetComponent<Item>(), g_slotIndex);
    }

    [ClientRpc]
    void RpcSpawnItem(Inventory g_inventory, Item spawnedItem, int g_slotIndex)
    {
        g_inventory.AddItemToSlot(spawnedItem, g_slotIndex);
    }

    public void SetPlayer(Inventory g_inventory)
    {
        m_playersInventory = g_inventory;
    }
}