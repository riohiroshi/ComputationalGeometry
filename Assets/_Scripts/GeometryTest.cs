using UnityEngine;

public class GeometryTest : MonoBehaviour
{
    [SerializeField] private Transform _lineStart;
    [SerializeField] private Transform _lineEnd;
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;


    private void OnDrawGizmos()
    {
        if (_lineStart == null) { return; }
        if (_lineEnd == null) { return; }
        if (_pointA == null) { return; }
        if (_pointB == null) { return; }

        var start = _lineStart.position;
        var end = _lineEnd.position;
        var pA = _pointA.position;
        var pB = _pointB.position;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(start, 0.25f);
        Gizmos.DrawSphere(end, 0.25f);
        Gizmos.DrawLine(start, end);

        //// Test is point on line
        //Gizmos.color = MathLibrary.IsOnLine(start, end, pA) ? Color.green : Color.red;
        //Gizmos.DrawSphere(pA, 0.25f);

        //// Test point at which side of line
        //var isRight = MathLibrary.IsOnRight(start, end, pA);
        //if (isRight < 0f) { Gizmos.color = Color.red; }
        //else if (isRight > 0f) { Gizmos.color = Color.blue; }
        //else { Gizmos.color = Color.green; }
        //Gizmos.DrawSphere(pA, 0.25f);

        //// Test if 2 points are at the same side of line
        //var isRightA = MathLibrary.IsOnRight(start, end, pA);
        //if (isRightA < 0f) { Gizmos.color = Color.red; }
        //else if (isRightA > 0f) { Gizmos.color = Color.blue; }
        //else { Gizmos.color = Color.green; }
        //Gizmos.DrawSphere(pA, 0.25f);
        //var isRightB = MathLibrary.IsOnRight(start, end, pB);
        //if (isRightB < 0f) { Gizmos.color = Color.red; }
        //else if (isRightB > 0f) { Gizmos.color = Color.blue; }
        //else { Gizmos.color = Color.green; }
        //Gizmos.DrawSphere(pB, 0.25f);
        //Gizmos.color = MathLibrary.IsOnSameSide(start, end, pA, pB) ? Color.red : Color.green;
        //Gizmos.DrawLine(pA, pB);

        //// Test if 2 lines are parallel
        //Gizmos.DrawWireSphere(pA, 0.25f);
        //Gizmos.DrawSphere(pB, 0.25f);
        //Gizmos.color = MathLibrary.IsParallel(start, end, pA, pB) ? Color.green : Color.red;
        //Gizmos.DrawLine(pA, pB);

        //// Test if 2 lines are perpendicular
        //Gizmos.DrawWireSphere(pA, 0.25f);
        //Gizmos.DrawSphere(pB, 0.25f);
        //Gizmos.color = MathLibrary.IsPerpendicular(start, end, pA, pB) ? Color.green : Color.red;
        //Gizmos.DrawLine(pA, pB);

        //// Test if 2 lines are intersect
        //Gizmos.DrawWireSphere(pA, 0.25f);
        //Gizmos.DrawSphere(pB, 0.25f);
        //Gizmos.color = MathLibrary.IsIntersect(start, end, pA, pB) ? Color.green : Color.red;
        //Gizmos.DrawLine(pA, pB);


    }
}