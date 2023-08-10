using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "MechGame/PID/New 1D PID Controller")]
public class PID : ScriptableObject
{
    public struct PIDState
    {
        public bool DerivativeInitialized;
        public float PrevError;
        public float PrevValue;
        public float IntegrationStored;
        public float Output;

        public float P, I, D;

        public void ResetState()
        {
            DerivativeInitialized = false;
            PrevError = 0;
            PrevValue = 0;
            IntegrationStored = 0;
            Output = 0;
        }
    }

    public enum DerivativeMeasurement
    {
        Velocity = 0,
        ErrorChangeRate = 1,
    }

    public bool IsEnabled = true;
    public float ProportionalGain;
    public float IntegralGain;
    public float IntegralLimit;
    public float DerivativeGain;
    public float OutputMin;
    public float OutputMax;
    public DerivativeMeasurement DerivativeMeasurementType = DerivativeMeasurement.Velocity;

    public PIDState UpdateTick(float deltaTime, PIDState state, float currentValue, float targetValue)
    {
        float _error = targetValue - currentValue;

        float P = ProportionalGain * _error;

        state.IntegrationStored = Mathf.Clamp(
            state.IntegrationStored + _error * deltaTime,
            -IntegralLimit,
            IntegralLimit);

        float I = IntegralGain * state.IntegrationStored;

        float _errorChangeRate = (_error - state.PrevError) / deltaTime;
        state.PrevError = _error;

        float _valueChangeRate = (currentValue - state.PrevValue) / deltaTime;
        state.PrevValue = currentValue;

        float _derivativeMeasure = 0;

        if (state.DerivativeInitialized)
        {
            switch (DerivativeMeasurementType)
            {
                default:
                case DerivativeMeasurement.Velocity:
                    _derivativeMeasure = -_valueChangeRate;
                    break;

                case DerivativeMeasurement.ErrorChangeRate:
                    _derivativeMeasure = _errorChangeRate;
                    break;
            }
        }
        else
        {
            state.DerivativeInitialized = true;
        }

        float D = DerivativeGain * _derivativeMeasure;

        float _result = P + I + D;

        state.Output = Mathf.Clamp(_result, OutputMin, OutputMax);

        if (IsEnabled == false)
            state.Output = 0;
        
        state.P = P;
        state.I = I;
        state.D = D;

        return state;
    }

    float AngleDifference(float a, float b)
    {
        return (a - b + 540) % 360 - 180;   //calculate modular difference, and remap to [-180, 180]
    }


    public PIDState UpdateAngleTick(float deltaTime, PIDState state, float currentAngle, float targetAngle)
    {
        float error = AngleDifference(targetAngle, currentAngle);

        //calculate P term
        float P = ProportionalGain * error;

        //calculate I term
        state.IntegrationStored = Mathf.Clamp(state.IntegrationStored + (error * deltaTime), -IntegralLimit, IntegralLimit);
        float I = IntegralGain * state.IntegrationStored;

        //calculate both D terms
        float errorRateOfChange = AngleDifference(error, state.PrevError) / deltaTime;
        state.PrevError = error;

        float valueRateOfChange = AngleDifference(currentAngle, state.PrevValue) / deltaTime;
        state.PrevValue = currentAngle;

        //choose D term to use
        float deriveMeasure = 0;

        if (state.DerivativeInitialized)
        {
            if (DerivativeMeasurementType == DerivativeMeasurement.Velocity)
            {
                deriveMeasure = -valueRateOfChange;
            }
            else
            {
                deriveMeasure = errorRateOfChange;
            }
        }
        else
        {
            state.DerivativeInitialized = true;
        }

        float D = DerivativeGain * deriveMeasure;

        float result = P + I + D;

        state.Output = Mathf.Clamp(result, OutputMin, OutputMax);

        return state;
    }
}
