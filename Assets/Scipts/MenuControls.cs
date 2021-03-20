using System.Collections;
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
    //private GameObject obj;

    void Start()
    {
        data = GameObject.FindGameObjectWithTag("DataForGame").GetComponent<DataForGame>();
        data.isCustomer = true;
    }

    public void SetUserType(bool isCustomer)
	{
        data.isCustomer = isCustomer;
        Debug.Log(data.isCustomer);
    }
    public void PlayPressed()
    {
        StartCoroutine(LoginRequest(login_enter.text, password_enter.text));
        Log_text.text = "Входим";
        Debug.Log("Login pressed!");
    }

    private IEnumerator LoginRequest(string log, string pas)
    {
        string url;
        if (data.isCustomer)
            url = "http://localhost:8080/store/customerSignIn/";
        else
            url = "http://localhost:8080/store/sellerSignIn/";
        url = url + log + "/" + pas;
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
                data.loginUser = log;
                data.passwordUser = pas;
                data.amount = response.value;
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
        string url;
        if (data.isCustomer)
            url = "http://localhost:8080/store/customerSignUp/";
        else
            url = "http://localhost:8080/store/sellerSignUp/";
        url = url + log + "/" + pas;
        var www = new WWW(url);
        Debug.Log(url);
        while (!www.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (www.error == null)
        {
            Debug.Log(www.text);
            var myObject = JsonUtility.FromJson<JSONTemplate.Response>(www.text);
            
            if (myObject.code == 0)
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
