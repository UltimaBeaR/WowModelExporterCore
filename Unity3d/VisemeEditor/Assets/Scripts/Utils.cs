using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TransformExtensions
{
    public static Transform GetSelfOrChildByName(this Transform transform, string name)
    {
        return EnumerateSelfAndChildren(transform).FirstOrDefault(x => x.name == name);
    }

    public static IEnumerable<Transform> EnumerateSelfAndChildren(this Transform transform)
    {
        yield return transform;

        for (int i = 0; i < transform.childCount; i++)
        {
            foreach (var deeperTransform in EnumerateSelfAndChildren(transform.GetChild(i)))
                yield return deeperTransform;
        }

        yield break;
    }
}