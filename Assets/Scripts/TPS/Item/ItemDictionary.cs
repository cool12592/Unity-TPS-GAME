using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDictionary : MonoBehaviour
{
    private static ItemDictionary instance = null;

    public int AllKindItem;
    public static Dictionary<string, Item> ALLItemDictionaryName = new Dictionary<string, Item>();
    public static Dictionary<int, Item> ALLItemDictionaryID = new Dictionary<int, Item>();

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }


        ALLItemDictionaryName.Add("",new Item());


        for (int i = 1; i <= AllKindItem; i++)
        {
            Item item = new Item();
            item.InitItem(i);
            ALLItemDictionaryName.Add(item.itemInfo.name, item);
            ALLItemDictionaryID.Add(item.itemInfo.id, item);
        }
    }

    public static ItemDictionary Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
}
