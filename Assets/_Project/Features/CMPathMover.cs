using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMPathMover : MonoBehaviour
{
    [SerializeField] private float m_speed = 1f;
    [SerializeField] private CinemachinePathBase m_path = null;

    private float m_currentPathPos;

    private void Start()
    {
        m_currentPathPos = m_path.FindClosestPoint(transform.position, 0, -1, 12);
    }

    private void Update()
    {
        m_currentPathPos += Time.deltaTime * m_speed;
        transform.position = m_path.EvaluatePosition(m_currentPathPos);
    }
}
