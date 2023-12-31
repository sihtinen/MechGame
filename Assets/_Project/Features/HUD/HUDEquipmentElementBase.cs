using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class HUDEquipmentElementBase : MonoBehaviour
{
    protected InputDeviceTypes m_inputDeviceType;
    protected RectTransform m_rectTransform = null;
    protected HUDEquipmentSlot m_currentSlot = null;
    private MechEquipmentRuntime m_equipmentRuntime = null;

    private void Awake()
    {
        m_rectTransform = transform as RectTransform;
    }

    public virtual void Initialize(HUDEquipmentSlot slot)
    {
        m_currentSlot = slot;

        m_rectTransform.SetParent(m_currentSlot.RectTransformComponent);
        m_rectTransform.anchorMin = Vector2.zero;
        m_rectTransform.anchorMax = Vector2.one;
        m_rectTransform.anchoredPosition = Vector2.zero;
        m_rectTransform.localRotation = Quaternion.identity;
        m_rectTransform.localScale = Vector2.one;

        m_rectTransform.SetLeft(0f);
        m_rectTransform.SetTop(0f);
        m_rectTransform.SetRight(0f);
        m_rectTransform.SetBottom(0f);
    }

    protected virtual void LateUpdate()
    {
        if (m_equipmentRuntime == null)
            return;

        var _inputDeviceType = m_equipmentRuntime.GetActiveInputDeviceType();
        if (_inputDeviceType != InputDeviceTypes.Inactive && _inputDeviceType != m_inputDeviceType)
        {
            m_inputDeviceType = _inputDeviceType;
            onInputDeviceTypeChanged();
        }
    }

    public virtual void BindToRuntimeComponent(MechEquipmentRuntime runtimeComponent) 
    {
        m_equipmentRuntime = runtimeComponent;
        m_inputDeviceType = m_equipmentRuntime.GetActiveInputDeviceType();
        onInputDeviceTypeChanged();
    }

    protected string getInputText()
    {
        switch (m_currentSlot.SlotType)
        {
            default:
            case EquipmentSlotTypes.LeftShoulder:
                switch (m_inputDeviceType)
                {
                    case InputDeviceTypes.KeyboardAndMouse:
                        return "L-Shift";
                    case InputDeviceTypes.Xbox:
                        return "LB";
                    default:
                    case InputDeviceTypes.PlayStation:
                        return "L1";
                }
            case EquipmentSlotTypes.LeftArm:
                switch (m_inputDeviceType)
                {
                    case InputDeviceTypes.KeyboardAndMouse:
                        return "Mouse0";
                    case InputDeviceTypes.Xbox:
                        return "LT";
                    default:
                    case InputDeviceTypes.PlayStation:
                        return "L2";
                }
            case EquipmentSlotTypes.RightShoulder:
                switch (m_inputDeviceType)
                {
                    case InputDeviceTypes.KeyboardAndMouse:
                        return "Space";
                    case InputDeviceTypes.Xbox:
                        return "RB";
                    default:
                    case InputDeviceTypes.PlayStation:
                        return "R1";
                }
            case EquipmentSlotTypes.RightArm:
                switch (m_inputDeviceType)
                {
                    case InputDeviceTypes.KeyboardAndMouse:
                        return "Mouse1";
                    case InputDeviceTypes.Xbox:
                        return "RT";
                    default:
                    case InputDeviceTypes.PlayStation:
                        return "R2";
                }
        }
    }

    protected virtual void onInputDeviceTypeChanged() { }
}