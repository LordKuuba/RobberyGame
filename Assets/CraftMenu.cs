using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class CraftMenu : NetworkBehaviour
{
    [Header("Recipe Menu")]
    [SerializeField] private GameObject m_craftRecipeUIPrefab;
    [SerializeField] private Transform m_craftRecipesTransform;

    private List<CraftRecipeUi> m_listOfCraftRecipeUI = new List<CraftRecipeUi>();

    [SerializeField] private TextMeshProUGUI m_recipeCraftName;

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
    int m_selectedRecipeIndex = -1;

    private void Start()
    {
        m_levelCraftingList = LevelCraftingList.instance;

        for(int i = 0; i < m_levelCraftingList.ListOfCraftinRecipes.Count; i++)
        {
            GameObject spawnedPrefab = Instantiate(m_craftRecipeUIPrefab, m_craftRecipesTransform);
            CraftRecipeUi spawnedPrefabComponent = spawnedPrefab.GetComponent<CraftRecipeUi>();

            spawnedPrefabComponent.SetupInformation(
                i,
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

    public void UpdateCraftInformation(int m_craftRecipeIndex)
    {
        CmdSelectRecipeIndex(m_craftRecipeIndex);

        // Update local index to avoid waiting for network sync
        m_selectedRecipeIndex = m_craftRecipeIndex; // Local update

        // Continue with the rest of the logic
        int amountOfNeededItems = m_levelCraftingList.ListOfCraftinRecipes[m_selectedRecipeIndex].ItemsToCraft.Count;
        int slotCounter = amountOfNeededItems;

        m_selectedRecipe = m_levelCraftingList.ListOfCraftinRecipes[m_selectedRecipeIndex];

        m_recipeCraftName.text = m_levelCraftingList.ListOfCraftinRecipes[m_selectedRecipeIndex].ItemToGet.m_itemParameter.ItemName;

        for (int i = 0; i < m_listOfAllInputSlots.Count; i++)
        {
            if (slotCounter <= 0)
            {
                m_listOfAllInputSlots[i].gameObject.SetActive(false);
            }
            else
            {
                m_listOfAllInputSlots[i].gameObject.SetActive(true);
                m_listOfAllInputSlots[i].UpdateSlotImage(m_selectedRecipe.ItemsToCraft[i].ItemUISprite);
            }
            slotCounter -= 1;
        }

        m_outputItemSlot.UpdateSlotImage(m_selectedRecipe.ItemToGet.m_itemParameter.ItemUISprite);
    }

    [Command(requiresAuthority = false)]
    void CmdSelectRecipeIndex(int index)
    {
        m_selectedRecipeIndex = index;
    }

    public void Craft()
    {
        bool canCraft = false;

        if(m_selectedRecipeIndex != -1)
        {
            ItemParameters[] m_listToCheck = new ItemParameters[4];

            for(int i = 0; i < 4; i++)
            {
                if(i >= m_levelCraftingList.ListOfCraftinRecipes[m_selectedRecipeIndex].ItemsToCraft.Count)
                {
                    m_listToCheck[i] = null;
                }
                else
                {
                    m_listToCheck[i] = m_levelCraftingList.ListOfCraftinRecipes[m_selectedRecipeIndex].ItemsToCraft[i];
                }
            }

            canCraft = m_playersInventory.HaveItems(m_listToCheck);

            for (int i = 0; i < m_levelCraftingList.ListOfCraftinRecipes[m_selectedRecipeIndex].ItemsToCraft.Count; i++)
            {
                if (!m_playersInventory.ListOfAllItemParameters.Contains(m_levelCraftingList.ListOfCraftinRecipes[m_selectedRecipeIndex].ItemsToCraft[i]))
                {
                    canCraft = false;
                }
            }

            if (canCraft)
            {
                for (int i = 0; i < m_levelCraftingList.ListOfCraftinRecipes[m_selectedRecipeIndex].ItemsToCraft.Count; i++)
                {
                    m_playersInventory.FindAndDestroyItem(m_levelCraftingList.ListOfCraftinRecipes[m_selectedRecipeIndex].ItemsToCraft[i]);
                }

                
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
        print("Craft!");
        var spawnedItemPrefab = Instantiate(m_levelCraftingList.ListOfCraftinRecipes[m_selectedRecipeIndex].ItemToGet.m_itemPrefab);
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