using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ARscript : MonoBehaviour
{

    private GameObject obj;
    private bool isLeftMouseButtonDown = false;
    private bool isRightMouseButtonDown = false;
    private Vector3 mousePos;
    // Start is called before the first frame update
    void Start()
    {
        var r = "obj";
        obj = GameObject.FindGameObjectWithTag("Object");
        obj.transform.position = new Vector3(10, 0, 0);
        obj.transform.localScale = new Vector3(1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
		{
            isLeftMouseButtonDown = true;
            mousePos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isLeftMouseButtonDown = false;
            mousePos = Input.mousePosition;
        }

        if (Input.GetMouseButtonDown(1))
        {
            isRightMouseButtonDown = true;
            mousePos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isRightMouseButtonDown = false;
        }

        if (isRightMouseButtonDown)
		{
            var pos = (mousePos - Input.mousePosition) * 0.0175f;
            Swap(ref pos.x, ref pos.z);
            pos.y *= -1;
            pos += obj.transform.position;
            mousePos = Input.mousePosition;
            pos.z = Mathf.Max(pos.z, -4);
            pos.z = Mathf.Min(pos.z, 4);
            pos.y = Mathf.Max(pos.y, -4);
            pos.y = Mathf.Min(pos.y, 4);

            obj.transform.position = pos;
        }

        if (isLeftMouseButtonDown)
        {
            var pos = mousePos - Input.mousePosition;
            mousePos = Input.mousePosition;
            Swap(ref pos.x, ref pos.y);
            Swap(ref pos.x, ref pos.z);
            pos.Normalize();
            pos *= 12;

            obj.transform.rotation = Quaternion.Euler(obj.transform.rotation.eulerAngles + pos);
        }

        if (Input.mouseScrollDelta.y  != 0)
        {
            obj.transform.localScale *= (Input.mouseScrollDelta.y + 8) / 8;
            if (obj.transform.localScale.x > 3)
                obj.transform.localScale = new Vector3(4, 4, 4);

            if (obj.transform.localScale.x < 0.3)
                obj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            Destroy(obj);
        }
    }

    private static void Swap<T>(ref T lhs, ref T rhs)
    {
        T temp;
        temp = lhs;
        lhs = rhs;
        rhs = temp;
    }
}
