using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TheAvatar : MonoBehaviour
{
    String m_path;
    int bone = 0;
    public String Path
    {
        get { return m_path; }
        set
        {
            if (m_path == value)
            {
                return;
            }

            m_path = value;
        }
    }
    //public Animator targetAvatar;
    public List<Bvh> motions = new List<Bvh>();
    public bool started = false;
    public int Motion = 0/*index of last motion*/, frames = 0, nodeframe = 0;
    public float frametime = 0, startime;
    public float frameRate;
    public GameObject thisavatar;
    GameObject super;
    
    private Dictionary<string, string> renamingMap;
    private Dictionary<string, Transform> nameMap;
    private string prefix;
    public int controlpointAt = 0;
    public GameObject[] controlpoint = new GameObject[4];
    public AvatarDescription AvatarDescription;
    public Avatar avatar;
    public Mesh Mesh;
    public Material Material;
    public Animation animation;
    public AnimationClip Animation;
    public bool doingAction = false;
    public Animator animator;
    public List<GameObject> Points;
    float scaling = 1.0f;
    [SerializeField]
    HumanPoseTransfer m_dst;
    public GameObject model;
    public List<GameObject> models;
    // Start is called before the first frame update
    void Start()
    {
        if(model != null)
        {
            GameObject step = Instantiate(model);
            m_dst = step.GetComponent<HumanPoseTransfer>();
            models.Add(step);
        }
           
        thisavatar = this.gameObject;
       
    }

    // Update is called once per frame
    void Update()
    {
        if(models.Count > 0) 
        {
            models[0].transform.position = this.transform.position;
        }
        /*float times  ;
        if (this.transform.childCount != 0 && motions.Count != 0)
        {
            times = (float)motions[Motion].FrameTime.TotalSeconds;
            InvokeRepeating("Do_Motions", times, times);
        }*/

    }

    public void ChangeControlPoint(Bvh current_motion)
    {
        while (Points.Count != 0)
        {
            GameObject step = Points[0];
            Points.Remove(step);
            Destroy(step);
        }
        //motions[0].ControlPoints[controlpointAt] = controlpoint[controlpointAt].transform.position;
        //ChannelCurve[] oldCh = motions[0].Channels;
        //ChangeCurveFitting(motions[0]);
        //InsertFrame(motions[0], oldCh);
        current_motion.ControlPoints[controlpointAt] = controlpoint[controlpointAt].transform.position;
        ChannelCurve[] oldCh = current_motion.Channels;
        ChangeCurveFitting(current_motion);
        InsertFrame(current_motion, oldCh);
        //ChordLength(motions[0]);
    }
    public void ChangeCurveFitting(Bvh bvh)
    {
        int frameCount = bvh.FrameCount;

        float B3_0(float t) => 0.16667f * (1 - t) * (1 - t) * (1 - t);
        float B3_1(float t) => 0.16667f * (3 * t * t * t - 6 * t * t + 4);
        float B3_2(float t) => 0.16667f * (-3 * t * t * t + 3 * t * t + 3 * t + 1);
        float B3_3(float t) => 0.16667f * t * t * t;

        for (int i = 0; i < frameCount; i++)
        {
            int j = frameCount - 1 - i;           
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            float t = bvh.Params[i];

            Vector3 pos = new Vector3();
            pos.x = B3_0(t) * motions[0].ControlPoints[0].x + B3_1(t) * motions[0].ControlPoints[1].x + B3_2(t) * motions[0].ControlPoints[2].x + B3_3(t) * motions[0].ControlPoints[3].x;
            pos.y = B3_0(t) * motions[0].ControlPoints[0].y + B3_1(t) * motions[0].ControlPoints[1].y + B3_2(t) * motions[0].ControlPoints[2].y + B3_3(t) * motions[0].ControlPoints[3].y;
            pos.z = B3_0(t) * motions[0].ControlPoints[0].z + B3_1(t) * motions[0].ControlPoints[1].z + B3_2(t) * motions[0].ControlPoints[2].z + B3_3(t) * motions[0].ControlPoints[3].z;
            Vector3 nextPos = new Vector3();
            if (i < frameCount - frameCount * 1 / 10)
            {
                float tNext = bvh.Params[i + frameCount / 10];
                nextPos.x = B3_0(tNext) * motions[0].ControlPoints[0].x + B3_1(tNext) * motions[0].ControlPoints[1].x + B3_2(tNext) * motions[0].ControlPoints[2].x + B3_3(tNext) * motions[0].ControlPoints[3].x;
                nextPos.y = B3_0(tNext) * motions[0].ControlPoints[0].y + B3_1(tNext) * motions[0].ControlPoints[1].y + B3_2(tNext) * motions[0].ControlPoints[2].y + B3_3(tNext) * motions[0].ControlPoints[3].y;
                nextPos.z = B3_0(tNext) * motions[0].ControlPoints[0].z + B3_1(tNext) * motions[0].ControlPoints[1].z + B3_2(tNext) * motions[0].ControlPoints[2].z + B3_3(tNext) * motions[0].ControlPoints[3].z;
                Vector3 f = (pos - nextPos);
                Vector3 forwardVector = Quaternion.LookRotation(f).ToEuler();
                bvh.Channels[4].SetKey(i, 0);
                bvh.Channels[5].SetKey(i,  -(360 / (float)(3.14 * 2) * forwardVector.y) +180);
                bvh.Channels[3].SetKey(i, 0);
            }
            else
            {
                float tNext = bvh.Params[i - frameCount / 10];
                nextPos.x = B3_0(tNext) * motions[0].ControlPoints[0].x + B3_1(tNext) * motions[0].ControlPoints[1].x + B3_2(tNext) * motions[0].ControlPoints[2].x + B3_3(tNext) * motions[0].ControlPoints[3].x;
                nextPos.y = B3_0(tNext) * motions[0].ControlPoints[0].y + B3_1(tNext) * motions[0].ControlPoints[1].y + B3_2(tNext) * motions[0].ControlPoints[2].y + B3_3(tNext) * motions[0].ControlPoints[3].y;
                nextPos.z = B3_0(tNext) * motions[0].ControlPoints[0].z + B3_1(tNext) * motions[0].ControlPoints[1].z + B3_2(tNext) * motions[0].ControlPoints[2].z + B3_3(tNext) * motions[0].ControlPoints[3].z;
                Vector3 f = (nextPos - pos);
                Vector3 forwardVector = Quaternion.LookRotation(f).ToEuler();
                bvh.Channels[4].SetKey(i, 0);
                bvh.Channels[5].SetKey(i, -(360 / (float)(3.14 * 2) * forwardVector.y)+180);
                bvh.Channels[3].SetKey(i, 0);
            }

            sphere.transform.position = pos;
            sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); 
            bvh.Channels[0].SetKey(i, pos.x);
            bvh.Channels[1].SetKey(i, pos.y);
            bvh.Channels[2].SetKey(i, pos.z);
            Points.Add(sphere);
        }
    }

    public void InsertFrame(Bvh bvh, ChannelCurve[] oldCh)
    {
        int oldFrameNum = bvh.FrameCount;
        double old_time = bvh.FrameCount * bvh.FrameTime.TotalSeconds;
        double currentL = CurveLength(bvh);
        int newFrameNum = (int)(currentL / bvh.lPerFrame);
        float tMax = ((float)oldFrameNum / (float)newFrameNum);
        bvh.m_frames = newFrameNum;
        ChannelCurve[] newCh = new ChannelCurve[bvh.Channels.Length];
        newCh = Enumerable.Range(0, bvh.Channels.Length)
            .Select(x => new ChannelCurve(newFrameNum))
            .ToArray()
            ;
        Debug.Log(oldFrameNum);
        Debug.Log(newFrameNum);
        //bvh.FrameTime = TimeSpan.FromSeconds(old_time / newFrameNum);
        float B3_0(float t) => 0.16667f * (1 - t) * (1 - t) * (1 - t);
        float B3_1(float t) => 0.16667f * (3 * t * t * t - 6 * t * t + 4);
        float B3_2(float t) => 0.16667f * (-3 * t * t * t + 3 * t * t + 3 * t + 1);
        float B3_3(float t) => 0.16667f * t * t * t;

        float[] newParams = new float[newFrameNum];
        if(newFrameNum > oldFrameNum)
        {
            for(int i = 0; i < oldFrameNum; i++)
            {
                newParams[i] = bvh.Params[i] * tMax;
            }
            for(int i = oldFrameNum; i < newFrameNum; i++)
            {
                newParams[i] = bvh.Params[i - oldFrameNum] * tMax + tMax;
            }
        }
        else
        {
            for(int i = 0; i < newFrameNum; i++)
            {
                newParams[i] = bvh.Params[i] * tMax;
            }
        }

        newCh[0].Keys = new float[newFrameNum];
        newCh[1].Keys = new float[newFrameNum];
        newCh[2].Keys = new float[newFrameNum];
        newCh[3].Keys = new float[newFrameNum];
        newCh[4].Keys = new float[newFrameNum];
        newCh[5].Keys = new float[newFrameNum];
        for (int i = 0; i < newFrameNum; i++)
        {
            float t = newParams[i];
            Vector3 pos = new Vector3();
            pos.x = B3_0(t) * motions[0].ControlPoints[0].x + B3_1(t) * motions[0].ControlPoints[1].x + B3_2(t) * motions[0].ControlPoints[2].x + B3_3(t) * motions[0].ControlPoints[3].x;
            pos.y = B3_0(t) * motions[0].ControlPoints[0].y + B3_1(t) * motions[0].ControlPoints[1].y + B3_2(t) * motions[0].ControlPoints[2].y + B3_3(t) * motions[0].ControlPoints[3].y;
            pos.z = B3_0(t) * motions[0].ControlPoints[0].z + B3_1(t) * motions[0].ControlPoints[1].z + B3_2(t) * motions[0].ControlPoints[2].z + B3_3(t) * motions[0].ControlPoints[3].z;
            Vector3 nextPos = new Vector3();
            if (i < newFrameNum - newFrameNum * 1 / 10)
            {
                float tNext = newParams[i + newFrameNum / 10];
                nextPos.x = B3_0(tNext) * motions[0].ControlPoints[0].x + B3_1(tNext) * motions[0].ControlPoints[1].x + B3_2(tNext) * motions[0].ControlPoints[2].x + B3_3(tNext) * motions[0].ControlPoints[3].x;
                nextPos.y = B3_0(tNext) * motions[0].ControlPoints[0].y + B3_1(tNext) * motions[0].ControlPoints[1].y + B3_2(tNext) * motions[0].ControlPoints[2].y + B3_3(tNext) * motions[0].ControlPoints[3].y;
                nextPos.z = B3_0(tNext) * motions[0].ControlPoints[0].z + B3_1(tNext) * motions[0].ControlPoints[1].z + B3_2(tNext) * motions[0].ControlPoints[2].z + B3_3(tNext) * motions[0].ControlPoints[3].z;
                Vector3 f = (pos - nextPos);
                Vector3 forwardVector = Quaternion.LookRotation(f).ToEuler();
                newCh[4].SetKey(i, 0);
                newCh[5].SetKey(i, -(360 / (float)(3.14 * 2) * forwardVector.y) + 180);
                newCh[3].SetKey(i, 0);
            }
            else
            {
                float tNext = newParams[i - newFrameNum / 10];
                nextPos.x = B3_0(tNext) * motions[0].ControlPoints[0].x + B3_1(tNext) * motions[0].ControlPoints[1].x + B3_2(tNext) * motions[0].ControlPoints[2].x + B3_3(tNext) * motions[0].ControlPoints[3].x;
                nextPos.y = B3_0(tNext) * motions[0].ControlPoints[0].y + B3_1(tNext) * motions[0].ControlPoints[1].y + B3_2(tNext) * motions[0].ControlPoints[2].y + B3_3(tNext) * motions[0].ControlPoints[3].y;
                nextPos.z = B3_0(tNext) * motions[0].ControlPoints[0].z + B3_1(tNext) * motions[0].ControlPoints[1].z + B3_2(tNext) * motions[0].ControlPoints[2].z + B3_3(tNext) * motions[0].ControlPoints[3].z;
                Vector3 f = (nextPos - pos);
                Vector3 forwardVector = Quaternion.LookRotation(f).ToEuler();
                newCh[4].SetKey(i, 0);
                newCh[5].SetKey(i, -(360 / (float)(3.14 * 2) * forwardVector.y) + 180);
                newCh[3].SetKey(i, 0);
            }
            newCh[0].SetKey(i, pos.x);
            newCh[1].SetKey(i, pos.y);
            newCh[2].SetKey(i, pos.z);
        }

        for(int j = 6; j < newCh.Length; j++)
        {
            newCh[j].Keys = new float[newFrameNum];
            for(int i = 0; i < newFrameNum; i++)
            {
                newCh[j].SetKey(i, oldCh[j].Keys[i % oldFrameNum]);
                //Debug.Log(i);
            }
        }
        bvh.Channels = newCh;
        bvh.Params = newParams;
        //int newFrameNum = 0;//目前總共+幾張
        //int lastFrameNum = 0;//剛剛+幾張
        //ChannelCurve[] temp = bvh.Channels;
        //for (int i = 0; i < frameCount - 1; i++)
        //{
        //    Vector3 pos = new Vector3(bvh.Channels[0].Keys[i], bvh.Channels[1].Keys[i], bvh.Channels[2].Keys[i]);
        //    Vector3 next = new Vector3(bvh.Channels[0].Keys[i + 1], bvh.Channels[1].Keys[i + 1], bvh.Channels[2].Keys[i + 1]);
        //    Vector3 dis = pos - next;

        //    if (dis.magnitude > bvh.lPerFrame)
        //    {
        //        lastFrameNum = newFrameNum;
        //        int newFrame = (int)(dis.magnitude / bvh.lPerFrame);//這次+幾張
        //        newFrameNum += newFrame;
        //        ChannelCurve[] c = new ChannelCurve[bvh.Channels.Length];
        //        for (int ch = 0; ch < c.Length; ch++)
        //        {
        //            c[ch].Keys = new float[frameCount + newFrameNum];//新ch大小

        //            int j = 0;
        //            for (j = 0; j < i + lastFrameNum; j++)
        //            {
        //                c[ch].Keys[j] = temp[ch].Keys[j];
        //            }
        //            for(; j < i + newFrameNum; j++)
        //            {
        //                c[ch].Keys[j] = temp[ch].Keys[lastFrameNum + i] * (j - i) / newFrame + temp[ch].Keys[lastFrameNum + i + 1] * (newFrame - j + i) / newFrame;
        //            }
        //            for(; j < frameCount + newFrameNum; j++)
        //            {
        //                c[ch].Keys[j] = temp[ch].Keys[lastFrameNum + j - newFrameNum];
        //            }
        //        }
        //        temp = c;
        //    }
        //}
        //bvh.m_frames += newFrameNum;
    }

    public void domotion()
    {

        //
        // create last motion AnimationClip
        //
        ChordLength(motions[Motion]);
        CurveFitting(motions[Motion], false, true);
        for (int i = 0; i < 4; i++) {
            controlpoint[i].transform.position = motions[Motion].ControlPoints[i];
            //controlpoint[i] = Instantiate(controlpoint[i]);
        }
        AnimationClip Animation = BvhAnimation.CreateAnimationClip(motions[Motion], scaling);
        Animation.name = thisavatar.name;
        Animation.legacy = true;
        Animation.wrapMode = WrapMode.Loop;
        animation.AddClip(Animation, Animation.name);
        animation.clip = Animation;
        animation.Play();
        Destroy(thisavatar.GetComponent<HumanPoseTransfer>());
        var humanPoseTransfer = thisavatar.AddComponent<HumanPoseTransfer>();
        humanPoseTransfer.Avatar = avatar;
    }

    // Deparcted
    //public void DoMotion1()
    //{
    //    //
    //    // create AnimationClip
    //    //
    //    int blendTime = 40;
    //    ChannelCurve[] oldM = motions[0].Channels;
    //    int f = GetCurrentFrame(1, thisavatar.transform.GetChild(0).gameObject.transform.position);
    //    int simF = FindSimFrame(f, motions[1], motions[0], blendTime);
    //    Vector3 origin = new Vector3(motions[0].Channels[0].Keys[0], motions[0].Channels[1].Keys[0], motions[0].Channels[2].Keys[0]);
        

    //    for (int i = 0; i < blendTime; i++)
    //    {
    //        for(int j = 3; j < motions[0].Channels.Length; j++)
    //        {
    //            motions[0].Channels[j].SetKey(i, motions[0].Channels[j].Keys[simF] * i / blendTime + motions[1].Channels[j].Keys[f] * (blendTime - i)/ blendTime);
    //        }
    //    }
    //    for (int i = 0; i < motions[0].FrameCount; i++)
    //    {
    //        //Vector3 off = new Vector3(0, 0, 0);
    //        //if (i + simF >= motions[0].FrameCount)
    //        //    off = origin;
    //        //else
    //        //{
    //        //    off = new Vector3(-oldM[0].Keys[0] + oldM[0].Keys[motions[0].FrameCount - 1],
    //        //        -oldM[1].Keys[0] + oldM[1].Keys[motions[0].FrameCount - 1],
    //        //        -oldM[2].Keys[0] + oldM[2].Keys[motions[0].FrameCount - 1]);
    //        //}
    //        motions[0].Channels[0].SetKey(i, thisavatar.transform.GetChild(0).gameObject.transform.position.x - origin.x + motions[0].Channels[0].Keys[i]);
    //        motions[0].Channels[1].SetKey(i, thisavatar.transform.GetChild(0).gameObject.transform.position.y - origin.y + motions[0].Channels[1].Keys[i]);
    //        motions[0].Channels[2].SetKey(i, thisavatar.transform.GetChild(0).gameObject.transform.position.z - origin.z + motions[0].Channels[2].Keys[i]);
    //    }

    //    CurveFitting(motions[0], false, true);
    //    ChangeControlPoint(motions[0]);

    //    AnimationClip Animation = BvhAnimation.CreateAnimationClip(motions[0], scaling);
    //    Animation.name = thisavatar.name;
    //    Animation.legacy = true;
    //    Animation.wrapMode = WrapMode.Loop;
    //    //var animation = null;
    //    //Destroy(thisavatar.GetComponent<Animation>());
    //    //animation = thisavatar.AddComponent<Animation>();
    //    animation.AddClip(Animation, Animation.name);
    //    animation.clip = Animation;
    //    animation.Play();
    //    //Destroy(thisavatar.GetComponent<HumanPoseTransfer>());
    //    var humanPoseTransfer = thisavatar.AddComponent<HumanPoseTransfer>();
    //    humanPoseTransfer.Avatar = avatar;
    //}

    // Deparcted
    //public void DoMotion2()
    //{
    //    //
    //    // create AnimationClip
    //    //
    //    int blendTime = 10;
    //    int f = GetCurrentFrame(0, thisavatar.transform.GetChild(0).gameObject.transform.position);
    //    int simFrame = FindSimFrame(f, motions[0], motions[1], blendTime);
    //    Vector3 origin = new Vector3(motions[1].Channels[0].Keys[0], motions[1].Channels[1].Keys[0], motions[1].Channels[2].Keys[0]);

    //    Bvh temp_bvh = motions[1].DeepCopyAnimation();
    //    for (int i = 0; i < blendTime; i++)
    //    {
    //        for (int j = 3; j < motions[1].Channels.Length; j++)
    //        {
    //            temp_bvh.Channels[j].SetKey(i, motions[1].Channels[j].Keys[simFrame] * i / blendTime + motions[0].Channels[j].Keys[f] * (blendTime - i) / blendTime);
    //            //motions[1].Channels[j].SetKey(i, motions[1].Channels[j].Keys[simFrame] * i / blendTime + motions[0].Channels[j].Keys[f] * (blendTime - i) / blendTime);
    //        }
    //    }
    //    for (int i = 0; i < motions[1].FrameCount - simFrame; i++) {
    //        for (int j = 3; j < motions[1].Channels.Length; j++) {
    //            temp_bvh.Channels[j].SetKey(blendTime + i, motions[1].Channels[j].Keys[simFrame+i]);
    //        }
    //    }

    //    //not finished
    //    temp_bvh.m_frames = motions[1].FrameCount - simFrame + blendTime;

    //    for (int i = 0; i < temp_bvh.FrameCount; i++)
    //    {
    //        //Vector3 off = new Vector3(0, 0, 0);
    //        //if (i + simF >= motions[1].FrameCount)
    //        //    off = origin;
    //        //else
    //        //{
    //        //    off = new Vector3(-oldM[0].Keys[0] + oldM[0].Keys[motions[1].FrameCount - 1],
    //        //        -oldM[1].Keys[0] + oldM[1].Keys[motions[1].FrameCount - 1],
    //        //        -oldM[2].Keys[0] + oldM[2].Keys[motions[1].FrameCount - 1]);
    //        //}
    //        temp_bvh.Channels[0].SetKey(i, thisavatar.transform.GetChild(0).gameObject.transform.position.x - origin.x + motions[1].Channels[0].Keys[simFrame-blendTime+i]);
    //        temp_bvh.Channels[1].SetKey(i, thisavatar.transform.GetChild(0).gameObject.transform.position.y - origin.y + motions[1].Channels[1].Keys[simFrame-blendTime+i]);
    //        temp_bvh.Channels[2].SetKey(i, thisavatar.transform.GetChild(0).gameObject.transform.position.z - origin.z + motions[1].Channels[2].Keys[simFrame-blendTime+i]);
    //    }

    //    //CurveFitting(temp_bvh);
    //    //ChangeControlPoint(temp_bvh);

    //    AnimationClip Animation = BvhAnimation.CreateAnimationClip(temp_bvh, scaling);
    //    Animation.name = thisavatar.name;
    //    Animation.legacy = true;
    //    Animation.wrapMode = WrapMode.Loop;

    //    // Return to original motions[1] when mixture animation ends.
    //    AnimationEvent evt;
    //    evt = new AnimationEvent();
    //    evt.time = temp_bvh.FrameCount / Animation.frameRate;
    //    Motion = 1;
    //    evt.functionName = "domotion";
    //    Animation.AddEvent(evt);

    //    //var animation = null;
    //    //Destroy(thisavatar.GetComponent<Animation>());
    //    //animation = thisavatar.AddComponent<Animation>();
    //    animation.AddClip(Animation, Animation.name);
    //    animation.clip = Animation;
    //    animation.Play();
    //    Destroy(thisavatar.GetComponent<HumanPoseTransfer>());
    //    var humanPoseTransfer = thisavatar.AddComponent<HumanPoseTransfer>();
    //    humanPoseTransfer.Avatar = avatar;
    //}

    public void Change_To_Motion(int index) {
        if (Motion == index)
            return;
        //
        // create AnimationClip
        //
        int current_index = (index + 1) % 2;
        int blendTime = 10;
        int f = GetCurrentFrame(current_index, thisavatar.transform.GetChild(0).gameObject.transform.position);
        int simFrame = FindSimFrame(f, motions[current_index], motions[index], blendTime);
        Vector3 origin = new Vector3(motions[index].Channels[0].Keys[simFrame - blendTime], motions[index].Channels[1].Keys[simFrame - blendTime], motions[index].Channels[2].Keys[simFrame - blendTime]);

        Bvh temp_bvh = motions[index].DeepCopyClip(motions[index].FrameCount - simFrame + blendTime);
        //not finished
        temp_bvh.m_frames = motions[index].FrameCount - simFrame + blendTime;

        // Blend
        for (int i = 0; i < blendTime; i++) {
            for (int j = 3; j < motions[index].Channels.Length; j++) {
                try {
                    temp_bvh.Channels[j].SetKey(i, motions[index].Channels[j].Keys[simFrame] * i / blendTime + motions[current_index].Channels[j].Keys[f] * (blendTime - i) / blendTime);
                }
                catch {
                }
            }
        }

        for (int i = simFrame; i < motions[index].FrameCount; i++) {
            for (int j = 3; j < motions[index].Channels.Length; j++) {
                temp_bvh.Channels[j].SetKey(blendTime + i - simFrame, motions[index].Channels[j].Keys[i]);
            }
        }

        for (int i = 0; i < temp_bvh.FrameCount; i++) {
            //Vector3 off = new Vector3(0, 0, 0);
            //if (i + simF >= motions[1].FrameCount)
            //    off = origin;
            //else
            //{
            //    off = new Vector3(-oldM[0].Keys[0] + oldM[0].Keys[motions[1].FrameCount - 1],
            //        -oldM[1].Keys[0] + oldM[1].Keys[motions[1].FrameCount - 1],
            //        -oldM[2].Keys[0] + oldM[2].Keys[motions[1].FrameCount - 1]);
            //}
            temp_bvh.Channels[0].SetKey(i, thisavatar.transform.GetChild(0).gameObject.transform.position.x - origin.x + motions[index].Channels[0].Keys[simFrame - blendTime + i]);
            temp_bvh.Channels[1].SetKey(i, thisavatar.transform.GetChild(0).gameObject.transform.position.y - origin.y + motions[index].Channels[1].Keys[simFrame - blendTime + i]);
            temp_bvh.Channels[2].SetKey(i, thisavatar.transform.GetChild(0).gameObject.transform.position.z - origin.z + motions[index].Channels[2].Keys[simFrame - blendTime + i]);
        }

        ChordLength(temp_bvh);
        CurveFitting(temp_bvh, false, true);

        for (int i = 0; i < 4; i++) {
            controlpoint[i].transform.position = temp_bvh.ControlPoints[i];
            //controlpoint[i] = Instantiate(controlpoint[i]);
        }

        //ChangeControlPoint(temp_bvh);

        AnimationClip Animation = BvhAnimation.CreateAnimationClip(temp_bvh, scaling);
        Animation.name = thisavatar.name;
        Animation.legacy = true;
        Animation.wrapMode = WrapMode.Once;

        // Return to original motions[index] when mixture animation ends.
        AnimationEvent evt;
        evt = new AnimationEvent();
        //evt.time = temp_bvh.FrameCount / Animation.frameRate;
        evt.time = (float)temp_bvh.FrameTime.TotalSeconds * temp_bvh.FrameCount;
        Motion = index;
        evt.functionName = "domotion";
        Animation.AddEvent(evt);

        //var animation = null;
        //Destroy(thisavatar.GetComponent<Animation>());
        //animation = thisavatar.AddComponent<Animation>();
        animation.AddClip(Animation, Animation.name);
        animation.clip = Animation;
        animation.Play();
        Destroy(thisavatar.GetComponent<HumanPoseTransfer>());
        var humanPoseTransfer = thisavatar.AddComponent<HumanPoseTransfer>();
        humanPoseTransfer.Avatar = avatar;
    }

    public int GetCurrentFrame(int currentMotion, Vector3 currentPos)
    {
        for (int i = 0; i < motions[currentMotion].FrameCount; i++)
        {
            Vector3 pos = new Vector3();

            pos.x = motions[currentMotion].Channels[0].Keys[i];
            pos.y = motions[currentMotion].Channels[1].Keys[i];
            pos.z = motions[currentMotion].Channels[2].Keys[i];

            if ((currentPos - pos).magnitude < 0.1)
                return i;
        }
        return 0;
    }

    public int FindSimFrame(int currentFrame, Bvh currentM, Bvh targetM, int blendTime)
    {
        double min = 3000000;
        int minFrame = 0;
        for(int i = 0; i < targetM.FrameCount; i++)
        {
            double diff = 0;
            for(int j = 6; j < targetM.Channels.Length; j++)
            {
                diff += (currentM.Channels[j].Keys[currentFrame] - targetM.Channels[j].Keys[i]) * (currentM.Channels[j].Keys[currentFrame] - targetM.Channels[j].Keys[i]);
            }
            diff = Math.Sqrt(diff);
            if(diff < min)
            {
                min = diff;
                minFrame = i;
            }
        }
        return minFrame;
    }

    public void Add_Motions(List<Bvh> newMotion, float x = 0, float y = 0, float z = 0)
    {
        // index of last motion
        int original_motion = Motion;
        if (started == true)
        {
            Motion = motions.Count - 1;
            Motion++;
        }

        started = true;

        motions.AddRange(newMotion);

        //
        // Build hierarchy
        //
        //thisavatar = new GameObject(System.IO.Path.GetFileNameWithoutExtension(Path));
        if (bone == 0)
        {
            if (motions.Count == 1)
            {
                bool spine_exisit = false;
                foreach (var node in motions[Motion].Root.Traverse()) {
                    if(node.Name == "spine") {
                        spine_exisit = true;
                    }
                }
                if (!spine_exisit) {
                    foreach (var node in motions[Motion].Root.Traverse()) {
                        if (node.Name == "chest") {
                            //node.Name = "spine";
                        }
                    }
                }
                var hips = BuildHierarchy(thisavatar.transform, motions[Motion].Root, 1.0f); // GameObject hierarchy
                var skeleton = Skeleton.Estimate(hips); // Build the map of bones and index/name. 
                var description = AvatarDescription.Create(hips.Traverse().ToArray(), skeleton); // Link GameObject and the map

                var foot = hips.Traverse().Skip(skeleton.GetBoneIndex(HumanBodyBones.LeftFoot)).First();
                var hipHeight = hips.position.y - foot.position.y;
                // hips height to a meter
                scaling = 1.0f / hipHeight;
                foreach (var c in thisavatar.transform.Traverse())
                {
                    c.localPosition *= scaling;
                }
                var scaledHeight = hipHeight * scaling;

                float posx = motions[Motion].Channels[0].Keys[0];
                float posy = motions[Motion].Channels[1].Keys[0];
                float posz = motions[Motion].Channels[2].Keys[0];
                hips.position = new Vector3(posx, posy, posz); // foot to ground       

                for (int i = 0; i < motions[Motion].FrameCount; ++i)
                {
                    motions[Motion].Channels[0].SetKey(i, motions[Motion].Channels[0].Keys[i] * scaling);
                    motions[Motion].Channels[1].SetKey(i, motions[Motion].Channels[1].Keys[i] * scaling);
                    motions[Motion].Channels[2].SetKey(i, motions[Motion].Channels[2].Keys[i] * scaling);
                }

                // chordal method
                ChordLength(motions[Motion]);
                // To calculate control points and curve
                CurveFitting(motions[Motion], true, true);
                motions[Motion].cLength = CurveLength(motions[Motion]);
                motions[Motion].lPerFrame = motions[Motion].cLength / motions[Motion].FrameCount;
                List<List<Single3>> newOffset = new List<List<Single3>>(newMotion.Count);
                for (int i = 0; i < 4; i++)
                {
                    controlpoint[i].transform.position = motions[Motion].ControlPoints[i];
                    controlpoint[i] = Instantiate(controlpoint[i]);
                }

                ChangeCurveFitting(motions[Motion]);

                //
                // avatar
                //
                avatar = description.CreateAvatar(thisavatar.transform);
                avatar.name = thisavatar.name;
                AvatarDescription = description;//可不用
                animator = thisavatar.AddComponent<Animator>();
                animator.avatar = avatar;



                //
                // create AnimationClip
                //
                Animation = BvhAnimation.CreateAnimationClip(motions[Motion], scaling);
                Animation.name = thisavatar.name;
                Animation.legacy = true;
                Animation.wrapMode = WrapMode.Loop;
                Destroy(thisavatar.GetComponent<Animation>());
                animation = thisavatar.AddComponent<Animation>();
                animation.AddClip(Animation, Animation.name);
                animation.clip = Animation;
                animation.Play();
                Destroy(thisavatar.GetComponent<HumanPoseTransfer>());
                var humanPoseTransfer = thisavatar.AddComponent<HumanPoseTransfer>();
                humanPoseTransfer.Avatar = avatar;

                // create SkinnedMesh for bone visualize創建皮膚網格物體以使骨骼可視化
                var renderer = SkeletonMeshUtility.CreateRenderer(animator);
                Material = new Material(Shader.Find("Standard"));
                renderer.sharedMaterial = Material;
                Mesh = renderer.sharedMesh;
                Mesh.name = "box-man";

                thisavatar.AddComponent<BoneMapping>();
                var src = thisavatar.AddComponent<HumanPoseTransfer>();
                m_dst.SourceType = HumanPoseTransfer.HumanPoseTransferSourceType.HumanPoseTransfer;
                m_dst.Source = src;

            }

            else
            {
                for (int i = 0; i < motions[Motion].FrameCount; ++i)
                {
                    motions[Motion].Channels[0].SetKey(i, motions[Motion].Channels[0].Keys[i] * scaling);
                    motions[Motion].Channels[1].SetKey(i, motions[Motion].Channels[1].Keys[i] * scaling);
                    motions[Motion].Channels[2].SetKey(i, motions[Motion].Channels[2].Keys[i] * scaling);
                }


                ChordLength(motions[Motion]);
                CurveFitting(motions[Motion], true, false);
                motions[Motion].cLength = CurveLength(motions[Motion]);
                motions[Motion].lPerFrame = motions[Motion].cLength / motions[Motion].FrameCount;
                List<List<Single3>> newOffset = new List<List<Single3>>(newMotion.Count);
                //for (int i = 0; i < 4; i++)
                //{
                //    controlpoint[i].transform.position = motions[Motion].ControlPoints[i];
                //    controlpoint[i] = Instantiate(controlpoint[i]);
                //}

                ChangeCurveFitting(motions[Motion]);
            }
        }
        Motion = original_motion;
    }



    //建立骨架
    public void Add_Bone() { ROOT(); }
    //GameObject super;
    public void ROOT()
    {
        GameObject root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        root.transform.position = new Vector3(motions[0].Root.Offset.x, motions[0].Root.Offset.y, motions[0].Root.Offset.z);
        root.gameObject.transform.parent = this.transform;
        root.name = motions[0].Root.Name;
        super = root;
        foreach (BvhNode descentant in motions[0].Root.Children)
        {
            super = root;
            bones(descentant);
        }

        Debug.Log("Bvh.Root.Offset" + motions[0].Root.Offset);
        //Debug.Log("Bvh.Root.Channel" + Bvh.Root.Channels);
    }

    public void bones(BvhNode child)
    {
        GameObject bone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bone.transform.parent = super.transform;
        bone.transform.Translate(new Vector3(child.Offset.x, child.Offset.y, child.Offset.z) + super.transform.position);
        bone.name = child.Name;
        DrawLS(super, bone);
        super = bone;
        if (child.Children != null)
        {
            foreach (BvhNode descentant in child.Children)
            {
                super = bone;
                bones(descentant);
            }
        }
        else if (child.Children == null)
        {
            super = null;
        }
    }
    //建立骨架連結
    void DrawLS(GameObject startP, GameObject finalP)
    {
        Vector3 rightPosition = (startP.transform.position + finalP.transform.position) / 2;
        Vector3 rightRotation = finalP.transform.position - startP.transform.position;
        float HalfLength = Vector3.Distance(startP.transform.position, finalP.transform.position) / 2;
        float LThickness = 0.1f;//粗?


        GameObject link = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        link.gameObject.transform.parent = this.transform;
        link.transform.position = rightPosition;
        link.transform.rotation = Quaternion.FromToRotation(Vector3.up, rightRotation);
        link.transform.localScale = new Vector3(LThickness, HalfLength, LThickness);
        link.name = "link";

    }


    // BVH to Unity
    private Quaternion fromEulerZXY(Vector3 euler)
    {
        return Quaternion.AngleAxis(euler.z, Vector3.forward) * Quaternion.AngleAxis(euler.x, Vector3.right) * Quaternion.AngleAxis(euler.y, Vector3.up);
    }

    static Transform BuildHierarchy(Transform parent, BvhNode node, float toMeter)
    {
        var game_object = new GameObject(node.Name);
        game_object.transform.localPosition = node.Offset.ToXReversedVector3() * toMeter;//反矩陣
        game_object.transform.SetParent(parent, false);

        //var gizmo = go.AddComponent<BoneGizmoDrawer>();
        //gizmo.Draw = true;

        foreach (var child in node.Children)
        {
            BuildHierarchy(game_object.transform, child, toMeter);
        }

        return game_object.transform;
    }

    void Cal_Origin_Path(Bvh bvh)
    {
        // a, c取值範圍
        const double MIN_a = -2768.0;
        const double MAX_a = 2768.0;
        const double MIN_c = -2768.0;
        const double MAX_c = 2768.0;
        // 遞增值
        const double INC = 0.01;

        //z = a * x + c
        double x_pos = bvh.Root.Offset.x;
        double z_pos = bvh.Root.Offset.z;

        int frameCount = bvh.FrameCount;
        double[] x_pts = new double[frameCount];
        double[] z_pts = new double[frameCount];

        double m_a = 0, m_c = 0;

        for (int i = 0; i < frameCount; i++)
        {
            x_pts[i] = bvh.Channels[0].Keys[i];
            z_pts[i] = bvh.Channels[2].Keys[i];

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = new Vector3(Convert.ToSingle(x_pts[i]), 0, Convert.ToSingle(z_pts[i]));
        }

        double minCost = CaculateCost(m_a, m_c, x_pts, z_pts, frameCount);
        double curCost = 0.0;

        // 先計算最佳的a
        for (double a = MIN_a; a <= MAX_a; a += INC)
        {
            curCost = CaculateCost(a, m_c, x_pts, z_pts, frameCount);
            if (curCost < minCost)
            {
                m_a = a;
                minCost = curCost;
            }
        }

        // 再計算最佳的c
        for (double c = MIN_c; c <= MAX_c; c += INC)
        {
            curCost = CaculateCost(m_a, c, x_pts, z_pts, frameCount);
            if (curCost < minCost)
            {
                m_c = c;
                minCost = curCost;
            }
        }

        //L = a * x + b * z + c = 0;
        double m_b = -1;
        double MAX_X = Double.MinValue, MAX_Z = Double.MinValue;
        double MIN_X = Double.MaxValue, MIN_Z = Double.MaxValue;
        double[] T_xs = new double[frameCount];
        double[] T_zs = new double[frameCount];
        for (int i = 0; i < frameCount; i++)
        {
            //T = (x0 + at, z0 + bt)
            double t = -(m_a * x_pts[i] + m_b * z_pts[i] + m_c) / (m_a * m_a + m_b * m_b);
            double T_x = x_pts[i] + m_a * t;
            double T_z = z_pts[i] + m_b * t;

            T_xs[i] = T_x;
            T_zs[i] = T_z;

            MAX_X = MAX_X < T_x ? T_x : MAX_X;
            MIN_X = MIN_X > T_x ? T_x : MIN_X;
            MAX_Z = MAX_Z < T_z ? T_z : MAX_Z;
            MIN_Z = MIN_Z > T_z ? T_z : MIN_Z;

            double vector_x = m_a * t;
            double vector_z = m_b * t;
            double dis = Math.Sqrt(vector_x * vector_x + vector_z * vector_z);
            Offset_Vector newOffset = new Offset_Vector();
            newOffset.x = vector_x / dis;
            newOffset.z = vector_z / dis;
            newOffset.length = dis;
            //bvh.Offset_Vectors.Add(newOffset);
        }

        if (m_a >= 0)
        {
            Pt2D start, end;
            start.x = MIN_X;
            start.z = MIN_Z;
            end.x = MAX_X;
            end.z = MAX_Z;
            bvh.Origin_Start = start;
            bvh.Origin_End = end;
        }
        else
        {
            Pt2D start, end;
            start.x = MIN_X;
            start.z = MAX_Z;
            end.x = MAX_X;
            end.z = MIN_Z;
            bvh.Origin_Start = start;
            bvh.Origin_End = end;
        }
        Debug.DrawLine(new Vector3(Convert.ToSingle(bvh.Origin_Start.x), 5.0f, Convert.ToSingle(bvh.Origin_Start.z)), new Vector3(Convert.ToSingle(bvh.Origin_End.x), 0.0f, Convert.ToSingle(bvh.Origin_End.z)), Color.green, 214748364);

        double x_intercept = bvh.Origin_Start.x - bvh.Origin_End.x;
        double z_intercept = bvh.Origin_Start.z - bvh.Origin_End.z;
        double pathLen = Math.Sqrt(x_intercept * x_intercept + z_intercept * z_intercept);
        /*for(int i = 0; i < frameCount; i++)
        {
            Offset_Vector newOffset = bvh.Offset_Vectors[i];
            x_intercept = T_xs[i] - bvh.Origin_Start.x;
            z_intercept = T_zs[i] - bvh.Origin_Start.z;
            double len = Math.Sqrt(x_intercept * x_intercept + z_intercept * z_intercept);
            newOffset.t = len / pathLen;
            bvh.Offset_Vectors[i] = newOffset;
        }*/
    }

    double CaculateCost(double a, double b, double[] x_pts, double[] y_pts, int size)
    {
        double cost = 0.0;
        double xReal = 0.0;
        double yReal = 0.0;
        double yPredict = 0.0;
        double yDef = 0.0;
        for (int i = 0; i < size; ++i)
        {
            // x實際值
            xReal = x_pts[i];
            // y實際值
            yReal = y_pts[i];
            // y預測值
            yPredict = a * xReal + b;

            yDef = yPredict - yReal;
            // 累加方差
            cost += (yDef * yDef);
        }
        return cost;
    }

    double ChordLength(Bvh bvh)
    {
        int frameCount = bvh.FrameCount;
        float[] prmtrs = new float[frameCount];

        double sum = 0;
        double[] accumulate = new double[frameCount];

        accumulate[0] = 0;
        for (int i = 1; i < frameCount; i++)
        {
            sum += CalDistance(bvh.Channels[0].Keys[i - 1], bvh.Channels[2].Keys[i - 1], bvh.Channels[0].Keys[i], bvh.Channels[2].Keys[i]);
            accumulate[i] = sum;
        }

        prmtrs[0] = 0;
        prmtrs[frameCount - 1] = 1;
        for (int i = 1; i < frameCount - 1; i++)
        {
            prmtrs[i] = Convert.ToSingle(accumulate[i] / sum);
        }

        bvh.Params = prmtrs;
        return sum;
    }

    double CurveLength(Bvh bvh)
    {
        int frameCount = bvh.FrameCount;

        double sum = 0;
        for (int i = 1; i < frameCount; i++)
        {
            sum += CalDistance(bvh.Channels[0].Keys[i - 1], bvh.Channels[2].Keys[i - 1], bvh.Channels[0].Keys[i], bvh.Channels[2].Keys[i]);
        }

        return sum;
    }

    double CalDistance(float x1, float y1, float x2, float y2)
    {
        float _x = x1 - x2, _y = y1 - y2;
        return Math.Sqrt(_x * _x + _y * _y);
    }

    void CurveFitting(Bvh bvh, bool first_time, bool redraw)
    {
        int frameCount = bvh.FrameCount;
        float[][] Q = MatrixCreate(frameCount, 3);
        //float[][] R = MatrixCreate(frameCount, 3);
        float[] x_pts = new float[frameCount];
        float[] y_pts = new float[frameCount];
        float[] z_pts = new float[frameCount];
        //float[] x_r = new float[frameCount];
        //float[] y_r = new float[frameCount];
        //float[] z_r = new float[frameCount];

        for (int i = 0; i < frameCount; i++)
        {
            Q[i][0] = bvh.Channels[0].Keys[i];
            Q[i][1] = bvh.Channels[1].Keys[i];
            Q[i][2] = bvh.Channels[2].Keys[i];
            //R[i][0] = bvh.Channels[4].Keys[i];
            //R[i][1] = bvh.Channels[5].Keys[i];
            //R[i][2] = bvh.Channels[6].Keys[i];
        }
        float B3_0(float t) => 0.16667f * (1 - t) * (1 - t) * (1 - t);
        float B3_1(float t) => 0.16667f * (3 * t * t * t - 6 * t * t + 4);
        float B3_2(float t) => 0.16667f * (-3 * t * t * t + 3 * t * t + 3 * t + 1);
        float B3_3(float t) => 0.16667f * t * t * t;

        float[][] A = MatrixCreate(4, 4);
        float[][] p = MatrixCreate(4, 3);

        for (int i = 0; i < frameCount; i++)
        {
            float B0 = B3_0(bvh.Params[i]);
            float B1 = B3_1(bvh.Params[i]);
            float B2 = B3_2(bvh.Params[i]);
            float B3 = B3_3(bvh.Params[i]);

            A[0][0] += B0 * B0;
            A[0][1] += B0 * B1;
            A[0][2] += B0 * B2;
            A[0][3] += B0 * B3;
            A[1][1] += B1 * B1;
            A[1][2] += B1 * B2;
            A[1][3] += B1 * B3;

            A[2][2] += B2 * B2;
            A[2][3] += B2 * B3;

            A[3][3] += B3 * B3;
        }

        A[1][0] = A[0][1];

        A[2][0] = A[0][2];
        A[2][1] = A[1][2];

        A[3][0] = A[0][3];
        A[3][1] = A[1][3];
        A[3][2] = A[2][3];

        for (int dimension = 0; dimension < 3; dimension++)
        {
            float[][] b = MatrixCreate(4, 1);
            for (int i = 0; i < frameCount; i++)
            {
                b[0][0] += B3_0(bvh.Params[i]) * Q[i][dimension];
                b[1][0] += B3_1(bvh.Params[i]) * Q[i][dimension];
                b[2][0] += B3_2(bvh.Params[i]) * Q[i][dimension];
                b[3][0] += B3_3(bvh.Params[i]) * Q[i][dimension];
            }
            float[][] x = MatrixProduct(MatrixInverse(A), b);
            p[0][dimension] = x[0][0];
            p[1][dimension] = x[1][0];
            p[2][dimension] = x[2][0];
            p[3][dimension] = x[3][0];
        }

        for (int i = 0; i < 4; i++)
        {
            bvh.ControlPoints[i].x = p[i][0];
            bvh.ControlPoints[i].y = p[i][1];
            bvh.ControlPoints[i].z = p[i][2];
        }

        if (redraw) {
            while (Points.Count != 0) {
                GameObject step = Points[0];
                Points.Remove(step);
                Destroy(step);
            }

            int frames_per_sphere = (int)(1 / bvh.FrameTime.TotalSeconds / 10);
            frames_per_sphere = first_time ? 60 : 1;
            for (int i = 0; i < frameCount; i += frames_per_sphere) {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                float t = bvh.Params[i];
                Vector3 pos = new Vector3();
                pos.x = B3_0(t) * bvh.ControlPoints[0].x + B3_1(t) * bvh.ControlPoints[1].x + B3_2(t) * bvh.ControlPoints[2].x + B3_3(t) * bvh.ControlPoints[3].x;
                pos.y = B3_0(t) * bvh.ControlPoints[0].y + B3_1(t) * bvh.ControlPoints[1].y + B3_2(t) * bvh.ControlPoints[2].y + B3_3(t) * bvh.ControlPoints[3].y;
                pos.z = B3_0(t) * bvh.ControlPoints[0].z + B3_1(t) * bvh.ControlPoints[1].z + B3_2(t) * bvh.ControlPoints[2].z + B3_3(t) * bvh.ControlPoints[3].z;
                sphere.transform.position = pos;
                sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                Points.Add(sphere);
            }
        }
        return;
    }

    float DeBoorValue(int index/*i*/, int degree/*p*/, float u, float[] knots)
    {
        if (degree == 0)
        {
            if (u >= knots[index] && u < knots[index + 1])
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        float first = 0;
        float second = 0;
        float first_DeBoor = DeBoorValue(index, degree - 1, u, knots);
        float second_DeBoor = DeBoorValue(index + 1, degree - 1, u, knots);
        float first_coef = (u - knots[index]) / (knots[index + degree] - knots[index]);
        float second_coef = (knots[index + degree + 1] - u) / (knots[index + degree + 1] - knots[index + 1]);

        if (first_DeBoor != 0)
        {
            first = first_coef * first_DeBoor;
        }

        if (second_DeBoor != 0)
        {
            second = second_coef * second_DeBoor;
        }

        float ans = first + second;
        return ans;
    }

    float[] ComputeCoef(int n, int p, int m, float u, float[] knots)
    {
        float[] N = new float[n + 1];
        for (int i = 0; i <= n; i++)
        {
            N[i] = 0;
        }

        if (u == knots[0])
        {
            N[0] = 0;
            return N;
        }
        else if (u == knots[m])
        {
            N[n] = 1;
            return N;
        }

        int k = 0;
        for (; k < m; k++)
        {
            if (u < knots[k])
            {
                k--;
                break;
            }
        }
        N[k] = 1.0f;
        for (int d = 1; d <= p; d++)
        {
            N[k - d] = (knots[k + 1] - u) / (knots[k + 1] - knots[k - d + 1]) * N[k - d + 1];
            for (int i = k - d + 1; i <= k - 1; i++)
            {
                N[i] = (u - knots[i]) / (knots[i + d] - knots[i]) * N[i] + (knots[i + d + 1] - u) / (knots[i + d + 1] - knots[i + 1]) * N[i + 1];
            }

            N[k] = (u - knots[k]) / (knots[k + d] - knots[k]) * N[k];
        }

        return N;
    }

    static float[][] MatrixCreate(int rows, int cols)
    {
        float[][] result = new float[rows][];
        for (int i = 0; i < rows; ++i)
        {
            result[i] = new float[cols];
        }

        return result;
    }

    static float[][] MatrixIdentity(int n)
    {
        // return an n x n Identity matrix
        float[][] result = MatrixCreate(n, n);
        for (int i = 0; i < n; ++i)
        {
            result[i][i] = 1.0f;
        }

        return result;
    }

    static float[][] MatrixProduct(float[][] matrixA, float[][] matrixB)
    {
        int aRows = matrixA.Length; int aCols = matrixA[0].Length;
        int bRows = matrixB.Length; int bCols = matrixB[0].Length;
        if (aCols != bRows)
        {
            throw new Exception("Non-conformable matrices in MatrixProduct");
        }

        float[][] result = MatrixCreate(aRows, bCols);

        for (int i = 0; i < aRows; ++i) // each row of A
        {
            for (int j = 0; j < bCols; ++j) // each col of B
            {
                for (int k = 0; k < aCols; ++k) // could use k less-than bRows
                {
                    result[i][j] += matrixA[i][k] * matrixB[k][j];
                }
            }
        }

        return result;
    }

    static float[][] MatrixInverse(float[][] matrix)
    {
        int n = matrix.Length;
        float[][] result = MatrixDuplicate(matrix);

        int[] perm;
        int toggle;
        float[][] lum = MatrixDecompose(matrix, out perm,
          out toggle);
        if (lum == null)
        {
            throw new Exception("Unable to compute inverse");
        }

        float[] b = new float[n];
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                if (i == perm[j])
                {
                    b[j] = 1.0f;
                }
                else
                {
                    b[j] = 0.0f;
                }
            }

            float[] x = HelperSolve(lum, b);

            for (int j = 0; j < n; ++j)
            {
                result[j][i] = x[j];
            }
        }
        return result;
    }

    static float[][] MatrixDuplicate(float[][] matrix)
    {
        // allocates/creates a duplicate of a matrix.
        float[][] result = MatrixCreate(matrix.Length, matrix[0].Length);
        for (int i = 0; i < matrix.Length; ++i) // copy the values
        {
            for (int j = 0; j < matrix[i].Length; ++j)
            {
                result[i][j] = matrix[i][j];
            }
        }

        return result;
    }

    static float[] HelperSolve(float[][] luMatrix, float[] b)
    {
        // before calling this helper, permute b using the perm array
        // from MatrixDecompose that generated luMatrix
        int n = luMatrix.Length;
        float[] x = new float[n];
        b.CopyTo(x, 0);

        for (int i = 1; i < n; ++i)
        {
            float sum = x[i];
            for (int j = 0; j < i; ++j)
            {
                sum -= luMatrix[i][j] * x[j];
            }

            x[i] = sum;
        }

        x[n - 1] /= luMatrix[n - 1][n - 1];
        for (int i = n - 2; i >= 0; --i)
        {
            float sum = x[i];
            for (int j = i + 1; j < n; ++j)
            {
                sum -= luMatrix[i][j] * x[j];
            }

            x[i] = sum / luMatrix[i][i];
        }

        return x;
    }

    static float[][] MatrixDecompose(float[][] matrix, out int[] perm, out int toggle)
    {
        // Doolittle LUP decomposition with partial pivoting.
        // rerturns: result is L (with 1s on diagonal) and U;
        // perm holds row permutations; toggle is +1 or -1 (even or odd)
        int rows = matrix.Length;
        int cols = matrix[0].Length; // assume square
        if (rows != cols)
        {
            throw new Exception("Attempt to decompose a non-square m");
        }

        int n = rows; // convenience

        float[][] result = MatrixDuplicate(matrix);

        perm = new int[n]; // set up row permutation result
        for (int i = 0; i < n; ++i) { perm[i] = i; }

        toggle = 1; // toggle tracks row swaps.
                    // +1 -greater-than even, -1 -greater-than odd. used by MatrixDeterminant

        for (int j = 0; j < n - 1; ++j) // each column
        {
            float colMax = Math.Abs(result[j][j]); // find largest val in col
            int pRow = j;
            //for (int i = j + 1; i less-than n; ++i)
            //{
            //  if (result[i][j] greater-than colMax)
            //  {
            //    colMax = result[i][j];
            //    pRow = i;
            //  }
            //}

            // reader Matt V needed this:
            for (int i = j + 1; i < n; ++i)
            {
                if (Math.Abs(result[i][j]) > colMax)
                {
                    colMax = Math.Abs(result[i][j]);
                    pRow = i;
                }
            }
            // Not sure if this approach is needed always, or not.

            if (pRow != j) // if largest value not on pivot, swap rows
            {
                float[] rowPtr = result[pRow];
                result[pRow] = result[j];
                result[j] = rowPtr;

                int tmp = perm[pRow]; // and swap perm info
                perm[pRow] = perm[j];
                perm[j] = tmp;

                toggle = -toggle; // adjust the row-swap toggle
            }

            // --------------------------------------------------
            // This part added later (not in original)
            // and replaces the 'return null' below.
            // if there is a 0 on the diagonal, find a good row
            // from i = j+1 down that doesn't have
            // a 0 in column j, and swap that good row with row j
            // --------------------------------------------------

            if (result[j][j] == 0.0)
            {
                // find a good row to swap
                int goodRow = -1;
                for (int row = j + 1; row < n; ++row)
                {
                    if (result[row][j] != 0.0)
                    {
                        goodRow = row;
                    }
                }

                if (goodRow == -1)
                {
                    throw new Exception("Cannot use Doolittle's method");
                }

                // swap rows so 0.0 no longer on diagonal
                float[] rowPtr = result[goodRow];
                result[goodRow] = result[j];
                result[j] = rowPtr;

                int tmp = perm[goodRow]; // and swap perm info
                perm[goodRow] = perm[j];
                perm[j] = tmp;

                toggle = -toggle; // adjust the row-swap toggle
            }
            // --------------------------------------------------
            // if diagonal after swap is zero . .
            //if (Math.Abs(result[j][j]) less-than 1.0E-20) 
            //  return null; // consider a throw

            for (int i = j + 1; i < n; ++i)
            {
                result[i][j] /= result[j][j];
                for (int k = j + 1; k < n; ++k)
                {
                    result[i][k] -= result[i][j] * result[j][k];
                }
            }


        } // main j column loop

        return result;
    }

    static float[][] MatrixTranspose(float[][] matrix)
    {
        float[][] result = MatrixCreate(matrix[0].Length, matrix.Length);
        for (int i = 0; i < matrix.Length; ++i) // copy the values
        {
            for (int j = 0; j < matrix[i].Length; ++j)
            {
                result[j][i] = matrix[i][j];
            }
        }

        return result;
    }
}
