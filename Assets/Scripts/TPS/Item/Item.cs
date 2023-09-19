using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public enum ItemType {Recover,Bullet, Parts };
    public enum PartsType { None, AR_Compensator, SR_Compensator, AR_Magazine, SR_Magazine, AR_Scope, SR_Scope };
    [Serializable]
    public struct ItemInfo
    {
        public int id;
        public string name;
        public string description;
        public int amount;
        public Sprite icon;
        public bool isUseAble;
        public int maxStackAble;
        public int defaultAmount;
        public ItemType itemType;
        public PartsType partsType;
        public float dropPercent;
    }

    public ItemInfo itemInfo;





    Dictionary<string, string> tableDic;
    // Start is called before the first frame update


    public void InitItem(int id,string dataFilePath = "ItemExcel")
    {
        // reader.ReaderTableAsExcel(out tableDic, dataPathExcel, tableNumber, level);
        ExcelReader.ReaderTable(out tableDic, dataFilePath, id);

        itemInfo.id = int.Parse(tableDic["id"]);
        itemInfo.name = tableDic["name"];
        itemInfo.defaultAmount = int.Parse(tableDic["amount"]);
        itemInfo.isUseAble = Convert.ToBoolean(tableDic["사용가능"]);
        itemInfo.maxStackAble = int.Parse(tableDic["최대소지"]);
        itemInfo.description = tableDic["설명"];

        string str = "DataFile/Excel/ItemIcon/" + itemInfo.name;
        itemInfo.icon = Resources.Load<Sprite>(str);

        itemInfo.amount = itemInfo.defaultAmount;

        itemInfo.itemType = (ItemType)Enum.Parse(typeof(ItemType), tableDic["타입"]);

        itemInfo.partsType = (PartsType)Enum.Parse(typeof(PartsType), tableDic["파츠"]);

        itemInfo.dropPercent = float.Parse(tableDic["드랍확율"]);
    }

    public static PartsType StringToEnum(string str)
    {
        return (PartsType)Enum.Parse(typeof(PartsType), str);
    }
}
