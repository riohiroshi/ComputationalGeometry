using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using NaughtyAttributes;
using System.Linq;

[ExecuteInEditMode]
public class ConvexHull : MonoBehaviour
{
    [SerializeField] private Vector2 _generateBound = new Vector2(7.5f, 7.5f);
    [SerializeField] private Transform _generateBallPrefab;

    [Min(3)]
    [SerializeField] private int _ballAmount = 15;

    [SerializeField] private Transform _generateParent;

    [SerializeField] private Transform[] _ballArray;

    private List<Transform> _ballList = new List<Transform>();
    private List<Transform> _convexHullPoints = new List<Transform>();

    [Button]
    private void DestroyChildren()
    {
        for (int i = _generateParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(_generateParent.GetChild(i).gameObject);
        }

        _generateParent.DetachChildren();

        _ballList.Clear();
        _ballArray = _ballList.ToArray();

        _convexHullPoints.Clear();
    }

    [Button]
    private void GenerateBalls()
    {
        DestroyChildren();

        for (int i = 0; i < _ballAmount; i++)
        {
            var ball = Instantiate(_generateBallPrefab, _generateParent);
            ball.name = $"{i:00}";
            ball.position = new Vector3(Random.Range(-_generateBound.x, _generateBound.x), 0f, Random.Range(-_generateBound.y, _generateBound.y));
            _ballList.Add(ball);
        }

        _ballArray = _ballList.ToArray();
    }

    private void ResetBallList()
    {
        _ballList = new List<Transform>();

        for (int i = 0; i < _ballArray.Length; i++)
        {
            _ballList.Add(_ballArray[i]);
        }
    }

    [Button]
    private void StartGrahamScan()
    {
        StartCoroutine(GrahamScan());
    }

    private IEnumerator GrahamScan()
    {
        ResetBallList();

        var bottomLeftPoint = FindBottomLeftPoint();

        SortPoints();

        _convexHullPoints = new List<Transform>();

        for (int i = 0; i < _ballArray.Length; i++)
        {
            while (_convexHullPoints.Count >= 3)
            {
                var lastIndex = _convexHullPoints.Count - 1;
                var nextPoint = _ballArray[i].position;
                var lastPoint = _convexHullPoints[lastIndex].position;
                var last2Point = _convexHullPoints[lastIndex - 1].position;

                if (IsOnRight(nextPoint, lastPoint, last2Point)) { break; }

                _convexHullPoints.RemoveAt(_convexHullPoints.Count - 1);

                yield return new WaitForSeconds(0.5f);
            }

            _convexHullPoints.Add(_ballArray[i]);

            yield return new WaitForSeconds(0.5f);
        }

        Transform FindBottomLeftPoint()
        {
            Transform bottomLeft = _ballArray[0];

            foreach (var ball in _ballArray)
            {
                if (ball.position.z > bottomLeft.position.z) { continue; }
                if (ball.position.z == bottomLeft.position.z && ball.position.x > bottomLeft.position.x) { continue; }

                bottomLeft = ball;
            }

            return bottomLeft;
        }

        void SortPoints()
        {
            _ballList.Remove(bottomLeftPoint);

            QuickSort(_ballList, 0, _ballList.Count - 1, CompareAngle);

            _ballList.Insert(0, bottomLeftPoint);

            _ballArray = _ballList.ToArray();
        }

        bool CompareAngle(Transform point0, Transform point1)
        {
            var line0 = point0.position - bottomLeftPoint.position;
            var line1 = point1.position - bottomLeftPoint.position;

            var y = Vector3.Cross(line0, line1).y;

            if (y > 0f) { return true; }
            if (y == 0f && (line0.sqrMagnitude < line1.sqrMagnitude)) { return true; }

            return false;
        }

        bool IsOnRight(Vector3 nextPoint, Vector3 lastPoint, Vector3 last2Point)
        {
            var lastLine = lastPoint - last2Point;
            var nextLine = nextPoint - lastPoint;

            return Vector3.Cross(lastLine, nextLine).y > 0f;
        }
    }

    [Button]
    private void StartAndrewMonotoneChain()
    {
        StartCoroutine(AndrewMonotoneChain());
    }

