using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SampleSpline : MonoBehaviour
{
	[SerializeField] SplineContainer m_splineContainer;
	private void Awake()
	{
		m_splineContainer = GetComponent<SplineContainer>();
	}

	public void SampleSplineWidth(SplineContainer m_splineContainer, float time, out Vector3 pos1, 
        out Vector3 pos2, float3 position, float3 tangent, float3 upVector, float m_width)
    {
        // Evaluates the splines such as getting the position of the knots
        m_splineContainer.Evaluate(0, time, out position, out tangent, out upVector);

        // Tangent is the forward direction of the travel along the spline to the point
        // Find the right direction based on this
        float3 right = Vector3.Cross(tangent, upVector).normalized;
        pos1 = position + (right * m_width);
        pos2 = position + (-right * m_width);
    }
}
