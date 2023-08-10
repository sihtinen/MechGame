using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ContextProcessor : ScriptableObject
{
    [Header("Base Settings")]
    [SerializeField] private ResultOperationTypes m_resultOperation;
    [Space]
    [SerializeField] private bool m_clampEffect = false;
    [SerializeField] private float m_clampEffectMin = -1;
    [SerializeField] private float m_clampEffectMax = 1;
    [Space]
    [SerializeField] private bool m_clampResult = false;
    [SerializeField] private float m_clampResultMin = -1;
    [SerializeField] private float m_clampResultMax = 1;

    public abstract void Process(ContextEntity entity);

    protected void calculateValue(ref float value, float effect)
    {
        if (m_clampEffect)
            effect = Mathf.Clamp(effect, m_clampEffectMin, m_clampEffectMax);

        switch (m_resultOperation)
        {
            default:
            case ResultOperationTypes.Add:
                value += effect;
                break;
            case ResultOperationTypes.Subtract:
                value -= effect;
                break;
            case ResultOperationTypes.Multiply:
                value *= effect;
                break;
            case ResultOperationTypes.Divide:
                value /= effect;
                break;
        }

        if (m_clampResult)
            Mathf.Clamp(value, m_clampResultMin, m_clampResultMax);
    }

    private enum ResultOperationTypes
    {
        Add,
        Subtract,
        Multiply,
        Divide,
    }
}
