using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class PoolableBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    [Header("Poolable Behaviour Base Settings")]
    [SerializeField] private bool m_deactivateGOWhenReturnedToPool = true;

    private Transform m_transform = null;
    private Transform m_originalParent = null;

    public void InitializePoolable()
    {
        m_transform = transform;
        m_originalParent = m_transform.parent;

        resetAndClearBindings();

        if (m_deactivateGOWhenReturnedToPool)
            gameObject.SetActiveOptimized(false);
    }

    public void ResetAndReturnToPool()
    {
        resetAndClearBindings();

        if (m_deactivateGOWhenReturnedToPool)
            gameObject.SetActiveOptimized(false);

        if (m_transform.parent != m_originalParent)
            m_transform.SetParent(m_originalParent);

        PoolableBehaviourPool<T>.Release(this as T);
    }

    protected abstract void resetAndClearBindings();
}
