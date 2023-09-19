using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDetection : MonoBehaviour
{
    Inventory itemDropinven;

    private void Start()
    {
        itemDropinven = GetComponent<Inventory>(); 
    }

    public void Insert(ItemBox itemBox)
    {
        
        foreach(var item in itemBox.itemContainer)
        {
            Debug.Log(item.Key);
            itemDropinven.InsertItem(item.Key, ItemDictionary.ALLItemDictionaryID[item.Key].itemInfo.defaultAmount * item.Value);
        }
    }
    public void Remove(ItemBox itemBox)
    {
        for(int i=1; i<= ItemDictionary.Instance.AllKindItem;i++)
        {
            if(itemBox.itemContainer.ContainsKey(i))
            {
                int id = i;
                int value = itemBox.itemContainer[i];
                itemDropinven.RemoveItem(id, ItemDictionary.ALLItemDictionaryID[id].itemInfo.defaultAmount * value, itemBox);

            }
        }

    }
}
