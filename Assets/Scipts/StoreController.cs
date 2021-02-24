using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreController : MonoBehaviour
{
    public GameObject prefab;
    public int count;
    // Start is called before the first frame update
    void Start()
    {
		for (int i = 0; i < count; ++i)
			Instantiate(prefab, this.transform);
	}

    public IEnumerator InitStoreList()
    {
        
        string url = "http://localhost:8080/store/getSellers";
        var www = new WWW(url);
        while (!www.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (www.error == null)
        {
            string str = ',' + www.text.Substring(1, www.text.Length - 2);
            Debug.Log(str);

            var response = str.Split('}');
            foreach (string sellerStr in response)
            {
                if (sellerStr.Length < 2)
                    continue;
                string json = sellerStr.Substring(1) + '}';
                Debug.Log(json);
                var seller = JsonUtility.FromJson<JSONTemplate.Seller>(json);
                var obj = Instantiate(prefab, this.transform);
                obj.GetComponent<StoreIcon>().Init(seller, this);
            }
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    public IEnumerator InitItemList(string login)
    {
        string url = "http://localhost:8080/store/getItems/" + login;
        var www = new WWW(url);
        while (!www.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (www.error == null)
        {
            string str = ',' + www.text.Substring(1, www.text.Length - 2);
            Debug.Log(str);

            var response = str.Split('}');
            foreach (string sellerStr in response)
            {
                if (sellerStr.Length < 2)
                    continue;
                string json = sellerStr.Substring(1) + '}';
                Debug.Log(json);
                var item = JsonUtility.FromJson<JSONTemplate.Item>(json);
                var obj = Instantiate(prefab, transform);
                obj.GetComponent<ItemIcon>().Init(item, this);
            }
        }
        else
        {
            Debug.Log(www.error);
        }
    }
}
