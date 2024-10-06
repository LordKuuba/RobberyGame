using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class CraftRecipeUi : MonoBehaviour
{
    [SerializeField] private Image m_craftRecipeImage;
    [SerializeField] private TextMeshProUGUI m_craftRecipeName;

    [SerializeField] private Button m_craftRecipeButton;

    private int m_craftingDataIndex = 0;

    private System.Action<int> m_onClickCallback;

    LevelCraftingList m_levelCraftingList;

    private void Awake()
    {
        m_levelCraftingList = LevelCraftingList.instance;
    }

    public void SetupInformation(int g_craftRecipeIndex, System.Action<int> g_onClickCallback)
    {
        m_craftingDataIndex = g_craftRecipeIndex;
        //Debug.Log(m_craftingDataIndex);

        m_craftRecipeImage.sprite = m_levelCraftingList.ListOfCraftinRecipes[g_craftRecipeIndex].ItemToGet.m_itemParameter.ItemUISprite;
        m_craftRecipeName.text = m_levelCraftingList.ListOfCraftinRecipes[g_craftRecipeIndex].ItemToGet.m_itemParameter.ItemName;

        m_onClickCallback = g_onClickCallback;

        m_craftRecipeButton.onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        if(m_onClickCallback != null)
        {
            m_onClickCallback.Invoke(m_craftingDataIndex);
        }
    }
}
