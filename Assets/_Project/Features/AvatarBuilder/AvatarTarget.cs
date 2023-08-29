using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AvatarTarget : MonoBehaviour
{
    public Transform Legs;
    public Transform Body;
    public Transform Arms;
    public Transform Head;

    private static List<Transform> m_bones = new();

    public Transform GetBone(string boneName)
    {
        return transform.FindChildRecursive(boneName);
    }

    public void BindToRig(Transform rigTransform, Dictionary<string, Transform> armatureCache)
    {
        GetComponentsInChildren(includeInactive: true, m_bones);

        for (int i = 0; i < m_bones.Count; i++)
        {
            var _bone = m_bones[i];

            if (armatureCache.TryGetValue(_bone.name, out var _matchingBone))
                createParentConstraint(_bone, _matchingBone);
        }

        m_bones.Clear();
    }

    private static void createParentConstraint(Transform source, Transform target)
    {
        ParentConstraint _constraint;

        if (source.TryGetComponent(out _constraint))
            _constraint.RemoveSource(0);
        else
            _constraint = source.gameObject.AddComponent<ParentConstraint>();

        _constraint.AddSource(new ConstraintSource { sourceTransform = target, weight = 1 });
        _constraint.SetTranslationOffset(0, Vector3.zero);
        _constraint.constraintActive = true;
    }
}
