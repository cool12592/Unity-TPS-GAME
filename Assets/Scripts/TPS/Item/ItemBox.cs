using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField]
    private List<int> keys = new List<int>();

    [SerializeField]
    private List<int> values = new List<int>();

    //아이템id 아이템 개수

    public Dictionary<int, int> itemContainer = new Dictionary<int, int>();
    [SerializeField]
    bool autoGenerate = true;
    // Start is called before the first frame update
    void Start()
    {
        if (autoGenerate == false)
        {
            if (keys.Count != values.Count)
                return;

            for(int i=0; i<keys.Count;i++)
            {
                itemContainer.Add(keys[i], values[i]);
            }
            return;
        }

        foreach(var item in ItemDictionary.ALLItemDictionaryName)
        {
            int id = item.Value.itemInfo.id;
            float rnd = Random.Range(0f, 100f);
            if(rnd<=item.Value.itemInfo.dropPercent)
            {
                if (itemContainer.ContainsKey(id))
                    itemContainer[id]++;
                else
                    itemContainer.Add(id, 1);
            }
        }
       

    }

   
}
