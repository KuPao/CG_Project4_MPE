using System;
using System.Collections.Generic;
using UnityEngine;



    public static class BvhAnimation
    {
        class CurveSet
        {
            BvhNode Node;
            Func<float, float, float, Quaternion> EulerToRotation;
            public CurveSet(BvhNode node)//讀取node
            {
                Node = node;
            }
            //節點曲線-位移
            public ChannelCurve PositionX;
            public ChannelCurve PositionY;
            public ChannelCurve PositionZ;
            public Vector3 GetPosition(int i)
            {
                return new Vector3(
                    PositionX.Keys[i],
                    PositionY.Keys[i],
                    PositionZ.Keys[i]);
            }
            //節點曲線-旋轉
            public ChannelCurve RotationX;
            public ChannelCurve RotationY;
            public ChannelCurve RotationZ;
            public Quaternion GetRotation(int i)//面向方向
            {
                if (EulerToRotation == null)
                {
                    EulerToRotation = Node.GetEulerToRotation();
                }
                return EulerToRotation(
                    RotationX.Keys[i],
                    RotationY.Keys[i],
                    RotationZ.Keys[i]
                    );
            }

            static void AddCurve(Bvh bvh, AnimationClip clip, ChannelCurve ch, float scaling)
            {
                if (ch == null) return;
                var pathWithProp = default(Bvh.PathWithProperty);//屬性路徑
                bvh.TryGetPathWithPropertyFromChannel(ch, out pathWithProp);//提取屬性路徑
                var curve = new AnimationCurve();//動畫曲線
                for (int i = 0; i < bvh.FrameCount; ++i)
                {
                    var time = (float)(i * bvh.FrameTime.TotalSeconds);
                    var value = ch.Keys[i];
                    //var value = ch.Keys[i] * scaling;// 獲取動畫曲線關鍵幀
                    curve.AddKey(time, value);//加入關鍵幀時間及個數
            }
                clip.SetCurve(pathWithProp.Path, typeof(Transform), pathWithProp.Property, curve);
            }

            public void AddCurves(Bvh bvh, AnimationClip clip, float scaling)//插入關鍵影格
            {
                AddCurve(bvh, clip, PositionX, -scaling);
                AddCurve(bvh, clip, PositionY, scaling);
                AddCurve(bvh, clip, PositionZ, scaling);

                var pathWithProp = default(Bvh.PathWithProperty);
                bvh.TryGetPathWithPropertyFromChannel(RotationX, out pathWithProp);

                // rotation
                var curveX = new AnimationCurve();
                var curveY = new AnimationCurve();
                var curveZ = new AnimationCurve();
                var curveW = new AnimationCurve();
                for (int i = 0; i < bvh.FrameCount; ++i)
                {
                    var time = (float)(i * bvh.FrameTime.TotalSeconds);
                    var q = GetRotation(i).ReverseX();
                    curveX.AddKey(time, q.x);
                    curveY.AddKey(time, q.y);
                    curveZ.AddKey(time, q.z);
                    curveW.AddKey(time, q.w);
                }
                clip.SetCurve(pathWithProp.Path, typeof(Transform), "localRotation.x", curveX);
                clip.SetCurve(pathWithProp.Path, typeof(Transform), "localRotation.y", curveY);
                clip.SetCurve(pathWithProp.Path, typeof(Transform), "localRotation.z", curveZ);
                clip.SetCurve(pathWithProp.Path, typeof(Transform), "localRotation.w", curveW);
            }
        }

        public static AnimationClip CreateAnimationClip(Bvh bvh, float scaling)
        {
            var clip = new AnimationClip();//動畫系統最小單位
            clip.legacy = true;//若有其它動畫會一起執行

            var curveMap = new Dictionary<BvhNode, CurveSet>();//Dictionary集合

            int j = 0;
            foreach (var node in bvh.Root.Traverse())
            {
                var curve_set = new CurveSet(node);
                curveMap[node] = curve_set;

                for (int i = 0; i < node.Channels.Length; ++i, ++j)
                {
                    var curve = bvh.Channels[j];
                    switch (node.Channels[i])
                    {
                        case Channel.Xposition: curve_set.PositionX = curve; break;
                        case Channel.Yposition: curve_set.PositionY = curve; break;
                        case Channel.Zposition: curve_set.PositionZ = curve; break;
                        case Channel.Xrotation: curve_set.RotationX = curve; break;
                        case Channel.Yrotation: curve_set.RotationY = curve; break;
                        case Channel.Zrotation: curve_set.RotationZ = curve; break;
                        default: throw new Exception();
                    }
                }
            }

            foreach (var curve_set in curveMap)
            {
                curve_set.Value.AddCurves(bvh, clip, scaling);//把bvh算出的值放入list中
            }

            clip.EnsureQuaternionContinuity();

            return clip;
        }
    }
