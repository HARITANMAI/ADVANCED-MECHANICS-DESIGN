using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class TankController : MonoBehaviour
{
	private AM_02Tank m_ActionMap;
	[SerializeField] private TankSO m_Data;
	[SerializeField] private Rigidbody m_RB;
	[SerializeField] private CameraController m_CameraController;
	[SerializeField] private Turret m_TurretController;
	[SerializeField] private DriveWheel[] m_DriveWheels;
	[SerializeField] private GameObject shellPrefab;
	[SerializeField] private Transform cannonTip;
	[SerializeField] float shellForce = 100f;
	private int m_NumSuspensionsGrounded;

	[SerializeField] TextMeshProUGUI shellSpeedText;

	public float shellSpeed;

	Rigidbody ShellRB;

	private float m_InAccelerate;

	private float m_InSteer;
	private bool m_IsSteering;
	private Coroutine m_CRSteer;

	private bool m_IsFiring;
	private Coroutine m_CRFire;

	private void Awake()
	{
		m_ActionMap = new AM_02Tank();
		m_RB = GetComponent<Rigidbody>();
		m_CameraController = GetComponent<CameraController>();
		m_TurretController = GetComponent<Turret>();
		m_NumSuspensionsGrounded = 0;
		foreach (DriveWheel wheel in m_DriveWheels)
		{
			wheel.Init(m_Data);
			wheel.OnGroundedChanged += Handle_SuspensionGroundedChanged;
		}
		m_TurretController.Init(m_Data);
	}

	private void FixedUpdate()
	{
		if (ShellRB != null)
		{
			shellSpeed = ShellRB.velocity.magnitude;
			UpdateSpeedText(shellSpeed);
		}
	}

	private void OnEnable()
	{
		m_ActionMap.Enable();

		m_ActionMap.Default.Accelerate.performed += Handle_AcceleratePerformed;
		m_ActionMap.Default.Accelerate.canceled += Handle_AccelerateCanceled;
		m_ActionMap.Default.Steer.performed += Handle_SteerPerformed;
		m_ActionMap.Default.Steer.canceled += Handle_SteerCanceled;
		m_ActionMap.Default.Fire.performed += Handle_FirePerformed;
		m_ActionMap.Default.Fire.canceled += Handle_FireCanceled;
		m_ActionMap.Default.Aim.performed += Handle_AimPerformed;
		m_ActionMap.Default.Zoom.performed += Handle_ZoomPerformed;
	}
	private void OnDisable()
	{
		m_ActionMap.Disable();

		m_ActionMap.Default.Accelerate.performed -= Handle_AcceleratePerformed;
		m_ActionMap.Default.Accelerate.canceled -= Handle_AccelerateCanceled;
		m_ActionMap.Default.Steer.performed -= Handle_SteerPerformed;
		m_ActionMap.Default.Steer.canceled -= Handle_SteerCanceled;
		m_ActionMap.Default.Fire.performed -= Handle_FirePerformed;
		m_ActionMap.Default.Fire.canceled -= Handle_FireCanceled;
		m_ActionMap.Default.Aim.performed -= Handle_AimPerformed;
		m_ActionMap.Default.Zoom.performed -= Handle_ZoomPerformed;
	}

	private void Handle_AcceleratePerformed(InputAction.CallbackContext context)
	{
		m_InAccelerate = context.ReadValue<float>();
		foreach (DriveWheel wheel in m_DriveWheels)
		{
			wheel.SetAcceleration(m_InAccelerate);
		}
		m_TurretController.SetRotationDirty();
	}
	private void Handle_AccelerateCanceled(InputAction.CallbackContext context)
	{
		m_InAccelerate = context.ReadValue<float>();
		foreach (DriveWheel wheel in m_DriveWheels)
		{
			wheel.SetAcceleration(m_InAccelerate);
		}
		m_TurretController.SetRotationDirty();
	}

	private void Handle_SteerPerformed(InputAction.CallbackContext context)
	{
		m_InSteer = context.ReadValue<float>();

		if (m_IsSteering) return;

		m_IsSteering = true;

		m_CRSteer = StartCoroutine(C_SteerUpdate());
	}
	private void Handle_SteerCanceled(InputAction.CallbackContext context)
	{
		m_InSteer = context.ReadValue<float>();

		if (!m_IsSteering) return;

		m_IsSteering = false;

		m_RB.angularVelocity = Vector3.zero;

		StopCoroutine(m_CRSteer);
	}
	private IEnumerator C_SteerUpdate()
	{
		while (m_IsSteering)
		{
			// ADD FORCE AT POSITION (FORWARD DIR OF TANK * TURN SPEED * STEERING INPUT, drive wheel[0].position, forcemode.force)
			m_RB.AddForceAtPosition(transform.forward * m_InSteer * m_Data.SuspensionData.HullTraverseDegrees, m_DriveWheels[0].transform.position, ForceMode.Force);
			m_RB.AddForceAtPosition(transform.forward * -m_InSteer * m_Data.SuspensionData.HullTraverseDegrees, m_DriveWheels[1].transform.position, ForceMode.Force);
			yield return null;
		}
	}

	private void Handle_FirePerformed(InputAction.CallbackContext context)
	{
		if (m_IsFiring) return;

		m_IsFiring = true;

		GameObject shell = Instantiate(shellPrefab, cannonTip.position, cannonTip.rotation);
		ShellRB = shell.GetComponent<Rigidbody>();
		// Constants placed here so i can see / change them easier for now!
		float shellMass = shellPrefab.GetComponent<Rigidbody>().mass;     // Mass of the bullet
		  // Desired force applied by the bullet

		float shellLifeTime = 10f;

		// Check player's movement (Angle not really necessary, might remove)
		float movementSpeed = GetComponent<Rigidbody>().velocity.magnitude;

		// Apply the force to the bullet using the total mass and acceleration
		Vector3 forceDirection = cannonTip.forward;
		Vector3 force = shellMass * shellForce * forceDirection;
		ShellRB.AddForce(force, ForceMode.Impulse);

		shellSpeed = ShellRB.velocity.magnitude;


		UpdateSpeedText(shellSpeed);

		Destroy(shell, shellLifeTime);
	}

	private void UpdateSpeedText(float Speed)
	{
		shellSpeedText.text = Speed.ToString() + " m/s";
	}

	private void Handle_FireCanceled(InputAction.CallbackContext context)
	{
		if (!m_IsFiring) return;

		m_IsFiring = false;

		StopCoroutine(m_CRFire);
	}
	private IEnumerator C_FireUpdate()
	{
		while (m_IsFiring)
		{
			yield return null;
		}
	}

	private void Handle_AimPerformed(InputAction.CallbackContext context)
	{
		m_CameraController.RotateSpringArm(context.ReadValue<Vector2>());
		m_TurretController.SetRotationDirty();
	}

	private void Handle_ZoomPerformed(InputAction.CallbackContext context)
	{
		m_CameraController.ChangeCameraDistance(context.ReadValue<float>());
		m_TurretController.SetRotationDirty();
	}

	private void Handle_SuspensionGroundedChanged(bool newGrounded)
	{

	}
}