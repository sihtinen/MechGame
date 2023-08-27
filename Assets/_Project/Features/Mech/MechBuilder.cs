using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class MechBuilder : MonoBehaviour
{
    public MechLoadout LoadoutAsset = null;

    [ContextMenu("Build")]
    public Transform Build()
    {
        Transform _root = new GameObject(LoadoutAsset.LoadoutName).transform;
        _root.SetParent(null);
        _root.SetPositionAndRotation(transform.position, transform.rotation);

        var _legsAsset = LoadoutAsset.Dictionary[EquipmentSlotTypes.Legs];
        if (_legsAsset == null || _legsAsset.VisualsPrefab == null)
        {
            Debug.LogError($"MechBuilder: legs invalid for asset: {LoadoutAsset.name}", LoadoutAsset);
            return _root;
        }

        var _legsObj = instantiate(_legsAsset.VisualsPrefab);
        _legsObj.transform.SetParent(_root.transform, false);

        var _bodyAsset = LoadoutAsset.Dictionary[EquipmentSlotTypes.Body];
        if (_bodyAsset == null || _bodyAsset.VisualsPrefab == null)
        {
            Debug.LogError($"MechBuilder: legs invalid for asset: {LoadoutAsset.name}", LoadoutAsset);
            return _root;
        }

        var _bodyObj = instantiate(_bodyAsset.VisualsPrefab);
        _bodyObj.transform.SetPositionAndRotation(_root.transform.position, _root.transform.rotation);
        _bodyObj.transform.SetParent(_root);

        var _bodySocket_Legs = getChild(_legsObj.transform, "Socket_Body");
        var _bodySocket_Body = getChild(_bodyObj.transform, "Socket_Body");

        createParentConstraint(
            sourceSocket: _bodySocket_Legs, 
            attachedSocket: _bodySocket_Body, 
            eulerOffset: _bodyAsset.VisualPrefabEulerOffset);

        var _armsAsset = LoadoutAsset.Dictionary[EquipmentSlotTypes.Arms];
        if (_armsAsset == null || _armsAsset.VisualsPrefab == null)
        {
            Debug.LogError($"MechBuilder: arms invalid for asset: {LoadoutAsset.name}", LoadoutAsset);
            return _root;
        }

        var _armsObj = instantiate(_armsAsset.VisualsPrefab);
        _armsObj.transform.SetPositionAndRotation(_root.transform.position, _root.transform.rotation);
        _armsObj.transform.SetParent(_root);

        var _armSocket_R_Body = getChild(_bodyObj.transform, "Socket_Arm.R");
        var _armSocket_L_Body = getChild(_bodyObj.transform, "Socket_Arm.L");
        var _armSocket_R_Arms = getChild(_armsObj.transform, "Socket_Arm.R");
        var _armSocket_L_Arms = getChild(_armsObj.transform, "Socket_Arm.L");

        createParentConstraint(
            sourceSocket: _armSocket_R_Body, 
            attachedSocket: _armSocket_R_Arms, 
            eulerOffset: _armsAsset.VisualPrefabEulerOffset);

        createParentConstraint(
            sourceSocket: _armSocket_L_Body, 
            attachedSocket: _armSocket_L_Arms, 
            eulerOffset: _armsAsset.VisualPrefabEulerOffset);

        var _headAsset = LoadoutAsset.Dictionary[EquipmentSlotTypes.Head];
        if (_headAsset == null || _headAsset.VisualsPrefab == null)
        {
            Debug.LogError($"MechBuilder: head invalid for asset: {LoadoutAsset.name}", LoadoutAsset);
            return _root;
        }

        var _headObj = instantiate(_headAsset.VisualsPrefab);
        _headObj.transform.SetPositionAndRotation(_root.transform.position, _root.transform.rotation);
        _headObj.transform.SetParent(_root);

        var _headSocket_Body = getChild(_bodyObj.transform, "Socket_Head");
        var _headSocket_Head = getChild(_headObj.transform, "Socket_Head");

        createParentConstraint(
            sourceSocket: _headSocket_Body, 
            attachedSocket: _headSocket_Head, 
            eulerOffset: _headAsset.VisualPrefabEulerOffset);

        return _root;
    }

    private static void createParentConstraint(Transform sourceSocket, Transform attachedSocket, Vector3 eulerOffset)
    {
        var _constraint = attachedSocket.AddComponent<ParentConstraint>();
        _constraint.AddSource(new ConstraintSource { sourceTransform = sourceSocket, weight = 1 });
        _constraint.SetTranslationOffset(0, Vector3.zero);
        _constraint.SetRotationOffset(0, eulerOffset);
        _constraint.constraintActive = true;
    }

    private GameObject instantiate(GameObject prefab)
    {
#if UNITY_EDITOR
        return UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent: null) as GameObject;
#endif

        return Instantiate(prefab, parent: null);
    }

    private static List<Transform> m_cachedTransforms = new List<Transform>();

    private Transform getChild(Transform root, string name)
    {
        m_cachedTransforms.Clear();
        root.GetComponentsInChildren(includeInactive: true, m_cachedTransforms);

        for (int i = 0; i < m_cachedTransforms.Count; i++)
        {
            if (m_cachedTransforms[i].name == name)
                return m_cachedTransforms[i];
        }

        return null;
    }
}
