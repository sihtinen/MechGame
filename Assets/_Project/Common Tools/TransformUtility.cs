using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformUtility
{
    public static void SetRotationAroundPivot(this Transform transform, Vector3 pivotPoint, Quaternion rotation)
    {
        transform.SetPositionAndRotation(
            rotation * (transform.position - pivotPoint) + pivotPoint, 
            rotation);
    }

    public struct AxisAngles
    {
        public float Pitch;
        public float Yaw;
        public float Roll;
    }

    public static AxisAngles GetAxisAngles(this Transform transform)
    {
        var _rot = transform.rotation;
        var _rad2Deg = Mathf.Rad2Deg;

        return new AxisAngles
        {
            Pitch = _rad2Deg * Mathf.Atan2(2 * _rot.x * _rot.w - 2 * _rot.y * _rot.z, 1 - 2 * _rot.x * _rot.x - 2 * _rot.z * _rot.z),
            Roll = _rad2Deg * Mathf.Asin(2 * _rot.x * _rot.y + 2 * _rot.z * _rot.w),
            Yaw = _rad2Deg * Mathf.Atan2(2 * _rot.y * _rot.w - 2 * _rot.x * _rot.z, 1 - 2 * _rot.y * _rot.y - 2 * _rot.z * _rot.z),
        };
    }

    public static Transform FindChildRecursive(this Transform transform, string childName)
    {
        m_cachedTransforms.Clear();
        transform.GetComponentsInChildren(includeInactive: true, m_cachedTransforms);

        for (int i = 0; i < m_cachedTransforms.Count; i++)
        {
            if (m_cachedTransforms[i].name == childName)
                return m_cachedTransforms[i];
        }

        return null;
    }

    private static List<Transform> m_cachedTransforms = new List<Transform>();
}
