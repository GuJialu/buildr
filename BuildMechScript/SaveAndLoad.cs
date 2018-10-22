using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShipData
{
    public string shipName;
    public List<CubeData> cubeDatas;
}
[System.Serializable]
public struct CubeData
{
    public string cubeName;
    public string materialName;
    public TransformData transformDatas;
    
}
[System.Serializable]
public struct TransformData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}

public class SaveAndLoad : MonoBehaviour {

    // Use this for initialization
    public GameObject panel;
    public GameObject cubeMenuPanel;
    public GameObject buildPanel;
    public GameObject OverWriteConfirm;
    public GameObject returnConfirm;
    public GameObject shipScroll;
    public BlockPlacing blockPlacing;
    public InputField shipNameInput;

    public GameObject theBulid;

    public ShipData myShipData;
    ViewControl camEditationCon;

    void Start () {

        OverWriteConfirm.SetActive(false);
        returnConfirm.SetActive(false);
        shipScroll = GameObject.Find("ShipContent");
        panel = GameObject.Find("Panel");
        panel.SetActive(false);
        cubeMenuPanel.SetActive(false);

        myShipData.shipName = "defaultName";
        myShipData.cubeDatas = new List<CubeData>();

        camEditationCon = Camera.main.GetComponent<ViewControl>();
    }
	


    public void GoToSaveNLoadPanel()
    {
        panel.SetActive(true);
        buildPanel.SetActive(false);
        LockCamera(true);
        LoadAndSaveShip();
    }

    public void GoToCubeMenuPanel()
    {
        cubeMenuPanel.SetActive(true);
        buildPanel.SetActive(false);
        LockCamera(true);
    }

    public void BackToBuildPanel()
    {
        panel.SetActive(false);
        cubeMenuPanel.SetActive(false);
        buildPanel.SetActive(true);
        LockCamera(false);
    }


    private void Purse(bool b)
    {
        blockPlacing.paused = b;
    }

    private string ShipToJson()
    {
        myShipData.shipName = "defaultName";
        myShipData.cubeDatas.Clear();
        foreach (Transform cube in theBulid.transform)
        {
            string cubeName = cube.name;
            int i = cubeName.IndexOf('(');
            if(i > 0)
            {
                cubeName = cubeName.Remove(i);
            }
            Debug.Log(cubeName);

            CubeData data;
            data.cubeName = cubeName;
            if (GetCubeMaterial(cube.gameObject))
            {
                string str = GetCubeMaterial(cube.gameObject).name;
                str = str.Remove(str.IndexOf(' '));
                data.materialName = str;
            }
            else
            {
                data.materialName = "Material0";
            }
            TransformData transData;
            transData.position = cube.position;
            transData.rotation = cube.rotation;
            transData.scale = cube.localScale;

            data.transformDatas = transData;

            myShipData.cubeDatas.Add(data);            
        }
        string json = JsonUtility.ToJson(myShipData);
        return json;
    }

    public void LoadAndSaveShip()
    {
        
        foreach (Transform t in shipScroll.transform)
        {
            Destroy(t.gameObject);
            Debug.Log(">>>>>>>>>>>>>>>Destoryed<<<<<<<<<<<<<<<<<");
        }
        
        string path = Application.persistentDataPath;
        Debug.Log(Application.persistentDataPath);
        string[] fileArray = Directory.GetFiles(path, "*.json");
        foreach (string fileName in fileArray)
        {

            GameObject b = (GameObject)Instantiate(Resources.Load("LoadButton"));
            b.transform.SetParent(shipScroll.GetComponent<VerticalLayoutGroup>().transform);
            //b.transform.position += i*60*Vector3.down;
            Debug.Log(b);
            string shipName = fileName.Substring(fileName.LastIndexOf('/')+1, fileName.Length-6- fileName.LastIndexOf('/'));
            b.GetComponent<Button>().onClick.AddListener(delegate { PutNameInInPutField(shipName); });
            b.GetComponentInChildren<Text>().text = shipName;
        }
    }

    public void PutNameInInPutField(string name)
    {
        shipNameInput.text = name;
    }

    public void LoadShip()
    {
        Debug.Log("enterLoad");       
        Transform theshipTrans = theBulid.transform;
        HelperFuns.LoadShip(theshipTrans, shipNameInput.text);
    }

    public void SaveShip()
    {
        string path = Application.persistentDataPath + "/" + shipNameInput.text + ".json";
        if (File.Exists(path))
        {
            OverWriteConfirm.SetActive(true);
            return;
        }
        WriteShipToFile();
    }

    public void WriteShipToFile()
    {
        string path = Application.persistentDataPath + "/" + shipNameInput.text + ".json";
        string json = ShipToJson();
        File.WriteAllText(path, json);
        OverWriteConfirm.SetActive(false);
        Debug.Log(">>" + path);
        LoadAndSaveShip();
    }

    public void DontOverwrite()
    {
        OverWriteConfirm.SetActive(false);
    }

    public void ReturnToMain()
    {
        returnConfirm.SetActive(true);
    }
    public void NotReturnToMain()
    {
        returnConfirm.SetActive(false);
    }

    Material GetCubeMaterial(GameObject cube)
    {
        if (cube.GetComponent<Renderer>())
        {
            return cube.GetComponent<Renderer>().material;
        }
        else
        {
            Renderer[] childCubes = cube.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in childCubes)
            {
                if (!r.material.name.Contains("Fixed"))
                {
                    return r.material;
                }
            }
            return null;
        }
    }

    void LockCamera(bool b)
    {
        if (b)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            camEditationCon.axes = ViewControl.RotationAxes.locked;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            camEditationCon.axes = ViewControl.RotationAxes.MouseXAndY;
        }
    }
}