    private IEnumerator AndrewMonotoneChain()
    {
        ResetBallList();

        SortPoints();

        _convexHullPoints = new List<Transform>();

        for (int i = 0; i < _ballArray.Length; i++)
        {
            var nextPoint = _ballArray[i].position;

            while (_convexHullPoints.Count >= 2 && IsOnRight(nextPoint, _convexHullPoints[_convexHullPoints.Count - 1].position, _convexHullPoints[_convexHullPoints.Count - 2].position))
            {
                _convexHullPoints.RemoveAt(_convexHullPoints.Count - 1);
                yield return new WaitForSeconds(0.5f);
            }

            _convexHullPoints.Add(_ballArray[i]);
            yield return new WaitForSeconds(0.5f);
        }

        var t = _convexHullPoints.Count - 1;

        for (int i = _ballArray.Length - 2; i >= 0; i--)
        {
            var lastIndex = _convexHullPoints.Count - 1;
            var nextPoint = _ballArray[i].position;
            var lastPoint = _convexHullPoints[lastIndex].position;
            var last2Point = _convexHullPoints[lastIndex - 1].position;

            while (_convexHullPoints.Count - 2 >= t && IsOnRight(nextPoint, lastPoint, last2Point))
            {
                _convexHullPoints.RemoveAt(_convexHullPoints.Count - 1);

                lastIndex = _convexHullPoints.Count - 1;
                lastPoint = _convexHullPoints[lastIndex].position;
                last2Point = _convexHullPoints[lastIndex - 1].position;

                yield return new WaitForSeconds(0.5f);
            }

            _convexHullPoints.Add(_ballArray[i]);
            yield return new WaitForSeconds(0.5f);
        }

        void SortPoints()
        {
            QuickSort(_ballList, 0, _ballList.Count - 1, Compare);

            _ballArray = _ballList.ToArray();
        }

        bool Compare(Transform point0, Transform point1)
        {
            if (point0.position.x < point1.position.x) { return true; }
            if (point0.position.x == point1.position.x && point0.position.z < point1.position.z) { return true; }

            return false;
        }

        bool IsOnRight(Vector3 nextPoint, Vector3 lastPoint, Vector3 last2Point)
        {
            var lastLine = lastPoint - last2Point;
            var nextLine = nextPoint - lastPoint;

            return Vector3.Cross(lastLine, nextLine).y > 0f;
        }
    }

    [Button]
    private void StartMelkman()
    {
        StartCoroutine(Melkman());
    }

    private IEnumerator Melkman()
    {
        ResetBallList();

        _convexHullPoints = new List<Transform>();
        _convexHullPoints.Add(_ballArray[2]);
        _convexHullPoints.Add(_ballArray[2]);

        var point0 = _ballArray[0].position;
        var point1 = _ballArray[1].position;
        var point2 = _ballArray[2].position;

        if (IsOnRight(point0, point1, point2))
        {
            _convexHullPoints.Insert(1, _ballArray[0]);
            _convexHullPoints.Insert(2, _ballArray[1]);
        }
        else
        {
            _convexHullPoints.Insert(1, _ballArray[1]);
            _convexHullPoints.Insert(2, _ballArray[0]);
        }

        for (int i = 3; i < _ballArray.Length; i++)
        {
            _ballArray[i].GetComponent<Renderer>().material.color = Color.blue;

            var last = _convexHullPoints.Count - 1;
            var currentPoint = _ballArray[i].position;

            if (IsInsideConvex(currentPoint)) { continue; }

            while (!IsOnRight(_convexHullPoints[0].position, _convexHullPoints[1].position, currentPoint))
            {
                _convexHullPoints.RemoveAt(0);
                yield return new WaitForSeconds(1f);
            }

            _convexHullPoints.Insert(0, _ballArray[i]);

            yield return new WaitForSeconds(1f);


            while (!IsOnRight(_convexHullPoints[_convexHullPoints.Count - 2].position, _convexHullPoints[_convexHullPoints.Count - 1].position, currentPoint))
            {
                _convexHullPoints.RemoveAt(_convexHullPoints.Count - 1);
                yield return new WaitForSeconds(1f);

            }

            _convexHullPoints.Add(_ballArray[i]);
            yield return new WaitForSeconds(1f);

            _ballArray[i].GetComponent<Renderer>().material.color = Color.green;
        }

        bool IsOnRight(Vector3 o, Vector3 a, Vector3 b)
        {
            var oa = a - o;
            var ob = b - o;
            return Vector3.Cross(oa, ob).y > 0f;
        }

        bool IsInsideConvex(Vector3 point)
        {
            for (int i = 0; i < _convexHullPoints.Count - 1; i++)
            {
                var p0 = _convexHullPoints[i].position;
                var p1 = _convexHullPoints[i + 1].position;
                if (!IsOnRight(p0, p1, point)) { return false; }
            }

            return true;
        }
    }

