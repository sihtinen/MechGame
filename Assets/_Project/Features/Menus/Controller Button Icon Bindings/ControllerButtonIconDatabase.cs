using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable, CreateAssetMenu(menuName = "MechGame/UI/Controller Button Icon Database")]
public class ControllerButtonIconDatabase : ScriptableObject
{
    public List<BindingsWrapper> Bindings = new List<BindingsWrapper>();

    public Sprite GetIcon(InputDeviceTypes deviceType, InputActionReference inputActionRef)
    {
        var _bindings = getBindings(deviceType);

        if (_bindings == null)
            return null;

        return _bindings.GetIcon(inputActionRef);
    }

    private ControllerButtonIconBindings getBindings(InputDeviceTypes deviceType)
    {
        for (int i = 0; i < Bindings.Count; i++)
        {
            var _wrapper = Bindings[i];

            if (_wrapper.InputDevice == deviceType)
                return _wrapper.IconBindings;
        }

        return null;
    }

    [Serializable]
    public class BindingsWrapper
    {
        public InputDeviceTypes InputDevice;
        public ControllerButtonIconBindings IconBindings = null;
    }
}