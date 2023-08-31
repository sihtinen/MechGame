using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechThrusterRuntime : MechEquipmentRuntime
{
    [Header("Runtime Parameters")]
    [NonEditable] public float RemainingEnergy = 100;

    private float m_boostInputWindow = 0.3f;
    private float m_previousCancelTimeStamp = 0;
    private ThrustType m_currentThrustType = ThrustType.Normal;
    private MechController m_mechController = null;
    private ThrusterEquipment m_settings = null;
    public ThrusterEquipment Settings => m_settings;

    public override void InitializeGameplay(MechController mech, Equipment settings)
    {
        m_mechController = mech;
        m_settings = settings as ThrusterEquipment;
    }

    protected override void onInputStarted(InputAction.CallbackContext context)
    {
        m_currentThrustType = Time.time - m_previousCancelTimeStamp <= m_boostInputWindow ? ThrustType.Boost : ThrustType.Normal;
    }

    protected override void onInputCanceled(InputAction.CallbackContext context)
    {
        m_previousCancelTimeStamp = Time.time;
    }

    private void Update()
    {
        if (m_mechController == null)
            return;

        float _horizontalThrust = 0;
        float _verticalThrust = 0;

        m_mechController.IsBoosting = m_inputActionRef != null && m_inputActionRef.action.IsPressed();

        if (m_mechController.IsBoosting)
        {
            var _thrustType = m_mechController.IsGrounded == false || m_currentThrustType == ThrustType.Boost ? 
                ThrustType.Boost : 
                ThrustType.Normal;

            if (RemainingEnergy > 0f)
            {
                _horizontalThrust = _thrustType == ThrustType.Boost ? m_settings.HorizontalForce_Boost : m_settings.HorizontalForce_Normal;
                _verticalThrust = _thrustType == ThrustType.Boost ? m_settings.VerticalForce_Boost : m_settings.VerticalForce_Normal;

                float _drainRate = _thrustType == ThrustType.Boost ? m_settings.EnergyDrainRate_Boost : m_settings.EnergyDrainRate_Normal;
                RemainingEnergy -= Time.deltaTime * _drainRate;
            }
        }
        else
        {
            RemainingEnergy += Time.deltaTime * m_settings.RechargeRate;
        }

        RemainingEnergy = Mathf.Clamp(RemainingEnergy, 0f, 100f);

        m_mechController.ThrustVelocityHorizontal = _horizontalThrust;
        m_mechController.ThrustVelocityVertical = _verticalThrust;
    }

    public enum ThrustType
    {
        Normal = 0,
        Boost = 1,
    }
}
