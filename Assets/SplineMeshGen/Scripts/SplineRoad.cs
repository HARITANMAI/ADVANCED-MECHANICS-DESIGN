using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode()]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SplineRoad : MonoBehaviour
{
    public SplineContainer m_SplineContainer;
    [SerializeField] int m_SplineIndex;
    private List<Vector3> m_verP1;
    private List<Vector3> m_verP2;

    float3 position;
    float3 tangent;
    float3 upVector;

    private MeshFilter m_Filter;
    private Mesh m_Mesh;
    public float m_width = 2f;
    [SerializeField] int resolution;

    SampleSpline sample;

	private void Awake()
	{
		m_Filter = GetComponent<MeshFilter>();
        m_SplineContainer = GetComponent<SplineContainer>();
        sample = GetComponent<SampleSpline>();
	}

	private void Update()
	{
		ProceduralMeshGen();
	}

	private void OnEnable()
	{
        Spline.Changed += OnSplineChanged;
	}

    private void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }

    private void OnSplineChanged(Spline arg1, int arg2, SplineModification arg3)
    {
        ProceduralMeshGen();
    }

    private void ProceduralMeshGen()
    {
        m_verP1 = new List<Vector3>();
        m_verP2 = new List<Vector3>();

        float step = 1 / (float)resolution;

        Vector3 p1;
        Vector3 p2;

        for (int i = 0; i < resolution; i++)
        {
            float time = step * i;
            sample.SampleSplineWidth(m_SplineContainer, time, out p1, out p2, position, tangent, upVector, m_width);

            // Fix the position of the generation of the mesh
            p1 -= transform.position;
            p2 -= transform.position;

            m_verP1.Add(p1);
            m_verP2.Add(p2);
        }

        sample.SampleSplineWidth(m_SplineContainer, 1f, out p1, out p2, position, tangent, upVector, m_width);

        p1 -= transform.position;
        p2 -= transform.position;

        m_verP1.Add(p1);
        m_verP2.Add(p2);

        // Generate mesh, vertice list and triangles
        m_Mesh = new Mesh { name = "Proc Road Mesh" };
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        int offset = 0;

        for (int currentSplineIndex = 0; currentSplineIndex < 1; currentSplineIndex++)
        {
            int splineOffset = (resolution * m_SplineIndex) + currentSplineIndex;

            for (int currentSplinePoint = 1; currentSplinePoint < resolution + 1; currentSplinePoint++)
            {
                // Gets the vertices from the position list
                int verticesOffset = splineOffset + currentSplinePoint;
                Vector3 Pos1 = m_verP1[verticesOffset - 1];
                Vector3 Pos2 = m_verP2[verticesOffset - 1];
                Vector3 Pos3 = m_verP1[verticesOffset];
                Vector3 Pos4 = m_verP2[verticesOffset];

                offset = 4 * resolution * m_SplineIndex;
                offset += 4 * (currentSplinePoint - 1);

                // Connect the triangles from vertices
                int tris1 = offset + 0;
                int tris2 = offset + 2;
                int tris3 = offset + 3;

                int tris4 = offset + 3;
                int tris5 = offset + 1;
                int tris6 = offset + 0;

                vertices.AddRange(new List<Vector3> { Pos1, Pos2, Pos3, Pos4 });
                triangles.AddRange(new List<int> { tris1, tris2, tris3, tris4, tris5, tris6 });
            }
        }

        m_Mesh.SetVertices(vertices);
        m_Mesh.SetTriangles(triangles, 0);
        m_Filter.mesh = m_Mesh;
    }

	IEnumerator UpdateSpline_CR()
	{
		ProceduralMeshGen();
		yield return null;
	}

	private void OnDrawGizmos()
	{
        for (int i = 0; i < m_verP1.Count; i++)
        {
            Handles.SphereHandleCap(0, transform.position + transform.InverseTransformVector(m_verP1[i]), Quaternion.identity, 1f, EventType.Repaint);
            Handles.SphereHandleCap(0, transform.position + transform.InverseTransformVector(m_verP2[i]), Quaternion.identity, 1f, EventType.Repaint);

            Handles.DrawLine(transform.position + transform.InverseTransformVector(m_verP1[i]), transform.position + transform.InverseTransformVector(m_verP2[i]));
        }
	}
}
