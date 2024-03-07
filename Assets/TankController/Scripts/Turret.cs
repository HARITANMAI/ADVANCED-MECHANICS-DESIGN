using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Turret : MonoBehaviour
{
	[SerializeField] private Transform m_CameraMount;
	[SerializeField] private Transform m_Turret;
	[SerializeField] private Transform m_Barrel;

	private TankSO m_Data;
	private bool m_RotationDirty;
	private Coroutine m_CRAimingTurret;

	private void Awake()
	{
	}

	public void Init(TankSO inData)
	{
		m_Data = inData;
	}

	public void SetRotationDirty()
	{
		
	}

	private IEnumerator C_AimTurret()
	{
		yield return null;
	}

	private void FixedUpdate()
	{
		Vector3 camfwdVec = m_CameraMount.forward;

		Vector3 projectedVec = Vector3.ProjectOnPlane(camfwdVec, transform.up);

		Quaternion targetRot = Quaternion.LookRotation(projectedVec, m_Turret.up);

		m_Turret.rotation = Quaternion.RotateTowards(m_Turret.rotation, targetRot, m_Data.TurretData.TurretTraverseSpeed * Time.deltaTime);
	}
}