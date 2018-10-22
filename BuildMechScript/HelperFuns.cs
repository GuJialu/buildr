using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HelperFuns
{
    static string[] wordsToCerser = new string[2] { "badword", "drowdab" };

    public static void SetCubeMaterial(GameObject cube, Material newMaterial)
    {
        if (cube.GetComponent<Renderer>())
        {
            cube.GetComponent<Renderer>().material = newMaterial;
        }
        else
        {
            Renderer[] childCubes = cube.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in childCubes)
            {
                if (!r.material.name.Contains("Fixed")&& !r.material.name.Contains("Particle"))
                {
                    r.material = newMaterial;
                }
            }
        }
    }

    public static void SetAllToLayer(GameObject g, string layer)
    {
        Debug.Log(g + "<<<<<<<");
        foreach(Transform cube in g.transform)
        {
            //Debug.Log(cube);
            cube.gameObject.layer = LayerMask.NameToLayer(layer);
        }
    }

    public static void PauseGame(bool b)
    {
        if (b)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public static bool LoadShip(Transform theShipTrans, string shipName)
    {

        string path = Application.persistentDataPath + "/" + shipName + ".json";
        Debug.Log(path);

        if (!File.Exists(path))
        {
            return false;
        }
        
        foreach (Transform cube in theShipTrans)
        {
            MonoBehaviour.Destroy(cube.gameObject);
        }
        string json = File.ReadAllText(path);
        ShipData loadedShipData = JsonUtility.FromJson<ShipData>(json);
        foreach (CubeData c in loadedShipData.cubeDatas)
        {
            GameObject o = (GameObject)MonoBehaviour.Instantiate(Resources.Load(c.cubeName), c.transformDatas.position, c.transformDatas.rotation, theShipTrans);
            o.transform.localScale = c.transformDatas.scale;
            //Debug.Log(c.materialName + "<<<<<");
            //Debug.Log("Mass>>>>>"+o.GetComponent<PhysicalProperty>().mass);
            if (Resources.Load(c.materialName))
            {
                //Debug.Log(c.materialName);
                HelperFuns.SetCubeMaterial(o, (Material)Resources.Load(c.materialName));
            }
        }
        return true;
    }

    public static void LoadShipFromJson(Transform theShipTrans, string json)
    {
        ShipData loadedShipData = JsonUtility.FromJson<ShipData>(json);
        foreach (CubeData c in loadedShipData.cubeDatas)
        {
            GameObject o = (GameObject)MonoBehaviour.Instantiate(Resources.Load(c.cubeName), c.transformDatas.position, c.transformDatas.rotation, theShipTrans);
            o.transform.localScale = c.transformDatas.scale;
            if (Resources.Load(c.materialName))
            {
                //Debug.Log(c.materialName);
                HelperFuns.SetCubeMaterial(o, (Material)Resources.Load(c.materialName));
            }
        }
    }

    public static void WordCenser(string str)
    {
        foreach(string word in wordsToCerser)
        {
            str.Replace("word", "****");
        }
    }
}
