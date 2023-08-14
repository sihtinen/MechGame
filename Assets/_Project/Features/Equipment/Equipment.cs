using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public abstract class Equipment : ScriptableObject
{
    [Header("Equipment Generic Settings")]
    public GUIDWrapper GUID = new GUIDWrapper();
    public string DisplayName = "New Equipment";

    public abstract void InitializeGameplay(
        MechController mech,
        EquipmentSlot slot,
        bool isPlayer,
        InputActionReference inputActionRef);

    protected T initializeRuntimeComponent<T>(MechController mech, EquipmentSlot slot) where T : MechEquipmentRuntime
    {
        var _newObj = new GameObject($"{slot.SlotType}_{DisplayName}");
        _newObj.transform.SetParent(slot.TransformComponent);
        _newObj.transform.localPosition = Vector3.zero;
        _newObj.transform.localRotation = Quaternion.identity;

        var _runtimeComponent = _newObj.AddComponent<T>();
        _runtimeComponent.InitializeGameplay(mech, this);

        return _runtimeComponent;
    }
}
