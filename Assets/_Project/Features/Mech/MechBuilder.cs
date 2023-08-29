using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MechBuilder : MonoBehaviour
{
    public MechLoadout LoadoutAsset = null;

    [ContextMenu("Build")]
    public Transform Build()
    {
        var _root = new GameObject(LoadoutAsset.LoadoutName).transform;
        _root.SetParent(null);
        _root.SetPositionAndRotation(transform.position, transform.rotation);
        _root.name = $"MechBuild: {LoadoutAsset.name}";

        var _avatarTarget = _root.gameObject.AddComponent<AvatarTarget>();

        var _legsAsset = LoadoutAsset.Dictionary[EquipmentSlotTypes.Legs];
        if (_legsAsset == null || _legsAsset.VisualsPrefab == null)
        {
            Debug.LogError($"MechBuilder: legs invalid for asset: {LoadoutAsset.name}", LoadoutAsset);
            return _root;
        }

        var _legsObj = instantiate(_legsAsset.VisualsPrefab);
        _legsObj.transform.SetParent(_root.transform, false);
        _avatarTarget.Legs = _legsObj.transform;

        var _bodyAsset = LoadoutAsset.Dictionary[EquipmentSlotTypes.Body];
        if (_bodyAsset == null || _bodyAsset.VisualsPrefab == null)
        {
            Debug.LogError($"MechBuilder: body invalid for asset: {LoadoutAsset.name}", LoadoutAsset);
            return _root;
        }

        var _bodySocket_Legs = _legsObj.transform.FindChildRecursive("Bone_Hip");

        var _bodyObj = instantiate(_bodyAsset.VisualsPrefab);

        _bodyObj.transform.SetPositionAndRotation(
            _bodySocket_Legs.position, 
            Quaternion.Euler(_bodyAsset.VisualPrefabEulerOffset) * _bodySocket_Legs.rotation);

        _bodyObj.transform.SetParent(_root);
        _avatarTarget.Body = _bodyObj.transform;

        var _armsAsset = LoadoutAsset.Dictionary[EquipmentSlotTypes.Arms];
        if (_armsAsset == null || _armsAsset.VisualsPrefab == null)
        {
            Debug.LogError($"MechBuilder: arms invalid for asset: {LoadoutAsset.name}", LoadoutAsset);
            return _root;
        }

        var _armsObj = instantiate(_armsAsset.VisualsPrefab);
        _armsObj.transform.SetPositionAndRotation(_root.transform.position, _root.transform.rotation);
        _armsObj.transform.SetParent(_root);
        _avatarTarget.Arms = _armsObj.transform;

        var _armSocket_R_Body = _bodyObj.transform.FindChildRecursive("Bone_Shoulder.R");
        var _armSocket_L_Body = _bodyObj.transform.FindChildRecursive("Bone_Shoulder.L");
        var _armSocket_R_Arms = _armsObj.transform.FindChildRecursive("Bone_Shoulder.R");
        var _armSocket_L_Arms = _armsObj.transform.FindChildRecursive("Bone_Shoulder.L");

        _armSocket_R_Arms.SetPositionAndRotation(_armSocket_R_Body.position, Quaternion.identity);
        _armSocket_L_Arms.SetPositionAndRotation(_armSocket_L_Body.position, Quaternion.identity);

        _armSocket_R_Arms.localPosition += _armsAsset.VisualPrefabPositionOffset;
        _armSocket_L_Arms.localPosition += Vector3.Scale(_armsAsset.VisualPrefabPositionOffset, new Vector3(-1, 1, 1));

        _armSocket_R_Arms.localEulerAngles = _armsAsset.VisualPrefabEulerOffset;
        _armSocket_L_Arms.localEulerAngles = _armsAsset.VisualPrefabEulerOffset;

        var _headAsset = LoadoutAsset.Dictionary[EquipmentSlotTypes.Head];
        if (_headAsset == null || _headAsset.VisualsPrefab == null)
        {
            Debug.LogError($"MechBuilder: head invalid for asset: {LoadoutAsset.name}", LoadoutAsset);
            return _root;
        }

        var _headObj = instantiate(_headAsset.VisualsPrefab);
        _headObj.transform.SetPositionAndRotation(_root.transform.position, _root.transform.rotation);
        _headObj.transform.SetParent(_root);
        _avatarTarget.Head = _headObj.transform;

        var _headSocket_Body = _bodyObj.transform.FindChildRecursive("Bone_Neck");
        var _headSocket_Head = _headObj.transform.FindChildRecursive("Bone_Neck");

        _headSocket_Head.SetPositionAndRotation(_headSocket_Body.position, Quaternion.identity);
        _headSocket_Head.localEulerAngles = _headAsset.VisualPrefabEulerOffset;

        return _root;
    }

    private GameObject instantiate(GameObject prefab)
    {
#if UNITY_EDITOR
        return UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent: null) as GameObject;
#endif

        return Instantiate(prefab, parent: null);
    }
}
