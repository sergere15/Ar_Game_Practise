using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMenu : MonoBehaviour
{
	public void BackToMenuPressed()
	{
		Debug.Log("@@@@@@@");
		SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
	}
}
