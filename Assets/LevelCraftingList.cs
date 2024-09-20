using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCraftingList : MonoBehaviour
{
    public static LevelCraftingList instance {get; private set;}
    public List<CraftRecipeData> ListOfCraftinRecipes => m_listOfCraftRecipes;

    [SerializeField] private List<CraftRecipeData> m_listOfCraftRecipes = new List<CraftRecipeData>();


    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
}
