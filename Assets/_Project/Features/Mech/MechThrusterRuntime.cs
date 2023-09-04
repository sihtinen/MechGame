using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechThrusterRuntime : MechEquipmentRuntime
{
    [Header("Runtime Parameters")]
    [NonEditable] public bool IsNormalBoostActive = false;
    [NonEditable] public float RemainingEnergy = 100;

    [NonSerialized] public ThrusterEquipment Settings = null;

    private MechController m_mechController = null;
    private Coroutine m_dashBoostCoroutine = null;

    public override void InitializeGameplay(MechController mech, Equipment settings, EquipmentSlotTypes slotType)
    {
        m_mechController = mech;
        Settings = settings as ThrusterEquipment;
    }

    protected override void onInputStarted(InputAction.CallbackContext context)
    {
        if (m_dashBoostCoroutine != null)
            return;

        if (RemainingEnergy < Settings.DashEnergyDrain)
            return;

        IsNormalBoostActive = true;
        m_dashBoostCoroutine = StartCoroutine(coroutine_dashBoost());
    }

    protected override void onInputCanceled(InputAction.CallbackContext context)
    {
        IsNormalBoostActive = false;
    }

    private void Update()
    {
        if (m_mechController == null)
            return;

        updateNormalBoost();

        m_mechController.IsBoosting = IsNormalBoostActive || m_dashBoostCoroutine != null;
    }

    private void updateNormalBoost()
    {
        if (m_dashBoostCoroutine != null)
            return;

        float _horizontalThrust = 0;
        float _verticalThrust = 0;

        bool _boostButtonHeld = m_inputActionRef != null && m_inputActionRef.action.IsPressed();

        if (IsNormalBoostActive && _boostButtonHeld)
        {
            if (RemainingEnergy > 0f)
            {
                float _inputAmount = m_mechController.MoveInput.magnitude;
                var _boostParams = Settings.GetBoostParams(_inputAmount);

                _horizontalThrust = _boostParams.HorizontalForce;
                _verticalThrust = _boostParams.VerticalForce;

                float _drainRate = Settings.EnergyDrainRate_Normal;
                RemainingEnergy -= Time.deltaTime * _drainRate;
            }
            else
                IsNormalBoostActive = false;
        }
        else
        {
            IsNormalBoostActive = false;
            RemainingEnergy += Time.deltaTime * Settings.RechargeRate;
        }

        RemainingEnergy = Mathf.Clamp(RemainingEnergy, 0f, 100f);

        m_mechController.BoostForceHorizontal = _horizontalThrust;
        m_mechController.BoostForceVertical = _verticalThrust;
    }

    private IEnumerator coroutine_dashBoost()
    {
        m_mechController.RegisterPreventMovementSource(GetInstanceID());
        m_mechController.BoostForceHorizontal = 0;
        m_mechController.BoostForceVertical = 0;

        float _timer = 0f;

        while (_timer < Settings.DashPrepareDuration)
        {
            yield return null;
            _timer += Time.deltaTime;
        }

        m_mechController.UnregisterPreventMovementSource(GetInstanceID());

        yield return new WaitForFixedUpdate();

        RemainingEnergy -= Settings.DashEnergyDrain;

        var _vel = m_mechController.RigidBody.velocity;
        _vel.y *= 1.0f - Settings.DashVerticalVelocityCancelAmount;
        m_mechController.RigidBody.velocity = _vel;

        var _inputDir = m_mechController.GetInputDirection();
        float _inputAmount = m_mechController.MoveInput.magnitude;
        var _dashBoostParams = Settings.GetDashBoostParams(_inputAmount);

        _timer = 0f;
        while (_timer < Settings.DashBoostDuration)
        {
            float _curveVal = Settings.DashBoostForceCurve.Evaluate(_timer / Settings.DashBoostDuration);

            Vector3 _dashBoostForce = _curveVal * _dashBoostParams.HorizontalForce * _inputDir;
            _dashBoostForce.y += _curveVal * _dashBoostParams.VerticalForce;

            m_mechController.DashBoostForce = _dashBoostForce;

            yield return null;

            _timer += Time.deltaTime;
        }

        m_mechController.DashBoostForce = Vector3.zero;

        m_dashBoostCoroutine = null;
    }
}
