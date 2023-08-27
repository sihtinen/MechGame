using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshColliderConvexBatch : MonoBehaviour
{
    private static List<MeshCollider> _colls = new List<MeshCollider>();

    private void Awake()
    {
        Execute();
    }

    [ContextMenu("Convert all child mesh colliders to convex")]
    public void Execute()
    {
        _colls.Clear();
        GetComponentsInChildren(includeInactive: true, _colls);

        for (int i = 0; i < _colls.Count; i++)
        {
            _colls[i].convex = true;

#if UNITY_EDITOR
            if (Application.isPlaying == false)
                UnityEditor.EditorUtility.SetDirty(_colls[i]);
#endif
        }
    }
}
