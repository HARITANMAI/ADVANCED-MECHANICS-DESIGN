using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRotation : MonoBehaviour
{
	private Camera m_Camera;
	[SerializeField, Range(0.1f, 500f)] private float m_rotSpeed;

	private void Awake()
	{
		m_Camera = Camera.main; //using camera.main in update is bad
	}

	private void Update()
	{
		//project cameras forward vector onto the plane of the tank
		Vector3 projectedVector = Vector3.ProjectOnPlane(m_Camera.transform.forward, transform.parent.up);

		//build a quat that looks along this
		Quaternion targetRot = Quaternion.LookRotation(projectedVector, transform.parent.up);

		//draw line to show it working
		Debug.DrawLine(transform.position, transform.position + projectedVector * 50, Color.red);

		// slowly rotate towards that orientation
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, m_rotSpeed * Time.deltaTime);
	}
}