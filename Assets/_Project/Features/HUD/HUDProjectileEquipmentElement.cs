using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDProjectileEquipmentElement : HUDEquipmentElementBase
{
    [SerializeField] private TMP_Text m_nameText = null;
    [SerializeField] private TMP_Text m_inputText = null;
    [SerializeField] private TMP_Text m_remainingUsageText = null;
    [SerializeField] private Slider m_slider = null;

    private MechProjectileRuntime m_projectile = null;

    private int m_prevUsesCount;

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

        m_projectile = runtimeComponent as MechProjectileRuntime;
        m_nameText.SetText(m_projectile.Settings.DisplayName);

        m_prevUsesCount = m_projectile.RemainingUses;
        m_slider.maxValue = m_prevUsesCount;
        m_slider.SetValueWithoutNotify(m_prevUsesCount);
        m_remainingUsageText.SetText(m_prevUsesCount.ToStringMinimalAlloc());
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (m_projectile == null)
            return;

        int _remainingUses = m_projectile.RemainingUses;
        if (_remainingUses != m_prevUsesCount)
        {
            m_prevUsesCount = _remainingUses;
            m_slider.SetValueWithoutNotify(m_prevUsesCount);
            m_remainingUsageText.SetText(m_prevUsesCount.ToStringMinimalAlloc());
        }
    }

    protected override void onInputDeviceTypeChanged()
    {
        m_inputText.SetText(getInputText());
    }
}
