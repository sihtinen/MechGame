using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugUICanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text m_activeProjectilesValueText = null;

    private void LateUpdate()
    {
        var _projectileManager = ProjectileManager.Instance;
        if (_projectileManager != null)
            m_activeProjectilesValueText.SetText(_projectileManager.ActiveProjectilesCount.ToStringMinimalAlloc());
    }
}
