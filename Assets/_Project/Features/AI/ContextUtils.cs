using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ContextUtils
{
    private static List<ContextTarget> m_cachedResultsList = new List<ContextTarget>();

    public static List<ContextTarget> GetActiveTargets(ContextTargetLayers layers)
    {
        m_cachedResultsList.Clear();
        var _allContextTargets = ObjectCollection<ContextTarget>.AllObjects;

        int _queryBitMask = (int)layers;

        for (int i = 0; i < _allContextTargets.Count; i++)
        {
            var _target = _allContextTargets[i];
            int _targetBitMask = (int)_target.TargetLayers;

            for (int ii = 0; ii < 32; ii++)
            {
                if (bitMaskIncludes(_queryBitMask, ii) && bitMaskIncludes(_targetBitMask, ii))
                {
                    m_cachedResultsList.Add(_target);
                    break;
                }
            }
        }

        return m_cachedResultsList;
    }

    private static bool bitMaskIncludes(int bitMask, int layer)
    {
        return (bitMask & 1 << layer) > 0;
    }
}
