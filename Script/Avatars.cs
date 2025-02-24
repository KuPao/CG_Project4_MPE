using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using Crosstales.FB;

public class Avatars : MonoBehaviour
{
    public Dropdown dropdown;
    public int avatarCount = 0;
    public int current = -1;
    public GameObject nullAvatar;
    public List<GameObject> avatars;
    private BVH_Importer importer;
    private GameObject avatar;
    public Material[] conMat;
    // Start is called before the first frame update
    void Start()
    {
        importer = this.GetComponent<BVH_Importer>();
        GameObject avatar = Instantiate(nullAvatar);
        avatar.name = "Avatar0";
        //avatar.transform.SetParent(this.transform);
        avatars.Add(avatar);
        avatarCount = 1;
        current = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void NextControlPoint()
    {
        int step = avatars[current].GetComponent<TheAvatar>().controlpoint.Length;
        if (avatars[current].GetComponent<TheAvatar>().controlpointAt != (step - 1))
        {
            avatars[current].GetComponent<TheAvatar>().controlpoint[avatars[current].GetComponent<TheAvatar>().controlpointAt].GetComponent<MeshRenderer>().material = conMat[0];
            avatars[current].GetComponent<TheAvatar>().controlpointAt++;
            avatars[current].GetComponent<TheAvatar>().controlpoint[avatars[current].GetComponent<TheAvatar>().controlpointAt].GetComponent<MeshRenderer>().material = conMat[1];
        }
    }
    public void PreControlPoint()
    {
        
        if (avatars[current].GetComponent<TheAvatar>().controlpointAt > 0)
        {
            avatars[current].GetComponent<TheAvatar>().controlpoint[avatars[current].GetComponent<TheAvatar>().controlpointAt].GetComponent<MeshRenderer>().material = conMat[0];
            avatars[current].GetComponent<TheAvatar>().controlpointAt--;
            avatars[current].GetComponent<TheAvatar>().controlpoint[avatars[current].GetComponent<TheAvatar>().controlpointAt].GetComponent<MeshRenderer>().material = conMat[1];
        }
    }
    public void ControlPointXPlus()
    {
        avatars[current].GetComponent<TheAvatar>().controlpoint[avatars[current].GetComponent<TheAvatar>().controlpointAt].transform.Translate(10, 0, 0);
        var current_avatar = avatars[current].GetComponent<TheAvatar>();
        current_avatar.ChangeControlPoint(current_avatar.motions[current_avatar.Motion]);
    }
    public void ControlPointXLess()
    {
        avatars[current].GetComponent<TheAvatar>().controlpoint[avatars[current].GetComponent<TheAvatar>().controlpointAt].transform.Translate(-10, 0, 0);
        var current_avatar = avatars[current].GetComponent<TheAvatar>();
        current_avatar.ChangeControlPoint(current_avatar.motions[current_avatar.Motion]);
    }
    public void ControlPointYPlus()
    {
        avatars[current].GetComponent<TheAvatar>().controlpoint[avatars[current].GetComponent<TheAvatar>().controlpointAt].transform.Translate(0, 10, 0);
        var current_avatar = avatars[current].GetComponent<TheAvatar>();
        current_avatar.ChangeControlPoint(current_avatar.motions[current_avatar.Motion]);
    }
    public void ControlPointYLess()
    {
        avatars[current].GetComponent<TheAvatar>().controlpoint[avatars[current].GetComponent<TheAvatar>().controlpointAt].transform.Translate(0, -10, 0);
        var current_avatar = avatars[current].GetComponent<TheAvatar>();
        current_avatar.ChangeControlPoint(current_avatar.motions[current_avatar.Motion]);
    }
    public void ControlPointZPlus()
    {
        avatars[current].GetComponent<TheAvatar>().controlpoint[avatars[current].GetComponent<TheAvatar>().controlpointAt].transform.Translate(0, 0, 10);
        var current_avatar = avatars[current].GetComponent<TheAvatar>();
        current_avatar.ChangeControlPoint(current_avatar.motions[current_avatar.Motion]);
    }
    public void ControlPointZLess()
    {
        avatars[current].GetComponent<TheAvatar>().controlpoint[avatars[current].GetComponent<TheAvatar>().controlpointAt].transform.Translate(0, 0, -10);
        var current_avatar = avatars[current].GetComponent<TheAvatar>();
        current_avatar.ChangeControlPoint(current_avatar.motions[current_avatar.Motion]);
    }
    public void Add_Avatar()
    {
        dropdown.options.Add(new Dropdown.OptionData() {text = "Avatar" + avatarCount.ToString()});
        avatar = Instantiate(nullAvatar);
        avatar.name = "Avatar" + avatarCount.ToString();
        //avatar.transform.SetParent(this.transform);
        avatar.transform.position = new Vector3(0, 0, 0);
        avatars.Add(avatar);
        avatarCount++;
        
    }

    public void Select_Avatar()
    {
        string name = dropdown.options[dropdown.value].text;
        current = int.Parse(name.Remove(0, 6));
    }
    
    public void Add_Motions()
    {
        Vector3 offset = new Vector3(0,0,0);
        TheAvatar ava = avatars[current].GetComponent<TheAvatar>();
        if (ava.motions.Count > 0)
        {
            offset.x = ava.motions[ava.motions.Count - 1].Channels[0].Keys[ava.motions[ava.motions.Count - 1].FrameCount - 1];
            offset.y = ava.motions[ava.motions.Count - 1].Channels[1].Keys[ava.motions[ava.motions.Count - 1].FrameCount - 1];
            offset.z = ava.motions[ava.motions.Count - 1].Channels[2].Keys[ava.motions[ava.motions.Count - 1].FrameCount - 1];
        }
            
        List <Bvh> newMotions = importer.parseFile(offset);
        avatars[current].GetComponent<TheAvatar>().Add_Motions(newMotions);        
        
    }
    public void Add_Bone()
    {
        avatars[current].GetComponent<TheAvatar>().Add_Bone();

    }
    public void Do_Motion()
    {
        avatars[current].GetComponent<TheAvatar>().domotion();
    }

    public void ChangeToMotion1()
    {
        //avatars[current].GetComponent<TheAvatar>().DoMotion1();
        avatars[current].GetComponent<TheAvatar>().Change_To_Motion(0);
    }
    public void ChangeToMotion2()
    {
        //avatars[current].GetComponent<TheAvatar>().DoMotion2();
        avatars[current].GetComponent<TheAvatar>().Change_To_Motion(1);
    }
}
