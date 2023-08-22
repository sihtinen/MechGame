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
    [Space]
    [SerializeField] private Sprite m_stickLeft = null;
    [SerializeField] private Sprite m_stickRight = null;

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

                case "leftShoulder": return m_shoulderLeft;
                case "rightShoulder": return m_shoulderRight;
                case "leftTrigger": return m_triggerLeft;
                case "rightTrigger": return m_triggerRight;

                case "leftStickPress": return m_stickLeft;
                case "rightStickPress": return m_stickRight;

                default:
                    Debug.LogWarning("ControllerButtonIconBindings: icon logic missing for control path - " + _controlPath.name);
                    break;
            }
        }

        return null;
    }
}