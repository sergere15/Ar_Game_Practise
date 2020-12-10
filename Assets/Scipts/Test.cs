using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject target;
    void Start()
    {
        //var o = Instantiate(target, new Vector3(0, 12, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
}
