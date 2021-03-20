using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        item.description = item.description.Replace("_", " ");
        this.item = item;

        itemName = transform.GetChild(0).GetComponent<Text>();
        itemDescription = transform.GetChild(1).GetComponent<Text>();
        itemCost = transform.GetChild(2).GetComponent<Text>();
        itemName.text = item.name;
        itemDescription.text = item.description;
        itemCost.text = item.cost.ToString();
    }

    public void BuyPressed()
    {
        GetComponentInParent<GameMenuControls>().Buying(item);
        Debug.Log("Buy Pressed!");
    }

    public void DeletePressed()
    {
        Debug.Log("Delete Pressed!");
        StartCoroutine(GetComponentInParent<GameMenuControls>().DeleteItem(gameObject, item));
    }

    public void OpenItem()
    {
        StartCoroutine(OpenItemCoroutine());
    }

    private IEnumerator OpenItemCoroutine()
    {
        string url = "http://localhost:8080/store/files/" + item.name;
        var www = new WWW(url);
        Debug.Log(url);
        while (!www.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (www.error == null)
        {
            var file = new StreamWriter("Assets/Resources/" + "obj" + ".prefab");
            file.Write(www.text);
            //Debug.Log(www.text);
            file.Close();
            var r = item.name;
            var obj = Resources.Load<GameObject>(r);
            //GameObject instance = Instantiate(Resources.Load(r, typeof(GameObject))) as GameObject;
            //instance.transform.position = new Vector3(0, 0, 0);
            if (obj == null)
                Debug.Log("null");
            else
            {
                obj = Instantiate(obj);
                obj.tag = "Object";
                DontDestroyOnLoad(Instantiate(obj));
                SceneManager.LoadScene("CameraScene", LoadSceneMode.Single);
            }
        }
        else
        {
            Debug.Log(www.error);
        }
    }
}
