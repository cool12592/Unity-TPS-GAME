using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TPS_WeaponParts : MonoBehaviour
{
    [HideInInspector] public PlayerController2 pc;

    [Header("파츠 이미지")]
    [SerializeField]
    Image ARcompensator_Image;
    [SerializeField]
    Image SRcompensator_Image, ARmagazine_Image, SRmagazine_Image, ARscope_Image, SRscope_Image;

    [HideInInspector]
    public bool isCompensator_AR, isCompensator_SR, isLargetCapacity_AR, isLargetCapacity_SR, isSpecialScope_AR, isSpecialScope_SR;
    public bool isEquipScope_AR, isEquipScope_SR;
    Dictionary<Item.PartsType, ItemParts> NowEquipParts = new Dictionary<Item.PartsType, ItemParts>();

    class ItemParts
    {
        public ItemParts(Image partsImage_, string partsName_ = "", int itemId_ = 0)
        {
            partsImage = partsImage_;
            partsName = partsName_;
            itemId = itemId_; ;
        }

        public Image partsImage;
        public string partsName;
        public int itemId;
    }


    private void Awake()
    {
        pc = GetComponent<PlayerController2>();

        NowEquipParts.Add(Item.PartsType.AR_Magazine, new ItemParts(ARmagazine_Image));
        NowEquipParts.Add(Item.PartsType.AR_Compensator, new ItemParts(ARcompensator_Image));
        NowEquipParts.Add(Item.PartsType.AR_Scope, new ItemParts(ARscope_Image));

        NowEquipParts.Add(Item.PartsType.SR_Magazine, new ItemParts(SRmagazine_Image));
        NowEquipParts.Add(Item.PartsType.SR_Compensator, new ItemParts(SRcompensator_Image));
        NowEquipParts.Add(Item.PartsType.SR_Scope, new ItemParts(SRscope_Image));
    }

    public void InitParts()
    {
        foreach(var parts in NowEquipParts)
        {
            UnEquipParts(parts.Key);
        }

        CreateARSopeParts();
        CreateSRSopeParts();
    }


    public void EquipParts(InvenItem invenItem)
    {
        var partsType = invenItem.item.itemInfo.partsType;
        var partsSprite = invenItem.item.itemInfo.icon;
        var partsName = invenItem.item.itemInfo.name;
        var itemId = invenItem.item.itemInfo.id;

        var itemParts = NowEquipParts[partsType];
        itemParts.partsImage.sprite = partsSprite;
        itemParts.partsImage.gameObject.SetActive(true);
        itemParts.partsName = partsName;
        itemParts.itemId = itemId;

        if (partsType == Item.PartsType.AR_Scope)
            isEquipScope_AR = true;
        else if (partsType == Item.PartsType.SR_Scope)
            isEquipScope_SR = true;

        switch (partsName)
        {
            case "AR보정기":
                isCompensator_AR = true;
                break;
            case "SR보정기":
                isCompensator_SR = true;
                break;
            case "AR대용량탄창":
                isLargetCapacity_AR = true;
                break;
            case "SR대용량탄창":
                isLargetCapacity_SR = true;
                break;
            case "AR열화상스코프":
                isSpecialScope_AR = true;
                break;
            case "SR열화상스코프":
                isSpecialScope_SR = true;
                break;

            case "AR스코프":
                break;

            case "SR스코프":
                break;

            case "전술조준경":
                pc.playerAttack.tacticalVisor.SetTacticalVisor(true);
                break;
            default:
                break;
        }
    }

    public void UnEquipPartsAsString(string partsName)
    {
        var itemType = Item.StringToEnum(partsName);
        UnEquipParts(itemType);
    }

    public void UnEquipParts(Item.PartsType partsType)
    {
        ItemParts parts = NowEquipParts[partsType];
        if (parts.partsName == "")
            return;
        parts.partsImage.gameObject.SetActive(false);
        pc.inven.InsertItem(parts.itemId, 1);

        if (partsType == Item.PartsType.AR_Scope)
            isEquipScope_AR = false;
        else if (partsType == Item.PartsType.SR_Scope)
            isEquipScope_SR = false;

        UnEquipPartsAsName(parts.partsName);

        NowEquipParts[partsType].partsName = "";
        NowEquipParts[partsType].itemId = 0;

    }

    void UnEquipPartsAsName(string partsName)
    {
        switch (partsName)
        {
            case "AR보정기":
                isCompensator_AR = false;
                break;

            case "SR보정기":
                isCompensator_SR = false;
                break;

            case "AR대용량탄창":
                isLargetCapacity_AR = false;
                break;

            case "SR대용량탄창":
                isLargetCapacity_SR = false;
                break;


            case "AR열화상스코프":
                isSpecialScope_AR = false;
                break;

            case "SR열화상스코프":
                isSpecialScope_SR = false;
                break;

            case "전술조준경":
                pc.playerAttack.tacticalVisor.SetTacticalVisor(false);
                break;

            default:
                break;

        }

    }

    public void CreateARSopeParts()
    {
        var parts = NowEquipParts[Item.PartsType.AR_Scope];
        parts.partsImage.sprite = ItemDictionary.ALLItemDictionaryName["AR스코프"].itemInfo.icon;
        parts.partsName = "AR스코프";
        parts.itemId = 10;
        isEquipScope_AR = true;

        parts.partsImage.gameObject.SetActive(true);
    }

    public void CreateSRSopeParts()
    {
        var parts = NowEquipParts[Item.PartsType.SR_Scope];
        parts.partsImage.sprite = ItemDictionary.ALLItemDictionaryName["SR스코프"].itemInfo.icon;
        parts.partsName = "SR스코프";
        parts.itemId = 12;
        isEquipScope_SR = true;

        parts.partsImage.gameObject.SetActive(true);
    }
}
