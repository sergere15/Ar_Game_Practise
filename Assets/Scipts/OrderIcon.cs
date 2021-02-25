using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderIcon : MonoBehaviour
{
    private Text itemName;
    private Text addres;
    private StoreController store;
    private JSONTemplate.Order order;
    public void Init(JSONTemplate.Order order, StoreController store)
    {
        this.store = store;
        this.order = order;
        itemName = transform.GetChild(0).GetComponent<Text>();
        addres = transform.GetChild(1).GetComponent<Text>();
        itemName.text = order.item;
        addres.text = order.addres;
        Debug.Log(order.id);
    }

    public void ClosePressed()
    {
        Debug.Log("Close Pressed!");
        StartCoroutine(GetComponentInParent<GameMenuControls>().CloseOrder(gameObject, order));
    }
}
