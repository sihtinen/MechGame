using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDThrusterEquipmentElement : HUDEquipmentElementBase
{
    [SerializeField] private TMP_Text m_nameText = null;
    [SerializeField] private TMP_Text m_inputText = null;
    [SerializeField] private Slider m_slider = null;

    private MechThrusterRuntime m_thruster = null;

    public override void Initialize(HUDEquipmentSlot slot)
    {
        base.Initialize(slot);

        bool _isLeftAligned = (slot.SlotType == EquipmentSlotTypes.LeftShoulder || slot.SlotType == EquipmentSlotTypes.LeftArm);

        m_nameText.alignment = _isLeftAligned ? TextAlignmentOptions.TopLeft : TextAlignmentOptions.TopRight;
        m_inputText.alignment = _isLeftAligned ? TextAlignmentOptions.TopRight : TextAlignmentOptions.TopLeft;
        m_slider.SetDirection(_isLeftAligned ? Slider.Direction.RightToLeft : Slider.Direction.LeftToRight, true);
    }

    public override void BindToRuntimeComponent(MechEquipmentRuntime runtimeComponent)
    {
        base.BindToRuntimeComponent(runtimeComponent);

        m_thruster = runtimeComponent as MechThrusterRuntime;
        m_nameText.SetText(m_thruster.Settings.DisplayName);
        m_slider.SetValueWithoutNotify(m_thruster.RemainingEnergy);
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (m_thruster == null)
            return;

        m_slider.SetValueWithoutNotify(m_thruster.RemainingEnergy);
    }

    protected override void onInputDeviceTypeChanged()
    {
        m_inputText.SetText(getInputText());
    }
}
