using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class WeaponVisualsManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0)] private float m_impulseForce = 1.0f;
    [SerializeField] private float m_minTimeBetweenEffects = 0.1f;

    [Header("Object References")]
    [SerializeField] private Transform m_weaponBarrel = null;
    [SerializeField] private CinemachineImpulseSource m_CMImpulseSource = null;
    [SerializeField] private VisualEffect m_muzzleFlashEffect = null;

    [Header("Events")]
    public UnityEvent OnPlayFireEffects = new UnityEvent();

    private float m_previousFireEffectsTime = 0;

    public void TriggerFireEffects()
    {
        if (Time.time - m_previousFireEffectsTime < m_minTimeBetweenEffects)
            return;

        m_previousFireEffectsTime = Time.time;

        m_CMImpulseSource?.GenerateImpulseWithForce(m_impulseForce);

        if (m_muzzleFlashEffect != null)
        {
            var _muzzleFlashLocalEuler = m_muzzleFlashEffect.transform.localEulerAngles;
            _muzzleFlashLocalEuler.z = Random.Range(0, 360);
            m_muzzleFlashEffect.transform.localEulerAngles = _muzzleFlashLocalEuler;

            m_muzzleFlashEffect.Play();
        }

        OnPlayFireEffects?.Invoke();
    }

    public Vector3 GetWeaponBarrelPosition() => m_weaponBarrel.position;
    public Vector3 GetWeaponBarrelForward() => -m_weaponBarrel.forward;
}