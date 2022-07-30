using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using NaughtyAttributes;

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

    [Button]
    private void StartGrahamScan()
    {
        StartCoroutine(GrahamScan());
    }

    private IEnumerator GrahamScan()
    {
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

            QuickSort(_ballList, 0, _ballList.Count - 1);

            _ballList.Insert(0, bottomLeftPoint);

            _ballArray = _ballList.ToArray();
        }

        bool CompareAngle(Vector3 point0, Vector3 point1)
        {
            var line0 = point0 - bottomLeftPoint.position;
            var line1 = point1 - bottomLeftPoint.position;

            var y = Vector3.Cross(line0, line1).y;

            if (y > 0f) { return true; }
            if (y == 0f && (line0.sqrMagnitude < line1.sqrMagnitude)) { return true; }

            return false;
        }

        List<Transform> QuickSort(List<Transform> list, int left, int right)
        {
            if (left >= right) { return list; }

            int partitionIndex = Partition(list, left, right);
            QuickSort(list, left, partitionIndex - 1);
            QuickSort(list, partitionIndex + 1, right);

            return list;
        }

        int Partition(List<Transform> list, int left, int right)
        {
            int pivot = left;
            int index = pivot + 1;
            for (int i = index; i <= right; i++)
            {
                if (CompareAngle(list[i].position, list[pivot].position))
                {
                    Swap(list, i, index);
                    index++;
                }
            }
            Swap(list, pivot, index - 1);
            return index - 1;
        }

        void Swap(List<Transform> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
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

        var t = _convexHullPoints.Count-1;

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
            QuickSort(_ballList, 0, _ballList.Count - 1);

            _ballArray = _ballList.ToArray();
        }

        bool Compare(Vector3 point0, Vector3 point1)
        {
            if (point0.x < point1.x) { return true; }
            if (point0.x == point1.x && point0.z < point1.z) { return true; }

            return false;
        }

        List<Transform> QuickSort(List<Transform> list, int left, int right)
        {
            if (left >= right) { return list; }

            int partitionIndex = Partition(list, left, right);
            QuickSort(list, left, partitionIndex - 1);
            QuickSort(list, partitionIndex + 1, right);

            return list;
        }

        int Partition(List<Transform> list, int left, int right)
        {
            int pivot = left;
            int index = pivot + 1;
            for (int i = index; i <= right; i++)
            {
                if (Compare(list[i].position, list[pivot].position))
                {
                    Swap(list, i, index);
                    index++;
                }
            }
            Swap(list, pivot, index - 1);
            return index - 1;
        }

        void Swap(List<Transform> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
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
            var bp0 = _convexHullPoints[0].position;
            var bp1 = _convexHullPoints[1].position;
            var tp0 = _convexHullPoints[last].position;
            var tp1 = _convexHullPoints[last - 1].position;
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
    }
}