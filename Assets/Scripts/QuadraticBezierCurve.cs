using UnityEngine;

public class QuadraticBezierCurve : MonoBehaviour
{
    public Transform P1;
    public Transform P2;
    public Transform P3;
    
    [Range(2, 100)]
    public int numberOfSamples = 2;
    public LineRenderer line;

    public bool drawGizmos = true;

    private Vector3 previousP1;
    private Vector3 previousP2;
    private Vector3 previousP3;
    private int previousNumberOfSamples = 0;
    private Vector3[] points;

    private void Update()
    {
        CheckAndUpdateCurve();
    }

    private void CheckAndUpdateCurve()
    {
        if (numberOfSamples == previousNumberOfSamples &&
            P1.position == previousP1 &&
            P2.position == previousP2 &&
            P3.position == previousP3)
            return;
        
        previousP1 = P1.position;
        previousP2 = P2.position;
        previousP3 = P3.position;

        CreateCurve();
    }

    // if the user changes the number of samples, create new bezier curve.
    // 우선, 균일한 t 값으로 샘플링한다.
    public void CreateCurve()
    {
        float dt = 1f / (numberOfSamples - 1);

        previousNumberOfSamples = numberOfSamples; // 샘플의 수가 바뀔 경우 갱신해준다.

        points = new Vector3[numberOfSamples];

        points[0] = P1.position;
        points[numberOfSamples - 1] = P3.position;

        for (int i = 1; i < numberOfSamples - 1; i++)
        {
            points[i] = BezierCurve(i * dt);
        }

        line.positionCount = numberOfSamples;
        line.SetPositions(points);
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            CheckAndUpdateCurve();

            Gizmos.color = Color.red;
            for (int i = 0; i < numberOfSamples - 1; i++)
            {
                Gizmos.DrawLine(points[i], points[i+1]);
            }
        }
    }

    // De Casteljau의 방법으로 베지에 곡선 상의 점을 구한다.
    public Vector3 BezierCurve(float t)
    {
        if (t >= 0 && t <= 1)
        {
            Vector3 midP1P2 = Vector3.Lerp(P1.position, P2.position, t);
            Vector3 midP2P3 = Vector3.Lerp(P2.position, P3.position, t);
            Vector3 pointOnCurve = Vector3.Lerp(midP1P2, midP2P3, t);

            return pointOnCurve;
        }

        // if t is out of range, get the point along the curve by extrapolation.
        if (t <= 0)
            return Vector3.Lerp(P1.position, P2.position, t);
        else
            return Vector3.Lerp(P2.position, P3.position, t);
    }
}
