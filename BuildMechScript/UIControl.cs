using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControl : MonoBehaviour {
    public GameObject panel;
    public GameObject cubeMenuPanel;
    public GameObject buildPanel;

    public BlockPlacing blockPlacing;
    public SaveAndLoad saveAndLoad;
    ViewControl camEditationCon;
    // Use this for initialization
    void Start () {
        camEditationCon = Camera.main.GetComponent<ViewControl>();
    }

    public void GoToSaveNLoadPanel()
    {
        panel.SetActive(true);
        buildPanel.SetActive(false);
        cubeMenuPanel.SetActive(false);
        LockCamera(true);
        saveAndLoad.LoadAndSaveShip();
    }

    public void GoToCubeMenuPanel()
    {
        cubeMenuPanel.SetActive(true);
        buildPanel.SetActive(false);
        panel.SetActive(false);
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
