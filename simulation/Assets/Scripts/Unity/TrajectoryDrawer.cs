// DroneTrajectoryDrawer.cs
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryDrawer : MonoBehaviour
{
    public Transform droneTransform; // �h���[���{�̂�Transform�������ɓ����
    public float minDistance = 0.1f; // �ŏ��ړ������i�_��łԊu�j

    private LineRenderer lineRenderer;
    private List<Vector3> positions = new List<Vector3>();

    void Start()
    {
        lineRenderer = this.gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // �ȈՃ��C���p

        // �O���f�[�V�������쐬
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.green, 0.0f),
                new GradientColorKey(Color.green, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(1.0f, 1.0f)
            }
        );
        lineRenderer.colorGradient = gradient;
        lineRenderer.useWorldSpace = true;
        if (droneTransform == null)
        {
            Debug.LogError("Drone Transform not set!");
        }
    }

    void Update()
    {
        if (droneTransform == null) return;

        Vector3 currentPos = droneTransform.position;

        if (positions.Count == 0 || Vector3.Distance(currentPos, positions[positions.Count - 1]) > minDistance)
        {
            positions.Add(currentPos);
            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());
        }
    }
}
