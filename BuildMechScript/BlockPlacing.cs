using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Toggle deleteToggle;


    enum Action { ADD = 0, DELETE = 1, PAINT = 3 };
    struct Operation
    {
        public GameObject o;
        public string name;
        public Transform t;
        public Action action;
        public Operation(string name, Transform t)
        {
            o = null;
            int i = name.IndexOf('(');
            if (i > 0)
            {
                name = name.Remove(i);
            }
            this.name = name;
            this.t = t;
            action = Action.DELETE;
        }
        public Operation(GameObject o)
        {
            this.o = o;
            this.name = null;
            this.t = null;
            action = Action.ADD;
        }
    }

    Stack<Operation> undoStack;
    Stack<Operation> redoStack;

    // Use this for initialization
    void Start()
    {
        ChooseBlock("Cube0");
        normalMaterial = currentBlock.GetComponent<Renderer>().material;
        undoStack = new Stack<Operation>();
        redoStack = new Stack<Operation>();
    }

    // Update is called once per frame
    void Update()
    {
        //stop building when paused
        if (paused)
        {
            return;
        }
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                // Construct a ray from the current touch coordinates
                var ray1 = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit1;
                if (Physics.Raycast(ray1, out hit1))
                {
                    //change color
                    if (painting)
                    {
                        SetCubeMaterial(hit1.collider.gameObject, paintMaterial);
                        return;
                    }
                    //deleting
                    if (deleteToggle.isOn)
                    {
                        undoStack.Push(new Operation(hit1.collider.gameObject.name, hit1.collider.gameObject.transform));
                        redoStack.Clear();
                        Destroy(hit1.collider.gameObject);
                        return;
                    }

                    //Check if the cube allows palcing on the x and z axis
                    if (hit1.normal != Vector3.up && currentBlock.GetComponent<CheckOverlapping>().fixUp)
                    {
                        return;
                    }
                    float distance = Vector3.Dot(currentBlock.GetComponent<BoxCollider>().bounds.size, hit1.normal);
                    distance = Mathf.Abs(distance);
                    //Debug.Log(">>>>>size" + currentBlock.GetComponent<BoxCollider>().bounds.size);
                    //Debug.Log(">>>>>distance: " + distance);
                    Vector3 pos = hit1.point + distance / 2 * hit1.normal;
                    pos.x = Mathf.Round(pos.x / 2.5f) * 2.5f;
                    pos.y = Mathf.Round(pos.y / 2.5f) * 2.5f;
                    pos.z = Mathf.Round(pos.z / 2.5f) * 2.5f;
                    currentBlock.transform.position = pos;

                    //check if placable
                    //Debug.Log("hit" + currentBlock.GetComponent<CheckOverlapping>().overlapped);
                    if (currentBlock.GetComponent<CheckOverlapping>().overlapped)
                    {
                        return;
                    }

                    //placing
                    GameObject b = Instantiate(currentBlock, theBuild.transform, true);
                    undoStack.Push(new Operation(b));
                    redoStack.Clear();
                    Destroy(b.GetComponent<Rigidbody>());
                    b.layer = LayerMask.NameToLayer("Default");
                }
            }
        }

        //mouse input for test only
        /*Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        Debug.DrawRay(ray.origin, ray.direction * 1011, Color.green);
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
        {
            //deleting
            if (deleteToggle.isOn)
            {
                undoStack.Push(new Operation(hit.collider.gameObject.name, hit.collider.gameObject.transform));
                redoStack.Clear();
                Destroy(hit.collider.gameObject);
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

            //check if placable
            //Debug.Log("hit" + currentBlock.GetComponent<CheckOverlapping>().overlapped);
            if (currentBlock.GetComponent<CheckOverlapping>().overlapped)
            {
                return;
            }

            //placing
            GameObject b = Instantiate(currentBlock, theBuild.transform, true);
            Destroy(b.GetComponent<Rigidbody>());
            b.layer = LayerMask.NameToLayer("Default");
            undoStack.Push(new Operation(b));
            redoStack.Clear();
        }


        //rotating
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentBlock.transform.Rotate(Vector3.up, 90f, Space.World);
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

                if (Vector3.Cross(currentBlock.transform.up, Vector3.up) == Vector3.zero)
                {
                    currentBlock.transform.localScale += Vector3.up;
                }
                else if (Vector3.Cross(currentBlock.transform.forward, Vector3.up) == Vector3.zero)
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
            if (Vector3.Cross(currentBlock.transform.up, Vector3.up) == Vector3.zero)
            {
                afterScaling = currentBlock.transform.localScale - Vector3.up;
            }
            else if (Vector3.Cross(currentBlock.transform.forward, Vector3.up) == Vector3.zero)
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
        */

        if (currentBlock)
        {
            currentBlock.transform.position = blockPreviewPos.position;
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
        deleteToggle.isOn = false;
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

    public void rotade()
    {
        currentBlock.transform.Rotate(Vector3.up, 90f, Space.World);
    }

    public void scaleUp()
    {
        currentBlock.transform.localScale += Vector3.up;
    }

    public void scaleDown()
    {
        Vector3 afterScaling;
        afterScaling = currentBlock.transform.localScale - Vector3.up;
        if (afterScaling.x != 0 && afterScaling.y != 0 && afterScaling.z != 0)
        {
            currentBlock.transform.localScale = afterScaling;
        }
    }

    public void Undo()
    {
        if (undoStack.Count == 0)
        {
            return;
        }
        Operation operation = undoStack.Pop();
        if (operation.action == Action.ADD)
        {
            GameObject temp = operation.o;
            redoStack.Push(new Operation(temp.name, temp.transform));
            Destroy(operation.o);
        }
        if (operation.action == Action.DELETE)
        {
            GameObject temp = (GameObject)Instantiate(Resources.Load(operation.name), theBuild.transform);
            temp.transform.position = operation.t.position;
            temp.transform.rotation = operation.t.rotation;
            temp.transform.localScale = operation.t.localScale;
            redoStack.Push(new Operation(temp.name, temp.transform));
        }
    }

    public void redo()
    {
        if (redoStack.Count == 0)
        {
            return;
        }
        Operation operation = redoStack.Pop();
        if (operation.action == Action.ADD)
        {
            GameObject temp = operation.o;
            undoStack.Push(new Operation(temp.name, temp.transform));
            Destroy(temp);
        }
        if (operation.action == Action.DELETE)
        {
            GameObject temp = (GameObject)Instantiate(Resources.Load(operation.name), theBuild.transform);
            temp.transform.position = operation.t.position;
            temp.transform.rotation = operation.t.rotation;
            temp.transform.localScale = operation.t.localScale;
            undoStack.Push(new Operation(temp.name, temp.transform));
        }
    }
}