    [Button]
    private void StartQuickHull()
    {
        StartCoroutine(QuickHull());
    }

    private IEnumerator QuickHull()
    {
        ResetBallList();

        FindPoints(out var p1, out var p2);
        _convexHullPoints.Add(p1);
        _convexHullPoints.Add(p2);

        yield return new WaitForSeconds(1f);

        var p3 = FindFurthestPointFromEdge(p1, p2, _ballList);
        _convexHullPoints.Add(p3);

        ClockwisePoints(ref p1, ref p2, ref p3);

        _ballList.Remove(p1);
        _ballList.Remove(p2);
        _ballList.Remove(p3);

        yield return new WaitForSeconds(1f);

        var edge_3_1 = new List<Transform>();
        var edge_2_3 = new List<Transform>();
        var edge_1_2 = new List<Transform>();

        RemovePointsWithinTriangle(p1.position, p2.position, p3.position, _ballList);

        for (int i = _ballList.Count - 1; i >= 0; i--)
        {
            var point = _ballList[i].position;

            var cross12 = (int)Mathf.Sign(Cross(p1.position, p2.position, point));
            var cross23 = (int)Mathf.Sign(Cross(p2.position, p3.position, point));
            var cross31 = (int)Mathf.Sign(Cross(p3.position, p1.position, point));

            if (cross12 != cross23 && cross23 == cross31)
            {
                edge_1_2.Add(_ballList[i]);
            }

            if (cross23 != cross31 && cross31 == cross12)
            {
                edge_2_3.Add(_ballList[i]);
            }

            if (cross31 != cross12 && cross12 == cross23)
            {
                edge_3_1.Add(_ballList[i]);
            }
        }

        var pointsOnHull_lf = CreateSubConvexHUll(p1, p3, edge_3_1);
        var pointsOnHull_fr = CreateSubConvexHUll(p3, p2, edge_2_3);
        var pointsOnHull_rl = CreateSubConvexHUll(p2, p1, edge_1_2);

        _convexHullPoints.Clear();
        _convexHullPoints.AddRange(pointsOnHull_lf);

        yield return new WaitForSeconds(1f);

        _convexHullPoints.AddRange(pointsOnHull_fr);

        yield return new WaitForSeconds(1f);

        _convexHullPoints.AddRange(pointsOnHull_rl);

        void FindPoints(out Transform p1, out Transform p2)
        {
            p1 = default;
            p2 = default;

            var minX = float.PositiveInfinity;
            var maxX = float.NegativeInfinity;

            foreach (var ball in _ballArray)
            {
                if (ball.position.x < minX)
                {
                    minX = ball.position.x;
                    p1 = ball;
                }

                if (ball.position.x > maxX)
                {
                    maxX = ball.position.x;
                    p2 = ball;
                }
            }
        }

        Transform FindFurthestPointFromEdge(Transform p1, Transform p2, List<Transform> points)
        {
            var edge = p2.position - p1.position;

            var edgeNormal = new Vector3(edge.z, 0f, -edge.x);

            var maxDist = float.NegativeInfinity;

            Transform p3 = default;

            foreach (var ball in points)
            {
                var lb = ball.position - p1.position;

                var dot = Mathf.Abs(Vector3.Dot(lb, edgeNormal));

                if (dot > maxDist)
                {
                    maxDist = dot;
                    p3 = ball;
                }
            }

            return p3;
        }

        void ClockwisePoints(ref Transform p1, ref Transform p2, ref Transform p3)
        {
            var p1Pos = p1.position;
            var p2Pos = p2.position;
            var p3Pos = p3.position;

            if (IsPointBOnRightOfLineOA(p1Pos, p2Pos, p3Pos)) { return; }

            var temp = p3;
            p3 = p2;
            p2 = temp;
        }

        List<Transform> CreateSubConvexHUll(Transform p1, Transform p3, List<Transform> points)
        {
            if (points.Count == 0) { return new List<Transform>() { p1 }; }

            var p2 = FindFurthestPointFromEdge(p1, p3, points);

            points.Remove(p2);

            RemovePointsWithinTriangle(p1.position, p2.position, p3.position, points);

            if (points.Count == 0) { return new List<Transform>() { p1, p2 }; }

            var edge_12 = new List<Transform>();
            var edge_23 = new List<Transform>();

            for (int i = points.Count - 1; i >= 0; i--)
            {
                var p = points[i];

                if (IsPointBOnRightOfLineOA(p1.position, p2.position, p.position))
                {
                    edge_12.Add(p);
                    continue;
                }

                edge_23.Add(p);
            }

            var subConvexHull_12 = CreateSubConvexHUll(p1, p2, edge_12);
            var subConvexHull_23 = CreateSubConvexHUll(p2, p3, edge_23);

            subConvexHull_12.AddRange(subConvexHull_23);

            return subConvexHull_12;
        }

        void RemovePointsWithinTriangle(Vector3 p1, Vector3 p2, Vector3 p3, List<Transform> points)
        {
            for (int i = points.Count - 1; i >= 0; i--)
            {
                var point = points[i].position;

                var cross12 = (int)Mathf.Sign(Cross(p1, p2, point));
                var cross23 = (int)Mathf.Sign(Cross(p2, p3, point));
                var cross31 = (int)Mathf.Sign(Cross(p3, p1, point));

                if (cross12 != cross23 || cross12 != cross31 || cross23 != cross31) { continue; }

                points[i].GetComponent<Renderer>().material.color = Color.blue;

                points.RemoveAt(i);
            }
        }

        float Cross(Vector3 o, Vector3 a, Vector3 b) => Vector3.Cross(a - o, b - o).y;

        bool IsPointBOnRightOfLineOA(Vector3 o, Vector3 a, Vector3 b) => Cross(o, a, b) > 0f;

        yield return null;
    }

