using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuControls : MonoBehaviour
{
    public Text add_char_log;
    public Text balance;
    private DataForGame data;
    public GameObject storeMenu;
    public GameObject itemMenu;


    void Start()
    {
        data = GameObject.FindGameObjectWithTag("DataForGame").GetComponent<DataForGame>();
        StartCoroutine(storeMenu.GetComponentInChildren<StoreController>().InitStoreList());
        Debug.Log(transform.childCount);
        balance.text = data.amount.ToString();
    }

    public void ExitPressed()
    {
        Debug.Log("Exit pressed!");
        Destroy(GameObject.FindGameObjectWithTag("DataForGame"));
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void DeleteAccountPressed()
    {
        StartCoroutine(DeleteAccountRequest());
        return;
        Debug.Log("Exit pressed!");
        Destroy(GameObject.FindGameObjectWithTag("DataForGame"));
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    private IEnumerator DeleteAccountRequest()
    {
        string url;
        if (data.isCustomer)
            url = "http://localhost:8080/store/deleteCustomer/";
        else
            url = "http://localhost:8080/store/deleteSeller/";
        url = url + data.loginUser + "/" + data.passwordUser;
        var www = new WWW(url);
        while (!www.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (www.error == null)
        {
            Debug.Log(www.text);

            Debug.Log("Account deleted!");
            Destroy(GameObject.FindGameObjectWithTag("DataForGame"));
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    public IEnumerator DeleteItem(GameObject itemNote, JSONTemplate.Item item)
    {
        string url = "http://localhost:8080/store/deleteItem/"
            + data.loginUser + "/"
            + data.passwordUser + "/"
            + item.id.ToString();
        var www = new WWW(url);
        while (!www.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (www.error == null)
        {
            Debug.Log(www.text);
            Debug.Log("item deleted!");
            Destroy(itemNote);
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    public void InitItemList(string login)
	{
        storeMenu.SetActive(false);
        itemMenu.SetActive(true);
        data.loginStore = login;
        StartCoroutine(itemMenu.GetComponentInChildren<StoreController>().InitItemList(login));
    }
    public IEnumerator UpdateBalanceCoroutine()
    {
        string url;
        if (data.isCustomer)
            url = "http://localhost:8080/store/customerSignIn/";
        else
            url = "http://localhost:8080/store/sellerSignIn/";
        url = url + data.loginUser + "/" + data.passwordUser;
        var www = new WWW(url);
        while (!www.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (www.error == null)
        {
            Debug.Log(www.text);
            var response = JsonUtility.FromJson<JSONTemplate.Response>(www.text);
            if (response.code == 0)
            {
                balance.text = response.value.ToString();
            }
            else
            {
                balance.text = "Unknown";
            }
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    public void Buying(JSONTemplate.Item item)
    {
        if (data.amount >= item.cost)
            StartCoroutine(BuyingCoroutine(item));
    }

    private IEnumerator BuyingCoroutine(JSONTemplate.Item item)
    {
        string url = "http://localhost:8080/store/buying/"
            + data.loginUser + "/"
            + data.loginStore + "/"
            + item.id.ToString() + "/"
            + data.loginUser;
        var www = new WWW(url);
        while (!www.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (www.error == null)
        {
            Debug.Log(www.text);
            var response = JsonUtility.FromJson<JSONTemplate.Response>(www.text);
            if (response.code == 0)
            {
                data.amount -= item.cost;
                balance.text = data.amount.ToString();
            }
            else
            {
                balance.text = "Unknown";
            }
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    public void AddMoney()
    {
        StartCoroutine(AddMoneyCoroutine());
    }

    private IEnumerator AddMoneyCoroutine()
    {
        int value = 10;
        string url = "http://localhost:8080/store/customer/addAmount/"
            + data.loginUser + "/"
            + value.ToString();
        var www = new WWW(url);
        while (!www.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (www.error == null)
        {
            Debug.Log(www.text);
            var response = JsonUtility.FromJson<JSONTemplate.Response>(www.text);
            if (response.code == 0)
            {
                data.amount += value;
                balance.text = data.amount.ToString();
            }
            else
            {
                balance.text = "Unknown";
            }
        }
        else
        {
            Debug.Log(www.error);
        }
    }
}
