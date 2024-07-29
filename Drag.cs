using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    public GameObject ControlPoint;
    private List<Vector3> Pos = new List<Vector3>();

    public bool alive;
    public bool isselected;

    //拖曳左右(0)or上下(1)
    public int DragMode = 0; 

    private Vector3 offset;
    private Vector3 OldPos, NewPos;
    // Use this for initialization
    void Start()
    {
        this.alive = true;
        this.isselected = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    
    void OnMouseDown()
    {
        OldPos = transform.position;
    }
    void OnMouseUp()
    {
        NewPos = transform.position;
    }
    void OnMouseDrag()
    {
        Vector3 TargetPos = Camera.main.WorldToScreenPoint(this.transform.position);
        Vector3 MousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, TargetPos.z);
        NewPos = Camera.main.ScreenToWorldPoint(MousePos);
        if(DragMode == 0)
            this.transform.position = new Vector3(NewPos.x, this.transform.position.y, NewPos.z);
        else if (DragMode == 1)
            this.transform.position = new Vector3(this.transform.position.x, NewPos.y, this.transform.position.z);

    }
    void OnMouseUpAsButton()
    {
        if (System.Math.Sqrt((NewPos.x- OldPos.x) *(NewPos.x - OldPos.x) +(NewPos.y - OldPos.y) *(NewPos.y - OldPos.y) 
            +(NewPos.z - OldPos.z) *(NewPos.z - OldPos.z)) < 3)
        {
            if (isselected == false)
            {
                this.GetComponent<Drag>().isselected = true;
                //this.isselected = true;
                this.GetComponent<MeshRenderer>().material.color = Color.red;
            }
            else
            {
                this.isselected = false;
                this.GetComponent<MeshRenderer>().material.color = new Color(1, 0.551f, 0, 1);
            }
        }
    }
    
}

    