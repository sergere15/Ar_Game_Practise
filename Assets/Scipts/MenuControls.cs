using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControls : MonoBehaviour
{

    public Text login_enter;
    public Text password_enter;
    public Text login_registration;
    public Text password_registration;
    public Text Log_text;

    private DataForGame data;

    void Start()
    {
        data = GameObject.FindGameObjectWithTag("DataForGame").GetComponent<DataForGame>();
    }
    public void PlayPressed()
    {
        StartCoroutine(LoginRequest(login_enter.text, password_enter.text));
        Log_text.text = "Входим";
        Debug.Log("Login pressed!");
    }

    private IEnumerator LoginRequest(string log, string pas)
    {
        var url = "https://csc-2020-team-all-16.dmitrybarashev.repl.co/test_log?log=" +
            log + "&p=" +
            pas;
        var www = new WWW(url);
        while (!www.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (www.error == null)
        {
            if (www.text != "false")
            {
                data.id = int.Parse(www.text);
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            }
            else
            {
                Log_text.text = "Не удалось войти";
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
        Application.Quit();
    }
    public void RegPressed()
    {
        StartCoroutine(RegistrationRequest(login_registration.text, password_registration.text));
        Log_text.text = "Идет регистарция";
        //отправить Login.Text и Password.Text куда-то на сервер
        Debug.Log("Registration!");
    }

    private IEnumerator RegistrationRequest(string log, string pas)
    {
        //45,38.9   45.13,39.13
        var x = Random.Range(45f, 45.13f).ToString().Replace(',', '.');
        var y = Random.Range(38.9f, 38.13f).ToString().Replace(',', '.');
        var url = "https://csc-2020-team-all-16.dmitrybarashev.repl.co/test_reg?log=" +
             log +"&p=" + pas +"&x="+ x + "&y=" +y;
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

                Log_text.text = "Регистарция прошла успешно";
            }
            else
            {
                Log_text.text = "Стасян, ну ты конечно Дебил";
            }
        }
        else
        {
            Debug.Log(www.error);
        }
    }
}
