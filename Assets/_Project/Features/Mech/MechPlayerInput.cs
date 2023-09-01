using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechPlayerInput : SingletonBehaviour<MechPlayerInput>
{
    [Header("Input Settings")]
    [SerializeField] private InputActionReference m_moveInputRef = null;
    [SerializeField] private InputRefDictionary m_inputRefDictionary = new();

    private MechController m_mechController = null;

    private void OnEnable()
    {
        if (Application.isPlaying == false)
            return;

        TryGetComponent(out m_mechController);

        if (m_moveInputRef != null)
            m_moveInputRef.action.Enable();

        foreach (var _kvp in m_inputRefDictionary)
        {
            if (_kvp.Value != null && _kvp.Value.action.enabled == false)
                _kvp.Value.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (Application.isPlaying == false)
            return;

        if (m_moveInputRef != null)
            m_moveInputRef.action.Disable();

        foreach (var _kvp in m_inputRefDictionary)
        {
            if (_kvp.Value != null && _kvp.Value.action.enabled)
                _kvp.Value.action.Disable();
        }
    }

    private void Update()
    {
        if (m_moveInputRef == null)
            return;

        m_mechController.MoveInput = m_moveInputRef.action.ReadValue<Vector2>();

        updateAimPosition();
    }

    private void updateAimPosition()
    {
        var _mainCam = MainCameraComponent.Instance;
        var _freeAimTarget = FreeAimCrosshair.Instance;

        var _ray = _mainCam.CameraComponent.ScreenPointToRay(_freeAimTarget.transform.position);

        int _hitCount = Physics.RaycastNonAlloc(_ray, PhysicsUtility.CachedRaycastHits);

        if (_hitCount == 0)
        {
            m_mechController.SetLookTargetPos(_ray.origin + 20000 * _ray.direction);
            return;
        }

        int _selfInstanceID = transform.root.GetInstanceID();

        for (int i = 0; i < _hitCount; i++)
        {
            var _hit = PhysicsUtility.CachedRaycastHits[i];

            if (_hit.transform.root.GetInstanceID() == _selfInstanceID)
                continue;

            m_mechController.SetLookTargetPos(_hit.point);
            return;
        }
    }

    public InputActionReference GetInputActionRef(EquipmentSlotTypes slotType) => m_inputRefDictionary[slotType];

    [ContextMenu("Reset Dictionary")]
    public void ResetDictionary()
    {
        m_inputRefDictionary.Clear();

        foreach (int i in Enum.GetValues(typeof(EquipmentSlotTypes)))
        {
            if (i < 0)
                continue;

            m_inputRefDictionary.Add((EquipmentSlotTypes)i, null);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
#endif
    }
}

[System.Serializable]
public class InputRefDictionary : SerializableDictionary<EquipmentSlotTypes, InputActionReference> { }