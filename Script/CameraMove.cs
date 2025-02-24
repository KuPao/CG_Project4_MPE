using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float sensitivityMouse = 2f;
    public float sensitivetyKeyBoard = 0.1f;
    public float sensitivetyMouseWheel = 10f;

    public GameObject parent;
    public Vector3 distance;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //滾輪實現鏡頭縮進和拉遠
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            Camera.main.fieldOfView = Camera.main.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * sensitivetyMouseWheel;
        }
        //按著鼠標右鍵實現視角轉動
        if (Input.GetMouseButton(1))
        {
            transform.Rotate(-Input.GetAxis("Mouse Y") * sensitivityMouse, Input.GetAxis("Mouse X") * sensitivityMouse, 0);
        }

        //鍵盤按鈕←/一和→/ d實現視角水平移動，鍵盤按鈕↑/ w的和↓/ s的實現視角水平旋轉
        if (parent != null && parent.activeSelf == true)
        {
            Vector3 pos = parent.transform.position;
            pos = pos + distance;
            transform.position = pos;
        }
        else
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                transform.Translate(Input.GetAxis("Horizontal") * sensitivetyKeyBoard, 0, 0);
            }
            if (Input.GetAxis("Vertical") != 0)
            {
                transform.Translate(0, Input.GetAxis("Vertical") * sensitivetyKeyBoard, 0);
            }
        }
    }
}