    private List<T> QuickSort<T>(List<T> list, int left, int right, System.Func<T, T, bool> Compare)
    {
        if (left >= right) { return list; }

        var partitionIndex = Partition(list, left, right, Compare);
        QuickSort(list, left, partitionIndex - 1, Compare);
        QuickSort(list, partitionIndex + 1, right, Compare);

        return list;
    }

    private int Partition<T>(List<T> list, int left, int right, System.Func<T, T, bool> Compare)
    {
        var pivot = left;
        var index = pivot + 1;
        for (int i = index; i <= right; i++)
        {
            if (Compare(list[i], list[pivot]))
            {
                Swap(list, i, index);
                index++;
            }
        }

        Swap(list, pivot, index - 1);
        return index - 1;
    }

    private void Swap<T>(List<T> list, int i, int j)
    {
        if (i >= list.Count || i < 0) { return; }
        if (j >= list.Count || j < 0) { return; }

        var temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }

    private static void PolygonInner(List<Vector3> points)
    {
        var m = new Mesh();
        m.SetVertices(points);
        m.SetNormals(points.Select(p => Vector3.up).ToList());

        var triList = new List<int[]>();
        for (int i = 2; i < points.Count; i++)
        {
            triList.Add(new int[] { 0, i - 1, i });
        }

        m.SetTriangles(
                triList.SelectMany(tl => tl.ToList()).ToArray(),
                0);

        Gizmos.DrawMesh(m, Vector3.zero, Quaternion.identity, Vector3.one);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position, new Vector3(_generateBound.x * 2, 0.1f, _generateBound.y * 2));

        for (int i = 0; i < _ballArray.Length; i++)
        {
            Handles.Label(_ballArray[i].position, $"{i}");
        }

        if (_convexHullPoints == null) { return; }
        if (_convexHullPoints.Count < 3) { return; }

        for (int i = 0; i < _convexHullPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(_convexHullPoints[i].position, _convexHullPoints[i + 1].position);
        }

        Gizmos.DrawLine(_convexHullPoints[_convexHullPoints.Count - 1].position, _convexHullPoints[0].position);

        //PolygonInner(_convexHullPoints.Select(p => p.position).ToList());
        //var reverse = _convexHullPoints.Reverse<Transform>();
        //PolygonInner(reverse.Select(p => p.position).ToList());
    }
}