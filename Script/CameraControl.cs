using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    public Avatars bvhController;
    public InputField[] CameraPos;
    public Dropdown CameraParDropdown;
    public CameraMove cameraMove;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnCameraParChange()
    {
        if (CameraParDropdown.value == 0)
        {
            cameraMove.parent = null;
            cameraMove.transform.position = new Vector3(0, 1, -10);
            cameraMove.transform.rotation = Quaternion.identity;
        }
        else if (CameraParDropdown.value == 1)
        {
            if (bvhController.avatars.Count >= 1)
                cameraMove.parent = bvhController.avatars[0].GetComponent<TheAvatar>().thisavatar.transform.GetChild(0).gameObject;
        }
        OnCameraPosChange();
    }

    public void OnCameraPosChange()
    {
        cameraMove.distance = new Vector3(float.Parse(CameraPos[0].text), float.Parse(CameraPos[1].text), float.Parse(CameraPos[2].text));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
