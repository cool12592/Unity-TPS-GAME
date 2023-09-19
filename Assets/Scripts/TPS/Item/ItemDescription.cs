using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemDescription : MonoBehaviour
{
    [SerializeField]
    Image icon;
    [SerializeField]
    Text itemName, description;

    RectTransform rect;
    private void Start()
    {
        rect = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    public void ActiveDescription(Item item)
    {

        icon.sprite = item.itemInfo.icon;
        itemName.text = item.itemInfo.name;
        description.text = item.itemInfo.description.ToString();
        gameObject.SetActive(true);
    }

    private void Update()
    {
        Update_MousePosition();
    }


    private void Update_MousePosition()
    {
        Vector2 mousePos = Input.mousePosition;

        rect.position = mousePos + new Vector2(150f,-150f);
    }


}
