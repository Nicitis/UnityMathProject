// TO DO: 이제 t가 아니라 dist를 기준으로 균일하게 샘플링해야 함. 그러기 위해서 DrawPoint 함수를 새로 만들어서 에디터가 아니라 카메라 씬 뷰에서도 각 점이 명확하게 보이게 해야 함.
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class CubicBezierCurve : MonoBehaviour
{
    public Transform P0;
    public Transform P1;
    public Transform P2;
    public Transform P3;
    
    [Range(2, 100)]
    public int numberOfSamples = 2;
    public LineRenderer line;

    public bool drawGizmos = true;

    private Vector3 previousP0;
    private Vector3 previousP1;
    private Vector3 previousP2;
    private Vector3 previousP3;
    private int previousNumberOfSamples = 0;
    private Vector3[] points;

    private Matrix4x4 bezierMatrix = new Matrix4x4(
        new Vector4(1, -3, 3, -1),
        new Vector4(0, 3, -6, 3),
        new Vector4(0, 0, 3, -3),
        new Vector4(0, 0, 0, 1)
    );

    #if UNITY_EDITOR
    [Button(nameof(TimeComplexityTest))]
    public bool testButtonField;
    public int numberOfTry = 10000;
    #endif

    private void Update()
    {
        CheckAndUpdateCurve();
    }

    private void CheckAndUpdateCurve()
    {
        if (numberOfSamples == previousNumberOfSamples &&
            P0.position == previousP0 &&
            P1.position == previousP1 &&
            P2.position == previousP2 &&
            P3.position == previousP3)
            return;
        
        previousP0 = P0.position;
        previousP1 = P1.position;
        previousP2 = P2.position;
        previousP3 = P3.position;

        previousNumberOfSamples = numberOfSamples; // 샘플의 수가 바뀔 경우 갱신해준다.

        CreateCurve();
    }

    // if the user changes the number of samples, create new bezier curve.
    // 우선, 균일한 t 값으로 샘플링한다.
    public void CreateCurve()
    {
        CreateCurveByPolynomialForm();
    }

    public void CreateCurveByDeCasteljau()
    {
        float dt = 1f / (numberOfSamples - 1);

        points = new Vector3[numberOfSamples];

        points[0] = P0.position;
        points[numberOfSamples - 1] = P3.position;

        for (int i = 1; i < numberOfSamples - 1; i++)
        {
            points[i] = BezierCurveByDeCasteljau(i * dt);
        }

        line.positionCount = numberOfSamples;
        line.SetPositions(points);
    }

    public void CreateCurveByPolynomialForm()
    {
        float dt = 1f / (numberOfSamples - 1);

        points = new Vector3[numberOfSamples];

        points[0] = P0.position;
        points[numberOfSamples - 1] = P3.position;

        Matrix4x4 controlPoints = new Matrix4x4(P3.position, P2.position, P1.position, P0.position);

        for (int i = 1; i < numberOfSamples - 1; i++)
        {
            points[i] = BezierCurveByBernsteinPolynomialForm(i * dt, controlPoints);
        }

        line.positionCount = numberOfSamples;
        line.SetPositions(points);
    }

    // De Casteljau의 방법으로 베지에 곡선 상의 점을 구한다.
    public Vector3 BezierCurveByDeCasteljau(float t)
    {
        if (t >= 0 && t <= 1)
        {
            // P0, P1, P2, P3 상에서 보간된 점
            Vector3 A = Vector3.Lerp(P0.position, P1.position, t);
            Vector3 B = Vector3.Lerp(P1.position, P2.position, t);
            Vector3 C = Vector3.Lerp(P2.position, P3.position, t);
            // A, B, C 상에서 보간된 점
            Vector3 D = Vector3.Lerp(A, B, t);
            Vector3 E = Vector3.Lerp(B, C, t);
            // 최종 보간된 점
            Vector3 pointOnCurve = Vector3.Lerp(D, E, t);

            return pointOnCurve;
        }

        // if t is out of range, get the point along the curve by extrapolation.
        if (t <= 0)
            return Vector3.Lerp(P0.position, P1.position, t);
        else
            return Vector3.Lerp(P2.position, P3.position, t);
    }

    public Vector3 BezierCurveByBernsteinPolynomialForm(float t, Matrix4x4 controlPoints)
    {
        if (t >= 0 && t <= 1)
        {
            Vector4 tVector = new Vector4(t*t*t, t*t, t, 1);
            
            return (Vector3)(controlPoints * bezierMatrix * tVector);
        }

        // if t is out of range, get the point along the curve by extrapolation.
        if (t <= 0)
            return Vector3.Lerp(P0.position, P1.position, t);
        else
            return Vector3.Lerp(P2.position, P3.position, t);
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

    #if UNITY_EDITOR
    public void TimeComplexityTest()
    {
        Debug.Log("Start the test for time complexity.");

        Stopwatch sw = new Stopwatch();
        sw.Start();
        for (int i = 0; i < numberOfTry; i++)
        {
            CreateCurveByDeCasteljau();
        }
        sw.Stop();

        Debug.Log("Elapsed time by De Casteljau Algorithm : " + sw.ElapsedMilliseconds.ToString() + "ms.");
        sw.Reset();

        sw.Start();
        for (int i = 0; i < numberOfTry; i++)
        {
            CreateCurveByPolynomialForm();
        }
        sw.Stop();
        Debug.Log("Elapsed time by Bernstein Polynomial Form : " + sw.ElapsedMilliseconds.ToString() + "ms.");

    }
    #endif
}
