using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class BvhException : Exception
{
    public BvhException(string msg) : base(msg) { }
}

public enum Channel
{
    Xposition,
    Yposition,
    Zposition,
    Xrotation,
    Yrotation,
    Zrotation,
}

//
public static class ChannelExtensions
{
    public static string ToProperty(this Channel ch)
    {
        switch (ch)
        {
            case Channel.Xposition: return "localPosition.x";
            case Channel.Yposition: return "localPosition.y";
            case Channel.Zposition: return "localPosition.z";
            case Channel.Xrotation: return "localEulerAnglesBaked.x";
            case Channel.Yrotation: return "localEulerAnglesBaked.y";
            case Channel.Zrotation: return "localEulerAnglesBaked.z";
        }

        throw new BvhException("no property for " + ch);
    }

    public static bool IsLocation(this Channel ch)
    {
        switch (ch)
        {
            case Channel.Xposition:
            case Channel.Yposition:
            case Channel.Zposition: return true;
            case Channel.Xrotation:
            case Channel.Yrotation:
            case Channel.Zrotation: return false;
        }

        throw new BvhException("no property for " + ch);
    }
}
//

public struct Single3
{
    public Single x;
    public Single y;
    public Single z;

    public Single3(Single _x, Single _y, Single _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }
}

public struct Offset_Vector
{
    public double x;
    public double z;
    public double length;
    public double t;
}

public struct Pt2D
{
    public double x;
    public double z;
}

public class BvhNode
{
    public String Name
    {
        get;
        protected set;
    }

    public Single3 Offset
    {
        get;
        protected set;
    }

    public void SetOffset(Single3 offset)
    {
        Offset = offset;
    }

    public Channel[] Channels
    {
        get;
        private set;
    }

    public List<BvhNode> Children
    {
        get;
        private set;
    }

    public BvhNode(string name)
    {
        Name = name;
        Children = new List<BvhNode>();
    }

    public virtual void Parse(StringReader reader)
    {
        string line = "";
        while (line == "")
            line = reader.ReadLine();
        Offset = ParseOffset(line);
        line = "";
        while (line == "")
            line = reader.ReadLine();
        Channels = ParseChannel(line);
    }

    protected static Single3 ParseOffset(string line)
    {
        string[] splited = line.Trim().Split();
        if (splited[0] != "OFFSET")
        {
            throw new BvhException("OFFSET not found");
        }
        //跳過第一項"OFFSET" 剩下不為空字元的字串轉乘float arrays
        float[] offset = splited.Skip(1).Where(x => !string.IsNullOrEmpty(x)).Select(x => float.Parse(x)).ToArray();
        return new Single3(offset[0], offset[1], offset[2]);
    }

    static Channel[] ParseChannel(string line)
    {
        string[] splited = line.Trim().Split();
        if (splited[0] != "CHANNELS")
        {
            throw new BvhException("CHANNELS not found");
        }
        //channel數
        int count = int.Parse(splited[1]);
        //首項"CHANNELS" 次項"channel數"
        if (count + 2 != splited.Length)
        {
            throw new BvhException("channel count is not match with splited count");
        }
        //跳過前兩項 剩下照順序轉成arrays
        return splited.Skip(2).Select(x => (Channel)Enum.Parse(typeof(Channel), x)).ToArray();
    }

    /// <summary>
    /// 取得自己及所有的children
    /// </summary>
    /// <returns></returns>
    public IEnumerable<BvhNode> Traverse()
    {
        //yield return的做法為當遇到符合條件的元素時，即刻將該元素回傳回上一層進行後續運算，後續運算完後再回到迴圈中找尋下一個元素。
        yield return this;

        foreach (BvhNode child in Children)
        {
            foreach (BvhNode descentant in child.Traverse())
            {

                yield return descentant;
            }
        }
    }
}

public class EndSite : BvhNode
{
    public EndSite() : base("")
    {
        Name = "End";
    }

    public override void Parse(StringReader reader)
    {
        Offset = ParseOffset(reader.ReadLine());
    }
}

public class ChannelCurve
{
    public float[] Keys//key frame的key
    {
        get;
        set;
    }

    public ChannelCurve(int frameCount)
    {
        Keys = new float[frameCount];
    }

    public ChannelCurve(ChannelCurve origin) {
        Keys = new List<float>(origin.Keys).ToArray();
    }

    public ChannelCurve(ChannelCurve origin, int clip_length) {
        Keys = new List<float>(origin.Keys).GetRange(0, clip_length).ToArray();
    }

    public void SetKey(int frame, float value)
    {
        Keys[frame] = value;
    }
}

