using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuControls : MonoBehaviour
{
    public Text X;
    public Text Y;
    public Text add_char_log;
    public GameObject Map;

    private int my_id;

    void Start()
    {
        my_id = GameObject.FindGameObjectWithTag("DataForGame").GetComponent<DataForGame>().id;
    }

    public void AddPressed()
    {
    // 
        Debug.Log("Add pressed!");
        Debug.Log(X.text);
        Debug.Log(Y.text);
        StartCoroutine(AddCharacterRequest(X.text, Y.text));
        add_char_log.text = "Добавляем персонажа";
    }

    private IEnumerator AddCharacterRequest(string x, string y)
	{
        // 45.0816099,38.9525191
        var url = "https://csc-2020-team-all-16.dmitrybarashev.repl.co/test_add_target?x=" +
            x + "&y=" + y;
        var www = new WWW(url);
        Debug.Log(url);
        while (!www.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (www.error == null)
        {
            Debug.Log(www.text);
            if (www.text == "true")
            {

                add_char_log.text = "Персонаж успешно добавлен";
                StartCoroutine(Map.GetComponent<LoadMap>().LoadObjects());
            }
            else
            {
                add_char_log.text = "Стасян, ну ты конечно Дебил";
            }
        }
        else
        {
            Debug.Log(www.error);
        }
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
        // 45.0816099,38.9525191
        var url = "https://csc-2020-team-all-16.dmitrybarashev.repl.co/test_del_user?id=" + my_id;
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
}
