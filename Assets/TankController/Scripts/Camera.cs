using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private Transform m_SpringArmTarget;
	[SerializeField] private Transform m_CameraMount;
	[SerializeField] private Camera m_Camera;

	private float m_CameraDist = 5f;

	[SerializeField] private float m_YawSensitivity;
	[SerializeField] private float m_PitchSensitivity;
	[SerializeField] private float m_ZoomSensitivity;

	[SerializeField] private float m_MaxDist;
	[SerializeField] private float m_MinDist;

	[SerializeField] private float m_CameraProbeSize;
	[SerializeField] private Vector3 m_TargetOffset;

	private void Start()
	{
		Cursor.visible = false;
	}

	public void RotateSpringArm(Vector2 change)
	{
		// Chaning the Yaw based on the mouse input
		m_TargetOffset.x += change.y * m_YawSensitivity * -1;

		// Chaning the Pitch based on the mouse input
		m_TargetOffset.y += change.x * m_PitchSensitivity;

		// Clamping camera
		m_TargetOffset.x = Mathf.Clamp(m_TargetOffset.x, -20, 90);
	}

	public void ChangeCameraDistance(float amount)
	{
		// Change zoom based on the mouse wheel input 
		m_CameraDist += amount * m_ZoomSensitivity;

		// Clamp the zoom
		m_CameraDist = Mathf.Clamp(m_CameraDist, m_MinDist, m_MaxDist);
	}

	private void LateUpdate()
	{
		m_SpringArmTarget.eulerAngles = m_TargetOffset;
		m_CameraMount.position = (m_SpringArmTarget.position + transform.position) - m_CameraMount.forward * m_CameraDist;
	}
}