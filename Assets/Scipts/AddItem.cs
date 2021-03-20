using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using UnityEditor;
using System.IO;
using System;

public class AddItem : MonoBehaviour
{
    public Text name;
    public Text description;
    public Text cost;
    private DataForGame data;
    void Start()
    {
        data = GameObject.FindGameObjectWithTag("DataForGame").GetComponent<DataForGame>();
    }

    public void OpenFile()
    {
        var extensions = new[] {  //какие файлы вообще можно открыть
            new ExtensionFilter("prefab" ),
            new ExtensionFilter("All Files", "*" ),
        };
        foreach (string path in StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true))
        { //открытие формы для загрузки файла
            Debug.Log(path);
            var fileRead = new StreamReader(path);
            var text = fileRead.ReadToEnd();
            fileRead.Close();
            var file = new StreamWriter("obj.prefab");
            file.Write(text);
            //Debug.Log(www.text);
            file.Close();
        }
    }
    public void AddPressed()
    {
        if (!File.Exists("obj.prefab"))
            return;
        Debug.Log("Add Pressed");
        int.Parse(cost.text);
        if (name.text.Length > 0 && description.text.Length > 0 && cost.text.Length > 0)
        {
            StartCoroutine(AddItemCoroutine());
            StartCoroutine(UpLoadTestCoroutine());
        }
        GetComponentInParent<GameMenuControls>().UpdateItemSellerMenu();
    }

    private IEnumerator AddItemCoroutine()
    {
        string url = "http://localhost:8080/store/seller/addItem/"
            + data.loginUser + "/"
            + name.text + "/"
            + cost.text + "/"
            + description.text;
        var www = new WWW(url);
        while (!www.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log(url);
        if (www.error == null)
        {
            Debug.Log(www.text);
            var response = JsonUtility.FromJson<JSONTemplate.Response>(www.text);
            
        }
        else
        {
            Debug.Log(www.error);
        }
        
    }

    private IEnumerator UpLoadTestCoroutine()
    {
        Debug.Log("Addition");
        var bytes = default(byte[]);
        var file = new StreamReader("obj.prefab");
        var fileq = new StreamWriter("Assets/Resources/" + name.text + ".prefab");
        fileq.Write(file.ReadToEnd());
        file.Close();
        file = new StreamReader("obj.prefab");
        //Debug.Log(www.text);
        fileq.Close();
        using (var memstream = new MemoryStream())
        {
            file.BaseStream.CopyTo(memstream);
            bytes = memstream.ToArray();
        }
        file.Close();
        string url = "http://localhost:8080/store/upload/" + name.text + "/" + Convert.ToBase64String(bytes);
        Debug.Log(url);
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