using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class CraftRecipeUi : MonoBehaviour
{
    [SerializeField] private Image m_craftRecipeImage;
    [SerializeField] private TextMeshProUGUI m_craftRecipeName;

    [SerializeField] private Button m_craftRecipeButton;

    private CraftRecipeData m_craftingData;

    private System.Action<CraftRecipeData> m_onClickCallback;

    public void SetupInformation(CraftRecipeData g_craftRecipe, System.Action<CraftRecipeData> g_onClickCallback)
    {
        m_craftingData = g_craftRecipe;

        m_craftRecipeImage.sprite = m_craftingData.ItemToGet.m_itemParameter.ItemUISprite;
        m_craftRecipeName.text = m_craftingData.ItemToGet.m_itemParameter.ItemName;

        m_onClickCallback = g_onClickCallback;

        m_craftRecipeButton.onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        if(m_onClickCallback != null)
        {
            m_onClickCallback.Invoke(m_craftingData);
        }
    }
}
