using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class MechTargeting : MonoBehaviour
{
    [Header("Per Component Settings")]
    public TargetingSettings Settings = new();
    public ContextTargetLayers TargetLayers = ContextTargetLayers.None;

    [System.NonSerialized] public List<TargetingOption> ValidTargets = new List<TargetingOption>();
    [System.NonSerialized] public TargetingOption ActiveTarget = null;
    [System.NonSerialized] public List<Vector3> PredictionPositions = new();

    private RaycastHit m_hitInfo;
    private Transform m_transform = null;
    private MechController m_mech = null;
    private Stack<TargetingOption> m_targetingOptionPool = new Stack<TargetingOption>();
    private Stack<TargetingOption> m_usedTargetingOptions = new Stack<TargetingOption>();

    private void Awake()
    {
        m_transform = transform;

        TryGetComponent(out m_mech);

        for (int i = 0; i < 32; i++)
            m_targetingOptionPool.Push(new TargetingOption());
    }

    private void FixedUpdate()
    {
        ActiveTarget = null;
        ValidTargets.Clear();
        PredictionPositions.Clear();

        while (m_usedTargetingOptions.Count > 0)
        {
            var _option = m_usedTargetingOptions.Pop();
            m_targetingOptionPool.Push(_option);
        }

        var _targetingPivot = m_mech.GetTargetingPivotTransform();
        var _possibleTargets = ContextUtils.GetActiveTargets(TargetLayers);

        for (int i = 0; i < _possibleTargets.Count; i++)
        {
            var _possibleTarget = _possibleTargets[i];

            Vector3 _toTarget = _possibleTarget.TransformComponent.position - _targetingPivot.position;
            Vector3 _toTargetNormalized = _toTarget.normalized;

            if (m_mech.IsPlayer)
            {
                var _targetingArea = TargetingArea.Instance;
                if (_targetingArea != null && _targetingArea.IsPointInsideArea(_possibleTarget.TransformComponent.position) == false)
                    continue;

                if (Vector3.Dot(_toTargetNormalized, MainCameraComponent.Instance.TransformComponent.forward) < 0)
                    continue;
            }

            if (_possibleTarget.TryGetComponent(out IDamageable _damageable) && _damageable.GetCurrentHealth() <= 0)
                continue;

            if (Vector3.Dot(_toTargetNormalized, m_transform.forward) < 0)
                continue;

            if (_toTarget.magnitude > Settings.MaxDistance)
                continue;

            if (Physics.Linecast(
                _targetingPivot.position,
                _possibleTarget.TransformComponent.position,
                out m_hitInfo,
                Physics.DefaultRaycastLayers))
            {
                if (m_hitInfo.collider.transform.root.GetInstanceID() != _possibleTarget.TransformComponent.GetInstanceID())
                    continue;
            }

            var _newOption = getTargetingOption();
            _newOption.TransformComponent = _possibleTarget.TransformComponent;

            ValidTargets.Add(_newOption);
        }

        float _bestDot = float.MinValue;

        for (int i = 0; i < ValidTargets.Count; i++)
        {
            var _target = ValidTargets[i];

            Vector3 _toTarget = _target.TransformComponent.position - _targetingPivot.position;
            Vector3 _toWorldTargetPos = m_mech.LookTargetWorldPos - _targetingPivot.position;

            float _toWorldTargetDot = Vector3.Dot(_toTarget.normalized, _toWorldTargetPos.normalized);

            _target.DotScore = _toWorldTargetDot * Settings.DotScoreMultiplier;

            float _distFromOptimalDistance = Mathf.Max(Mathf.InverseLerp(
                Settings.OptimalDistance,
                Settings.MaxDistance,
                _toTarget.magnitude), 0);

            _target.DistScore = (1.0f - _distFromOptimalDistance) * Settings.DistanceScoreMultiplier;

            _target.TotalScore = _target.DotScore + _target.DistScore;

            if (_target.TotalScore > _bestDot)
            {
                _bestDot = _target.TotalScore;
                ActiveTarget = _target;
            }
        }
    }

    private TargetingOption getTargetingOption()
    {
        if (m_targetingOptionPool.Count > 0)
            return m_targetingOptionPool.Pop();

        return new TargetingOption();
    }

    [System.Serializable]
    public class TargetingOption
    {
        public Transform TransformComponent = null;
        public float DotScore;
        public float DistScore;
        public float TotalScore;
    }

    [System.Serializable]
    public class TargetingSettings
    {
        public float MaxDistance = 500f;
        public float OptimalDistance = 100f;

        public float DotScoreMultiplier = 1f;
        public float DistanceScoreMultiplier = 1f;

        public Vector2 AreaSize = new Vector2(250, 180);
    }
}