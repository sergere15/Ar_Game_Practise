using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
    private Text itemName;
    private Text itemDescription;
    private Text itemCost;
    private StoreController store;
    private JSONTemplate.Item item;

    public void Init(JSONTemplate.Item item, StoreController store)
    {
        this.store = store;
        this.item = item;
        itemName = transform.GetChild(0).GetComponent<Text>();
        itemDescription = transform.GetChild(1).GetComponent<Text>();
        itemCost = transform.GetChild(2).GetComponent<Text>();
        itemName.text = item.name;
        itemDescription.text = item.description;
        itemCost.text = item.cost.ToString();
        Debug.Log(item.name);
    }

    public void BuyPressed()
    {
        GetComponentInParent<GameMenuControls>().Buying(item);
        Debug.Log("Buy Pressed!");
    }
}
