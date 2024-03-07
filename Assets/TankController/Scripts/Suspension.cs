using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suspension : MonoBehaviour
{
	public event Action<bool> OnGroundedChanged; 

	[SerializeField] private Transform m_Wheel;
	[SerializeField] private Rigidbody m_RB;

	private SuspensionSO m_Data;
	private float m_SpringSize;
	private bool m_Grounded;

    RaycastHit hit;

    public void Init(SuspensionSO inData)
	{
		m_Data = inData;
		m_SpringSize = Mathf.Abs(transform.localPosition.y) + (m_Data.WheelDiameter / 2);
    }

	public bool GetGrounded()
	{      
        if (Physics.Raycast(transform.position, -transform.up, out hit, m_SpringSize, m_Data.SuspensionLayermask))
        {
            m_Grounded = true;
        }
        else m_Grounded = false;

		OnGroundedChanged?.Invoke(m_Grounded);

		return false;
	}

	private void FixedUpdate()
	{
		GetGrounded();

        if (m_Grounded)
        {
			Vector3 dir = m_Wheel.up;
			Vector3 worldVel = m_RB.GetPointVelocity(m_Wheel.position);

			float suspensionOffset = m_SpringSize - hit.distance;
			float suspensionVel = Vector3.Dot(dir, worldVel);
			float suspensionForce = (suspensionOffset * m_Data.SuspensionStrength) - (suspensionVel * m_Data.SuspensionDamper);

			m_RB.AddForceAtPosition(dir * (suspensionForce * m_RB.mass), transform.position, ForceMode.Force);
			m_Wheel.localPosition = Vector3.down * (hit.distance - m_Data.WheelDiameter / 2);
        }
    }
}
