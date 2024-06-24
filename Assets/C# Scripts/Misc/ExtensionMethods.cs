using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ExtensionMethods
{
    public static void SetParent(this Transform trans, Transform parent, bool keepLocalPos, bool keepLocalRot)
    {
        if (parent == null)
        {
            Debug.LogWarning("You are trying to set a transform to a parent that doesnt exist, this is not allowed");
            return;
        }

        trans.parent = parent;
        if (!keepLocalPos)
        {
            trans.localPosition = Vector3.zero;
        }
        if (!keepLocalRot)
        {
            trans.localRotation = Quaternion.identity;
        }
    }
}
public static class VectorLogic
{
    public static Vector3 InstantMoveTowards(Vector3 from, Vector3 to, float maxDist)
    {
        Vector3 newVec = from;

        newVec.x += (from.x > to.x) ? -maxDist : maxDist;
        newVec.y += (from.y > to.y) ? -maxDist : maxDist;
        newVec.z += (from.z > to.z) ? -maxDist : maxDist;

        newVec.x = Mathf.Clamp(newVec.x, Mathf.Min(from.x, to.x), Mathf.Max(from.x, to.x));
        newVec.y = Mathf.Clamp(newVec.y, Mathf.Min(from.y, to.y), Mathf.Max(from.y, to.y));
        newVec.z = Mathf.Clamp(newVec.z, Mathf.Min(from.z, to.z), Mathf.Max(from.z, to.z));

        return newVec;
    }
    public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max)
    {
        value.x = Mathf.Clamp(value.x, min.x, max.x);
        value.y = Mathf.Clamp(value.y, min.y, max.y);
        value.z = Mathf.Clamp(value.z, min.z, max.z);

        return value;
    }
}