public class Bvh
{
    Vector3[] rootPos;
    /*Property*/
    public BvhNode Root
    {
        get;
        private set;
    }
    //public BvhNode EndSite
    public TimeSpan FrameTime
    {
        get;
        set;
    }

    public ChannelCurve[] Channels
    {
        get;
        set;
    }

    public int m_frames;
    public int FrameCount
    {
        get { return m_frames; }
    }

    public float[] Params
    {
        get;
        set;
    }
    
    public Vector3[] ControlPoints
    {
        get;
        set;
    }
    public double lPerFrame;
    public double cLength;

    //
    public struct PathWithProperty//屬性路徑
    {
        public string Path;
        public string Property;
        public bool IsLocation;
    }

    public Bvh DeepCopyAnimation() {
        //Bvh other = (Bvh)this.MemberwiseClone();
        Bvh other = new Bvh(this.Root, this.m_frames, (float)this.FrameTime.TotalSeconds);
        //other.Channels = new List<ChannelCurve>(this.Channels).ToArray();
        other.Channels = new ChannelCurve[this.Channels.Length];
        for(int i = 0; i < Channels.Length; i++) {
            other.Channels[i] = new ChannelCurve(this.Channels[i]);
        }
        //other.Params = new List<float>(this.Params).ToArray();
        other.Params = new float[this.Params.Length];
        this.Params.CopyTo(other.Params, 0);
        //other.ControlPoints = new List<Vector3>(this.ControlPoints).ToArray();
        other.ControlPoints = new Vector3[this.ControlPoints.Length];
        this.ControlPoints.CopyTo(other.ControlPoints, 0);
        return other;
    }

    public Bvh DeepCopyClip(int clip_length) {
        if(clip_length > this.m_frames) {
            return null;
        }

        //Bvh other = (Bvh)this.MemberwiseClone();
        Bvh other = new Bvh(this.Root, this.m_frames, (float)this.FrameTime.TotalSeconds);
        //other.Channels = new List<ChannelCurve>(this.Channels).ToArray();
        other.Channels = new ChannelCurve[this.Channels.Length];
        for (int i = 0; i < Channels.Length; i++) {
            other.Channels[i] = new ChannelCurve(this.Channels[i], clip_length);
        }
        //other.Params = new List<float>(this.Params).ToArray();
        other.Params = new float[this.Params.Length];
        this.Params.CopyTo(other.Params, 0);
        //other.ControlPoints = new List<Vector3>(this.ControlPoints).ToArray();
        other.ControlPoints = new Vector3[this.ControlPoints.Length];
        this.ControlPoints.CopyTo(other.ControlPoints, 0);
        other.lPerFrame = this.lPerFrame;
        return other;
    }

    public bool TryGetPathWithPropertyFromChannel(ChannelCurve channel, out PathWithProperty pathWithProp)//嘗試從渠道(ChannelCurve)獲取具有屬性的路徑
    {
        var index = Channels.ToList().IndexOf(channel);//從ChannelCurve列表提取每個channel做為路徑
        if (index == -1)
        {
            pathWithProp = default(PathWithProperty);
            return false;
        }

        foreach (var node in Root.Traverse())
        {
            for (int i = 0; i < node.Channels.Length; ++i, --index)
            {
                if (index == 0)
                {
                    pathWithProp = new PathWithProperty
                    {
                        Path = GetPath(node),
                        Property = node.Channels[i].ToProperty(),
                        IsLocation = node.Channels[i].IsLocation(),
                    };
                    return true;
                }
            }
        }

        throw new BvhException("channel is not found");
    }

    public string GetPath(BvhNode node)
    {
        var list = new List<string>() { node.Name };

        var current = node;
        while (current != null)
        {
            current = GetParent(current);
            if (current != null)
            {
                list.Insert(0, current.Name);
            }
        }

        return String.Join("/", list.ToArray());
    }

    BvhNode GetParent(BvhNode node)
    {
        foreach (var x in Root.Traverse())
        {
            if (x.Children.Contains(node))
            {
                return x;
            }
        }

        return null;
    }

    public ChannelCurve GetChannel(BvhNode target, Channel channel)
    {
        var index = 0;
        foreach (var node in Root.Traverse())
        {
            for (int i = 0; i < node.Channels.Length; ++i, ++index)
            {
                if (node == target && node.Channels[i] == channel)
                {
                    return Channels[index];
                }
            }
        }

        throw new BvhException("channel is not found");
    }
    //
    public List<Offset_Vector> Offset_Vectors
    {
        get;
        set;
    }

