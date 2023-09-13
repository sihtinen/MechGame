using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class GameplayRaycastUtility
{
    public struct Results
    {
        public bool HitFound;
        public RaycastHit Hit;
    }

    public static Results Raycast(
        Vector3 from, 
        Vector3 to, 
        LayerMask hitLayers, 
        List<GameObject> ignoredObjects = null,
        bool collideWithTriggers = false)
    {
        Vector3 _diff = to - from;

        Results _results = new Results()
        {
            HitFound = false,
            Hit = default,
        };

        var _triggerInteractionMode = collideWithTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

        int _hitCount = Physics.RaycastNonAlloc(
            from,
            _diff.normalized,
            PhysicsUtility.CachedRaycastHits,
            _diff.magnitude + 0.01f,
            hitLayers,
            _triggerInteractionMode);

        if (_hitCount == 0) 
            return _results;

        var _resultsWrapper = PhysicsUtility.CachedRaycastHits.GetClosestHit(_hitCount, ignoredObjects);
        
        if (_resultsWrapper.HitFound == false) 
            return _results;

        _results.HitFound = true;
        _results.Hit = _resultsWrapper.RaycastHit;

        return _results;
    }
}
