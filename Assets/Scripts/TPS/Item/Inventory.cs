using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Inventory : MonoBehaviour
{
   // Dictionary<int, int> invenMap;
    SortedDictionary<int, List<InvenItem>> invenContainer;
    ScrollRect scroll;

    ObjectPool itemPool;

    [SerializeField]
    PlayerController2 pc;


    // Start is called before the first frame update
    void Start()
    {
        scroll = GetComponent<ScrollRect>();
        itemPool = GetComponent<ObjectPool>();

        invenContainer = new SortedDictionary<int, List<InvenItem>>();
        //invenMap = new Dictionary<int, int>();
    }

    public void ClearInventory()
    {
        foreach(var itemList in invenContainer)
        {
            foreach(var invenItem in itemList.Value)
            {
                itemPool.ReturnObject(invenItem.gameObject);
            }
        }
        invenContainer.Clear();
    }

    public void GetItem(int id, int num)
    {
        RemoveItem(id, num);
        pc.inven.InsertItem(id, num);

        pc.playerAttack.playerWeapon.nowWeapon_.ChangeRemainBullet(); 
    }

   

    public void InsertItem(int id, int addAmount)
    {
        if (addAmount == 0)
            return;

        if (invenContainer.ContainsKey(id))
        {

            int lastInd = invenContainer[id].Count - 1;
            if (lastInd == -1)
                Debug.Log("머여");
            int maxCount = invenContainer[id][lastInd].item.itemInfo.maxStackAble;

            for (int i = lastInd; 0 <= i; i--)
            {
                int amount = invenContainer[id][i].item.itemInfo.amount;

                if (maxCount < amount + addAmount)
                {
                    invenContainer[id][i].changeAmount(maxCount);
                    addAmount = addAmount - (maxCount - amount);

                }
                else
                {
                    invenContainer[id][i].changeAmount(amount + addAmount);
                    return;
                }
            }
        }

        CreateItem(id, addAmount);
        SortInventory();
    }

    public void RemoveItem(int id, int num, ItemBox itemBox = null)
    {
        if (invenContainer.ContainsKey(id) == false)
        {
            if(itemBox!=null)
            {
                itemBox.itemContainer[id] = 0;
            }
            return;
        }

        

        while(0 < invenContainer[id].Count)
        {
            int len = invenContainer[id].Count - 1;

            var invenItem = invenContainer[id][len];
            int amount = invenItem.item.itemInfo.amount;

            if(num <= amount)
            {
                invenItem.changeAmount(amount - num);
                if(invenItem.item.itemInfo.amount==0)
                {
                    itemPool.ReturnObject(invenItem.gameObject);
                    invenContainer[id].Remove(invenItem);
                }

                break;
            }
            else
            {
                itemPool.ReturnObject(invenItem.gameObject);
                invenContainer[id].Remove(invenItem);
                num -= amount;
            }
        }

        if (invenContainer[id].Count == 0)
            invenContainer.Remove(id);
    }

    public void CreateItem(int id,int num)
    {
        if (num <= 0)
            return;

        var invenItem = itemPool.GetObject().GetComponent<InvenItem>();
        invenItem.InitItem(id);
        invenItem.parentInven = this;
        invenItem.transform.parent = scroll.content;
        invenItem.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

        if (invenContainer.ContainsKey(id) == false)
            invenContainer[id] = new List<InvenItem>();

        invenContainer[id].Add(invenItem);

        //invenItem.myIndex = invenContainer[id].Count - 1;

        int maxCount = invenItem.item.itemInfo.maxStackAble;
        if (maxCount < num)
        {
            CreateItem(id, num - maxCount);
            num = maxCount;

        }

        invenItem.changeAmount(num);


        
    }

    void SortInventory()
    {
        foreach(var itemMap in invenContainer)
        {
            int len = itemMap.Value.Count-1;
            for(int i = len; 0 <= i; i--)
            {
                itemMap.Value[i].transform.SetAsLastSibling();
            }
        }
    }

    public void UseItem(InvenItem invenItem)
    {
        string itemName = invenItem.item.itemInfo.name;
        int id = invenItem.item.itemInfo.id;

       
        switch (itemName)
        {
            case "구급상자":
                pc.GetComponent<PlayerHealth>().HealHP(40f);
                break;
            case "약":
                pc.GetComponent<PlayerHealth>().HealEnergy(0.4f);
                break;
            case "백신":
                pc.GetComponent<PlayerHealth>().HealEnergy(0.6f);
                break;
            default:
                break;
        }

        if (invenItem.item.itemInfo.itemType == Item.ItemType.Parts)
            UseParts(invenItem);


        if (invenItem.item.itemInfo.amount == 0)
        {
            invenContainer[id].Remove(invenItem);
            if (invenContainer[id].Count == 0)
                invenContainer.Remove(id);

            itemPool.ReturnObject(invenItem.gameObject);
        }

    }

    void UseParts(InvenItem invenItem)
    {
        pc.playerAttack.weaponParts.UnEquipParts(invenItem.item.itemInfo.partsType);
        pc.playerAttack.weaponParts.EquipParts(invenItem);
    }

    public int ChargeBullet(int bulletId,int nowBulletAmount,int maxBulletAmount)
    {
        int res = 0;
        int needBullet = maxBulletAmount;

        needBullet = needBullet - nowBulletAmount;
        if (needBullet <= 0)
            return 0;

        while(0 < needBullet && invenContainer.ContainsKey(bulletId))
        {
            int len = invenContainer[bulletId].Count - 1;
            int amount = invenContainer[bulletId][len].item.itemInfo.amount;

            if (amount < needBullet)
            {
                RemoveItem(bulletId, amount);
                res += amount;
                needBullet -= amount;

            }
            else
            {
                RemoveItem(bulletId, needBullet);
                res += needBullet;
                needBullet -= needBullet;

            }


        }

        return res;
    }

    public int GetInvenRemainBullet(int bulletId)
    {
        int res = 0;

        if (invenContainer.ContainsKey(bulletId) == false)
            return 0;

        foreach(var invenItem in invenContainer[bulletId])
        {
            res += invenItem.item.itemInfo.amount;
        }

        return res;
    }

     
}
