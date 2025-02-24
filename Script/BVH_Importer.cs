using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;
using Crosstales.FB;

public class BVH_Importer : MonoBehaviour
{
    
    public string[] paths;
    public Bvh Bvh;
    public string bvh_src;
    public GameObject skeleton;
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Bvh> parseFile(Vector3 offset)
    {
        paths = FileBrowser.OpenFiles("bvh");
        List<Bvh> bvhs = new List<Bvh>();
        int i = 0;
        foreach (string path in paths)
        {
            bvh_src = File.ReadAllText(path);
            Bvh = Bvh.Parse(bvh_src, offset);
            bvhs.Add(Bvh);
        }
        //ROOT();
        return bvhs;
        Debug.Log("load " + paths.Length + " files.");
        
    }
    GameObject super;
    public void ROOT()
    {
         GameObject root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
         root.transform.position =new Vector3 (Bvh.Root.Offset.x, Bvh.Root.Offset.y, Bvh.Root.Offset.z);
         root.name = Bvh.Root.Name;
        super = root;
        foreach (BvhNode descentant in Bvh.Root.Children)
        {
            super = root;
            bones(descentant);
        }
        
        Debug.Log("Bvh.Root.Offset"+ Bvh.Root.Offset);
        //Debug.Log("Bvh.Root.Channel" + Bvh.Root.Channels);
    }

    public void bones(BvhNode child)
    {
        GameObject bone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bone.transform.parent = super.transform;
        bone.transform.position = new Vector3(child.Offset.x, child.Offset.y, child.Offset.z);
        bone.name = child.Name;
        DrawLS(super, bone);
        super = bone;
        if(child.Children!=null)
            foreach (BvhNode descentant in child.Children)
            {
                super = bone;
                bones(descentant);            
            }
        
    }
    void DrawLS(GameObject startP, GameObject finalP)
    {
        Vector3 rightPosition = (startP.transform.position + finalP.transform.position) / 2;
        Vector3 rightRotation = finalP.transform.position - startP.transform.position;
        float HalfLength = Vector3.Distance(startP.transform.position, finalP.transform.position) / 2;
        float LThickness = 0.1f;//粗细

        //創建圆柱體
        GameObject MyLine = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        MyLine.gameObject.transform.parent = skeleton.transform;
        MyLine.transform.position = rightPosition;
        MyLine.transform.rotation = Quaternion.FromToRotation(Vector3.up, rightRotation);
        MyLine.transform.localScale = new Vector3(LThickness, HalfLength, LThickness);

        //設置材質
        //MyLine.GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
    }

}
