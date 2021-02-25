using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddItem : MonoBehaviour
{
    public Text name;
    public Text description;
    public Text cost;
    private DataForGame data;
    void Start()
    {
        data = GameObject.FindGameObjectWithTag("DataForGame").GetComponent<DataForGame>();
    }

    public void AddPressed()
    {
        Debug.Log("Add Pressed");
        int.Parse(cost.text);
        if (name.text.Length > 0 && description.text.Length > 0 && cost.text.Length > 0)
            StartCoroutine(AddItenCoroutine());
        GetComponentInParent<GameMenuControls>().UpdateItemSellerMenu();
    }

    private IEnumerator AddItenCoroutine()
    {
        string url = "http://localhost:8080/store/seller/addItem/"
            + data.loginUser + "/"
            + name.text + "/"
            + cost.text + "/"
            + description.text;
        var www = new WWW(url);
        while (!www.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (www.error == null)
        {
            Debug.Log(www.text);
        }
        else
        {
            Debug.Log(www.error);
        }
    }
}