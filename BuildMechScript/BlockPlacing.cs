﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlacing : MonoBehaviour
{
    public GameObject currentBlock;
    public Transform blockPreviewPos;
    public GameObject theBuild;

    public Material warningMaterial;
    private Material normalMaterial;

    public bool painting;
    public bool paused = false;
    public Material paintMaterial;

    // Use this for initialization
    void Start()
    {
        ChooseBlock("Cube0");
        //theBuild = GameObject.Find("TheShip");
        normalMaterial = currentBlock.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {

        //stop building when paused
        if (paused)
        {
            return;
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        Debug.DrawRay(ray.origin, ray.direction * 1011, Color.green);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //change color
            if (painting)
            {
                if (Input.GetMouseButton(0))
                {
                    SetCubeMaterial(hit.collider.gameObject, paintMaterial);
                }
                return;
            }
            //Check if the cube allows palcing on the x and z axis
            if (hit.normal != Vector3.up && currentBlock.GetComponent<CheckOverlapping>().fixUp)
            {
                return;
            }
            float distance = Vector3.Dot(currentBlock.GetComponent<BoxCollider>().bounds.size, hit.normal);
            distance = Mathf.Abs(distance);
            //Debug.Log(">>>>>size" + currentBlock.GetComponent<BoxCollider>().bounds.size);
            //Debug.Log(">>>>>distance: " + distance);
            Vector3 pos = hit.point + distance / 2 * hit.normal;
            pos.x = Mathf.Round(pos.x / 2.5f) * 2.5f;
            pos.y = Mathf.Round(pos.y / 2.5f) * 2.5f;
            pos.z = Mathf.Round(pos.z / 2.5f) * 2.5f;
            currentBlock.transform.position = pos;

            //rotating
            if (Input.GetKeyDown(KeyCode.R))
            {
                currentBlock.transform.Rotate(hit.normal, 90f, Space.World);
            }

            //scaling
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                int maxScale = currentBlock.GetComponent<CheckOverlapping>().maxScale;
                float totalScale = currentBlock.transform.localScale.x *
                    currentBlock.transform.localScale.y *
                    currentBlock.transform.localScale.z;
                if (totalScale < maxScale)
                {

                    if (Vector3.Cross(currentBlock.transform.up, hit.normal) == Vector3.zero)
                    {
                        currentBlock.transform.localScale += Vector3.up;
                    }
                    else if (Vector3.Cross(currentBlock.transform.forward, hit.normal) == Vector3.zero)
                    {
                        currentBlock.transform.localScale += Vector3.forward;
                    }
                    else
                    {
                        currentBlock.transform.localScale += Vector3.right;
                    }

                }
            }
            if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                Vector3 afterScaling;
                if (Vector3.Cross(currentBlock.transform.up, hit.normal) == Vector3.zero)
                {
                    afterScaling = currentBlock.transform.localScale - Vector3.up;
                }
                else if (Vector3.Cross(currentBlock.transform.forward, hit.normal) == Vector3.zero)
                {
                    afterScaling = currentBlock.transform.localScale - Vector3.forward;
                }
                else
                {
                    afterScaling = currentBlock.transform.localScale - Vector3.right;
                }

                if (afterScaling.x != 0 && afterScaling.y != 0 && afterScaling.z != 0)
                {
                    currentBlock.transform.localScale = afterScaling;
                }
            }

            //check if placable
            //Debug.Log("hit" + currentBlock.GetComponent<CheckOverlapping>().overlapped);
            if (currentBlock.GetComponent<CheckOverlapping>().overlapped)
            {
                SetCubeMaterial(currentBlock, warningMaterial);
                return;
            }
            SetCubeMaterial(currentBlock, normalMaterial);

            //placing
            if (Input.GetMouseButtonDown(0))
            {

                GameObject b = Instantiate(currentBlock, theBuild.transform, true);
                Destroy(b.GetComponent<Rigidbody>());
                b.layer = LayerMask.NameToLayer("Default");
            }
            //deleting
            if (Input.GetMouseButtonDown(1))
            {
                if (theBuild.transform.childCount > 1)
                {
                    Destroy(hit.collider.gameObject);
                }
                else
                {
                    Debug.Log(">>>>cannot delete");
                }
            }
        }
        else
        {
            if (currentBlock)
            {
                currentBlock.transform.position = blockPreviewPos.position;
            }
        }
    }

    public void ChooseBlock(string blockName)
    {
        if (currentBlock)
        {
            Destroy(currentBlock);
        }
        currentBlock = (GameObject)Instantiate(Resources.Load(blockName), blockPreviewPos.position, Quaternion.identity);
        currentBlock.layer = LayerMask.NameToLayer("Ignore Raycast");
        Rigidbody rb = currentBlock.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        painting = false;
    }

    public void ChooseColor(string materialName)
    {
        if (currentBlock)
        {
            Destroy(currentBlock);
        }
        paintMaterial = (Material)Resources.Load(materialName);
        painting = true;
    }

    public void SetCubeMaterial(GameObject cube, Material newMaterial)
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
                if (!r.material.name.Contains("Fixed") && !r.material.name.Contains("Particle"))
                {
                    r.material = newMaterial;
                }
            }
        }
    }
}