using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveWheel : MonoBehaviour
{
	public event Action<bool> OnGroundedChanged;

	[SerializeField] private Rigidbody m_RB;
	[SerializeField] private TankSO m_Data;
	[SerializeField] private Suspension[] m_SuspensionWheels;
	private int m_NumGroundedWheels;
	private bool m_Grounded;

	private float m_Acceleration;
	public void SetAcceleration(float amount)
	{
		m_Acceleration = amount;
	}

	public void Init(TankSO inData)
	{
		m_Data = inData;
		foreach (Suspension Wheel in m_SuspensionWheels)
		{
			Wheel.Init(m_Data.SuspensionData);
			Wheel.OnGroundedChanged += Handle_WheelGroundedChanged;
        }
	}

	private void Handle_WheelGroundedChanged(bool newGrounded)
	{
		m_Grounded = newGrounded;
	}

	private void FixedUpdate()
	{
		if (m_Grounded)
		{
			MoveTank();
		}
    }

	void MoveTank()
	{
        float speed = Vector3.Dot(transform.forward, m_RB.velocity) * 3.6f /*mph to kmh*/;

        float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(speed) / 11.11f  /* 11.11 is the tank max speed*/);

        float DesiredAcceleration = m_Data.EngineData.HorsePower / (m_RB.mass * Mathf.Abs(speed)) * m_Acceleration;

        m_RB.AddForceAtPosition(transform.forward * normalizedSpeed * DesiredAcceleration, transform.position, ForceMode.Acceleration);
    }
}