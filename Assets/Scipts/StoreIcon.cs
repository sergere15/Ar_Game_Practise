using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreIcon : MonoBehaviour
{
    private Text storeName;
    private Text storeDescription;
    private StoreController storeMenu;

	public void Init(JSONTemplate.Seller seller, StoreController store)
	{
        this.storeMenu = store;
        storeName = transform.GetChild(0).GetComponent<Text>();
        storeDescription = transform.GetChild(1).GetComponent<Text>();
        storeName.text = seller.login;
        storeDescription.text = seller.description;
        Debug.Log(seller.login);
    }

	public void OpenStorePressed()
    {
        GetComponentInParent<GameMenuControls>().InitItemList(storeName.text);
        Debug.Log("Open Store Pressed!");
    }
}
