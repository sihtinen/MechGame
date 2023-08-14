using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable, CreateAssetMenu(menuName = "MechGame/UI/Controller Button Icon Bindings")]
public class ControllerButtonIconBindings : ScriptableObject
{
    [SerializeField] private Sprite m_dpadUp = null;
    [SerializeField] private Sprite m_dpadDown = null;
    [SerializeField] private Sprite m_dpadLeft = null;
    [SerializeField] private Sprite m_dpadRight = null;
    [Space]
    [SerializeField] private Sprite m_faceUp = null;
    [SerializeField] private Sprite m_faceDown = null;
    [SerializeField] private Sprite m_faceLeft = null;
    [SerializeField] private Sprite m_faceRight = null;
    [Space]
    [SerializeField] private Sprite m_shoulderLeft = null;
    [SerializeField] private Sprite m_shoulderRight = null;
    [SerializeField] private Sprite m_triggerLeft = null;
    [SerializeField] private Sprite m_triggerRight = null;
    [Space]
    [SerializeField] private Sprite m_menuLeft = null;
    [SerializeField] private Sprite m_menuRight = null;

    public Sprite GetIcon(InputActionReference inputActionRef)
    {
        var _controlPaths = inputActionRef.action.controls;

        for (int i = 0; i < _controlPaths.Count; i++)
        {
            var _controlPath = _controlPaths[i];

            switch (_controlPath.name)
            {
                case "buttonNorth": return m_faceUp;
                case "buttonSouth": return m_faceDown;
                case "buttonWest": return m_faceLeft;
                case "buttonEast": return m_faceRight;

                default: break;
            }
        }

        return null;
    }
}