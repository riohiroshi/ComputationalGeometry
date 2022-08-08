using UnityEngine;
using UnityEditor;

public class GeometryTest : MonoBehaviour
{
    [SerializeField] private Transform _lineStart;
    [SerializeField] private Transform _lineEnd;
    [SerializeField] private Transform _pointP;
    [SerializeField] private Transform _pointQ;


    private void OnDrawGizmos()
    {
        DrawCoordinate();

        DrawPoint();


        //if (_lineStart == null) { return; }
        //if (_lineEnd == null) { return; }
        //if (_pointP == null) { return; }
        //if (_pointQ == null) { return; }

        //var start = _lineStart.position;
        //var end = _lineEnd.position;
        //var pP = _pointP.position;
        //var pQ = _pointQ.position;

        //Gizmos.color = Color.yellow;

        //Gizmos.DrawWireSphere(start, 0.25f);
        //Gizmos.DrawSphere(end, 0.25f);
        //Gizmos.DrawLine(start, end);

        //Handles.Label(start + new Vector3(0, 0, -0.5f), $"A");
        //Handles.Label(end + new Vector3(0, 0, -0.5f), $"B");

        ////// Test is point on line
        ////Gizmos.color = MathLibrary.IsOnLine(start, end, pP) ? Color.green : Color.red;
        ////Gizmos.DrawSphere(pP, 0.25f);

        ////// Test point at which side of line
        ////var isRight = MathLibrary.IsOnRight(start, end, pP);
        ////if (isRight < 0f) { Gizmos.color = Color.red; }
        ////else if (isRight > 0f) { Gizmos.color = Color.blue; }
        ////else { Gizmos.color = Color.green; }
        ////Gizmos.DrawSphere(pP, 0.25f);

        //// Test if 2 points are at the same side of line
        //var isRightA = MathLibrary.IsOnRight(start, end, pP);
        //if (isRightA < 0f) { Gizmos.color = Color.red; }
        //else if (isRightA > 0f) { Gizmos.color = Color.blue; }
        //else { Gizmos.color = Color.green; }
        //Gizmos.DrawSphere(pP, 0.25f);
        //var isRightB = MathLibrary.IsOnRight(start, end, pQ);
        //if (isRightB < 0f) { Gizmos.color = Color.red; }
        //else if (isRightB > 0f) { Gizmos.color = Color.blue; }
        //else { Gizmos.color = Color.green; }
        //Gizmos.DrawSphere(pQ, 0.25f);
        //Gizmos.color = MathLibrary.IsOnSameSide(start, end, pP, pQ) ? Color.red : Color.green;
        //Gizmos.DrawLine(pP, pQ);

        ////// Test if 2 lines are parallel
        ////Gizmos.DrawWireSphere(pP, 0.25f);
        ////Gizmos.DrawSphere(pQ, 0.25f);
        ////Gizmos.color = MathLibrary.IsParallel(start, end, pP, pQ) ? Color.green : Color.red;
        ////Gizmos.DrawLine(pP, pQ);

        ////// Test if 2 lines are perpendicular
        ////Gizmos.DrawWireSphere(pP, 0.25f);
        ////Gizmos.DrawSphere(pQ, 0.25f);
        ////Gizmos.color = MathLibrary.IsPerpendicular(start, end, pP, pQ) ? Color.green : Color.red;
        ////Gizmos.DrawLine(pP, pQ);

        ////// Test if 2 lines are intersect
        ////Gizmos.DrawWireSphere(pP, 0.25f);
        ////Gizmos.DrawSphere(pQ, 0.25f);
        ////Gizmos.color = MathLibrary.IsIntersect(start, end, pP, pQ) ? Color.green : Color.red;
        ////Gizmos.DrawLine(pP, pQ);

        //Handles.Label(pP + new Vector3(0, 0, -0.5f), $"P");
        //Handles.Label(pQ + new Vector3(0, 0, -0.5f), $"Q");
    }

    private void DrawCoordinate()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Vector3.back * 100f, Vector3.forward * 100);

        for (int i = 0; i < 20; i++)
        {
            Handles.Label(Vector3.forward * i, $"{i}");
            Handles.Label(Vector3.back * i, $"{-i}");
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.left * 100f, Vector3.right * 100f);

        for (int i = 0; i < 20; i++)
        {
            Handles.Label(Vector3.right * i, $"{i}");
            Handles.Label(Vector3.left * i, $"{-i}");
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(Vector3.down * 100f, Vector3.up * 100f);

        for (int i = 0; i < 20; i++)
        {
            Handles.Label(Vector3.up * i, $"{i}");
            Handles.Label(Vector3.down * i, $"{-i}");
        }
    }

    private void DrawPoint()
    {
        if (_pointP == null) { return; }

        var pP = _pointP.position;

        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(pP, pP + _pointP.forward);
        //Gizmos.DrawSphere(pP + _pointP.forward, 0.1f);

        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(pP, pP + _pointP.right);
        //Gizmos.DrawSphere(pP + _pointP.right, 0.1f);

        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(pP, pP + _pointP.up);
        //Gizmos.DrawSphere(pP + _pointP.up, 0.1f);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(pP, 0.025f);

        var rot = _pointP.rotation;
        var theta = Mathf.Acos(rot.w) * 2 * Mathf.Rad2Deg;
        var s = Mathf.Sin(Mathf.Acos(rot.w)) + float.Epsilon;

        var axis = new Vector3(rot.x / s, rot.y / s, rot.z / s);

        Handles.Label(pP + Vector3.down * 0.1f, $"{(theta)}, ({(rot.x / s)}, {(rot.y / s)}, {(rot.z / s)})");

        Gizmos.DrawLine(pP, pP + axis);
    }
}