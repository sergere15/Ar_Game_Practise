using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeDescriptionStore : MonoBehaviour
{
    public Text description;
    private DataForGame data;
    // Start is called before the first frame update
    void Start()
    {
        data = GameObject.FindGameObjectWithTag("DataForGame").GetComponent<DataForGame>();
    }

    public void Change()
    {
         StartCoroutine(BuyingCoroutine(description.text));
    }

    private IEnumerator BuyingCoroutine(string text)
    {
        string url = "http://localhost:8080/store/seller/setDescription/"
            + data.loginUser + "/"
            + data.passwordUser + "/"
            + text;
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
