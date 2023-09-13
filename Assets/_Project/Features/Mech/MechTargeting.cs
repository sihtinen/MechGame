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

    private Transform m_transform = null;
    private MechController m_mech = null;
    private Stack<TargetingOption> m_targetingOptionPool = new Stack<TargetingOption>();
    private Stack<TargetingOption> m_usedTargetingOptions = new Stack<TargetingOption>();

    private static List<Transform> m_tempTransformList = new();
    private List<GameObject> m_raycastIgnoredObjects = new List<GameObject>();

    private void Awake()
    {
        m_transform = transform;

        TryGetComponent(out m_mech);

        for (int i = 0; i < 32; i++)
            m_targetingOptionPool.Push(new TargetingOption());
    }

    private void Start()
    {
        // create list of all ignored objects in visibility raycasts

        m_transform.root.GetComponentsInChildren(includeInactive: true, m_tempTransformList);

        for (int i = 0; i < m_tempTransformList.Count; i++)
            m_raycastIgnoredObjects.Add(m_tempTransformList[i].gameObject);

        m_tempTransformList.Clear();
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
        var _mainCameraTransformForward = MainCameraComponent.Instance.TransformComponent.forward;

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

                if (Vector3.Dot(_toTargetNormalized, _mainCameraTransformForward) < 0)
                    continue;
            }

            //if (Vector3.Dot(_toTargetNormalized, m_transform.forward) < 0)
            //    continue;

            if (_possibleTarget.TryGetComponent(out IDamageable _damageable) && _damageable.GetCurrentHealth() <= 0)
                continue;

            if (_toTarget.magnitude > Settings.MaxDistance)
                continue;

            var _raycastResults = GameplayRaycastUtility.Raycast(
                _targetingPivot.position,
                _possibleTarget.TransformComponent.position,
                Physics.DefaultRaycastLayers,
                ignoredObjects: m_raycastIgnoredObjects);

            if (_raycastResults.HitFound)
            {
                if (_possibleTarget.HasColliderWithID(_raycastResults.Hit.collider.GetInstanceID()) == false)
                    continue;
            }

            var _newOption = getTargetingOption();
            _newOption.ContextTargetComponent = _possibleTarget;

            ValidTargets.Add(_newOption);
        }

        float _bestDot = float.MinValue;

        for (int i = 0; i < ValidTargets.Count; i++)
        {
            var _target = ValidTargets[i];

            Vector3 _toTarget = _target.ContextTargetComponent.TransformComponent.position - _targetingPivot.position;
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
        public ContextTarget ContextTargetComponent = null;
        public float DotScore;
        public float DistScore;
        public float TotalScore;

        public Vector3 GetPosition() => ContextTargetComponent.TransformComponent.position;
        public Vector3 GetVelocity() => ContextTargetComponent.GetVelocity();
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