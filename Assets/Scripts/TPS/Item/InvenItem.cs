using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class InvenItem : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public Item item;

    public Image icon;
    [SerializeField]
    Text itemName, amount;

    ItemDescription description;

    [SerializeField]
    bool isDropItem;

    public Inventory parentInven;
    // Start is called before the first frame update
    void Awake()
    {
        item = GetComponent<Item>();
        
    }

    public void InitItem(int id)
    {
        item.InitItem(id);

        icon.sprite = item.itemInfo.icon;
        itemName.text = item.itemInfo.name;
        amount.text = item.itemInfo.defaultAmount.ToString();

        description = GameObject.Find("아이템설명창루트").transform.GetChild(0).GetComponent<ItemDescription>();
    }


    public void changeAmount(int num)
    {
        item.itemInfo.amount = num;
        amount.text = item.itemInfo.amount.ToString();
    }

    public void AddItem(int num)
    {
        item.itemInfo.amount += num;
        amount.text = item.itemInfo.amount.ToString();
    }

    public void RemoveItem(int num)
    {
        item.itemInfo.amount -= num;
        amount.text = item.itemInfo.amount.ToString();
    }

    // 마우스 커서가 슬롯에 들어갈 때 발동
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null && description.gameObject.activeSelf==false)
            description.ActiveDescription(item);
    }

    // 마우스 커서가 슬롯에서 나올 때 발동
    public void OnPointerExit(PointerEventData eventData)
    {
        description.gameObject.SetActive(false);
    }

    public void ClickFunc()
    {
        if (isDropItem )
        {
            parentInven.GetItem(item.itemInfo.id, item.itemInfo.amount);
        }
        else
        {
            if (item.itemInfo.isUseAble == false)
                return;
            RemoveItem(1);
            parentInven.UseItem(this);
        }

    }

    private void OnDisable()
    {
        if(description!=null)
            description.gameObject.SetActive(false);
    }
}