    public Pt2D Origin_Start
    {
        get;
        set;
    }

    public Pt2D Origin_End
    {
        get;
        set;
    }

    /*Counstructer*/
    public Bvh(BvhNode root, int frames, float seconds)
    {
        Root = root;
        FrameTime = TimeSpan.FromSeconds(seconds);
        m_frames = frames;
        ControlPoints = new Vector3[4];

        int channelCount = Root.Traverse()//所有children
            .Where(x => x.Channels != null)
            .Select(x => x.Channels.Length)//所有node的channels數總和
            .Sum();
        //"channelCount"個channel 一個channel有"frames"幀
        Channels = Enumerable.Range(0, channelCount)
            .Select(x => new ChannelCurve(frames))
            .ToArray()
            ;
    }
    /*Method*/
    public static Bvh Parse(string src, Vector3 offset)
    {
        using (StringReader reader = new StringReader(src))
        {
            if (reader.ReadLine() != "HIERARCHY")
            {
                throw new BvhException("Doesn't start with HIERARCHY");
            }

            BvhNode root = ParseNode(reader); // Parse the bones of avatar
            if (root == null) 
            {
                return null;
            }

            Single3 o = new Single3(offset.x, offset.y, offset.z); // offset
            root.SetOffset(o);
            
            string nextLine = "";
            while (nextLine == "")
                nextLine = reader.ReadLine();

            int frames = 0;// numbers of frame
            float frameTime = 0.0f;// time of a frame

            if (nextLine == "MOTION")
            {
                //格式e.g."Frames: 2751"
                string[] frameSplited = reader.ReadLine().Split(':');
                if (frameSplited[0] != "Frames")
                {
                    throw new BvhException("Frames is not found");
                }
                frames = int.Parse(frameSplited[1]);

                //格式e.g."Frame Time: .0083333"
                string[] frameTimeSplited = reader.ReadLine().Split(':');
                if (frameTimeSplited[0] != "Frame Time")
                {
                    throw new BvhException("Frame Time is not found");
                }
                frameTime = float.Parse(frameTimeSplited[1]);
            }

            Bvh bvh = new Bvh(root, frames, frameTime);

            /*key frames*/
            for (int i = 0; i < frames; ++i)
            {
                string line = reader.ReadLine();
                bvh.ParseFrame(i, line);
            }
            return bvh;
        }
    }

    static BvhNode ParseNode(StringReader reader, int level = 0)
    {
        //讀進第一行 Trim()去除頭尾white-space
        string firstLine = reader.ReadLine().Trim();
        while(firstLine == "")
            firstLine = reader.ReadLine().Trim();
        string[] splited = firstLine.Split();

        if (splited.Length != 2)//e.g. ROOT Hips
        {
            if (splited.Length == 1 && splited[0] == "}")//讀到node結尾
            {
                return null;
            }
            throw new BvhException(String.Format("splited to {0}({1})", splited.Length, splited));
        }

        BvhNode node = null;
        if (splited[0] == "ROOT")
        {
            if (level != 0) // Not the first ROOT
            {
                throw new BvhException("nested ROOT");
            }
            node = new BvhNode(splited[1]);
        }
        else if (splited[0] == "JOINT")
        {
            if (level == 0) // First node should be ROOT
            {
                throw new BvhException("Should be ROOT, but JOINT");
            }
            node = new BvhNode(splited[1]);
        }
        else if (splited[0] == "End") //最後一個child 肢體末端
        {
            if (level == 0)
            {
                throw new BvhException("End in level 0");
            }
            node = new EndSite();
        }
        else
        {
            throw new BvhException("unknown type: " + splited[0]);
        }

        //讀完node名稱 要讀內容 start with "{"
        if (reader.ReadLine().Trim() != "{")
        {
            throw new BvhException("'{' is not found");
        }

        //讀node內容
        node.Parse(reader);

        // child nodes
        while (true)
        {
            BvhNode child = ParseNode(reader, level + 1);
            if (child == null)
            {
                break;
            }

            if (!(child is EndSite))
            {
                node.Children.Add(child);
            }
        }

        return node;
    }

    public void ParseFrame(int frame, string line)
    {
        //一行代表一個frame
        string[] splited = line.Trim().Split().Where(x => !string.IsNullOrEmpty(x)).ToArray();
        if (splited.Length != Channels.Length)
        {
            throw new BvhException("Key length doesn't match channel's length");
        }

        for (int i = 0; i < Channels.Length; ++i)
        {
            Channels[i].SetKey(frame, float.Parse(splited[i]));// values of each frame
        }
    }
}
