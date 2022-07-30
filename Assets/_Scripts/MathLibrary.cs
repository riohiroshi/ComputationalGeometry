using UnityEngine;

public class MathLibrary
{
    public static bool IsOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        var offset = 0.001f;
        return Vector3.Dot((lineStart - point).normalized, (lineEnd - point).normalized) + 1 < offset;
    }

    //public static bool IsOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    //{
    //    var offset = 0.1f;
    //    return Vector3.Cross(lineStart - point, lineEnd - point).magnitude < offset;
    //}

    public static float IsOnRight(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        return Vector3.Cross(lineEnd - lineStart, point - lineStart).y;
    }

    public static bool IsOnSameSide(Vector3 lineStart, Vector3 lineEnd, Vector3 pointA, Vector3 pointB)
    {
        return IsOnRight(lineStart, lineEnd, pointA) * IsOnRight(lineStart, lineEnd, pointB) > 0f;
    }

    public static bool IsParallel(Vector3 line0Start, Vector3 line0End, Vector3 line1Start, Vector3 line1End)
    {
        return Vector3.Cross(line0End - line0Start, line1End - line1Start).y == 0f;
    }

    public static bool IsPerpendicular(Vector3 line0Start, Vector3 line0End, Vector3 line1Start, Vector3 line1End)
    {
        return Vector3.Dot(line0End - line0Start, line1End - line1Start) == 0f;
    }

    public static bool IsIntersect(Vector3 line0Start, Vector3 line0End, Vector3 line1Start, Vector3 line1End)
    {
        if (IsOnSameSide(line0Start, line0End, line1Start, line1End)) { return false; }
        if (IsOnSameSide(line1Start, line1End, line0Start, line0End)) { return false; }

        return true;
    }
}