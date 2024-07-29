
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BvhBone : IBone
{
    public string Name
    {
        private set;
        get;
    }

    public Vector3 SkeletonLocalPosition
    {
        private set;
        get;
    }

    public BvhBone(string name, Vector3 position)//設定骨架原始位置及名字
    {
        Name = name;                //骨架名稱
        SkeletonLocalPosition = position;//骨架原始位置
    }

    public override string ToString()
    {
        return string.Format("<BvhBone: {0}>", Name);
    }

    public IBone Parent
    {
        private set;
        get;
    }

    List<IBone> _children = new List<IBone>();
    public IList<IBone> Children
    {
        get { return _children; }
    }

    public void Build(Transform t) //在這個transform下設定骨架原始位置及名字
    {
        foreach (Transform child in t)
        {
            var childBone = new BvhBone(child.name, SkeletonLocalPosition + child.localPosition);
            childBone.Parent = this;
            _children.Add(childBone);

            childBone.Build(child);
        }
    }

    public void Build(BvhNode node)
    {
        foreach (var child in node.Children)
        {
            var childBone = new BvhBone(child.Name, SkeletonLocalPosition + child.Offset.ToXReversedVector3());
            childBone.Parent = this;
            _children.Add(childBone);

            childBone.Build(child);
        }
    }

    public IEnumerable<BvhBone> Traverse()
    {
        yield return this;
        foreach (var child in Children)
        {
            foreach (var x in child.Traverse())
            {
                yield return (BvhBone)x;
            }
        }
    }
}