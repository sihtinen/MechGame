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
        EquipmentSlotTypes slotType, 
        bool isPlayer,
        InputActionReference inputActionRef);

    protected T initializeRuntimeComponent<T>(MechController mech, EquipmentSlotTypes slot) where T : MechEquipmentRuntime
    {
        var _newObj = new GameObject($"{slot}_{DisplayName}");
        _newObj.transform.SetParent(mech.TransformComponent);
        _newObj.transform.localPosition = new Vector3(0f, 10f, 0f);
        _newObj.transform.localRotation = Quaternion.identity;

        var _runtimeComponent = _newObj.AddComponent<T>();
        _runtimeComponent.InitializeGameplay(mech, this);

        return _runtimeComponent;
    }
}
