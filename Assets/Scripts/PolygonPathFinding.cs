using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonPathFinding : MonoBehaviour
{
    public const float epsilon = 0.1f;
    private PolygonCollider2D[] colliders;
    public PathData path = new PathData();

    // Start is called before the first frame update
    void Start()
    {
        path.points = new List<PointData>();
        colliders = GetComponentsInChildren<PolygonCollider2D>();
        foreach (var collider in colliders)
        {
            foreach (var point in collider.points)
            {
                PointData pointData = new PointData(point + new Vector2(collider.transform.position.x, collider.transform.position.y));
                path.points.Add(pointData);

            }
        }
        int index = 0;

        foreach (var collider in colliders)
        {
            for (int i = 0; i < collider.points.Length; i ++)
            {
                var crIndex = index + i;
                var nextIndex = index + (i + 1) % collider.points.Length;
                path.points[crIndex].neighbours.Add(nextIndex);
                path.points[nextIndex].neighbours.Add(crIndex);
                //Debug.DrawLine(path.points[crIndex].point, path.points[nextIndex].point, Color.white, 1000);
            }
            index += collider.points.Length;
        }
        for (int i = 0; i < path.points.Count - 1; i++)
        {
            for (int j = i + 1; j < path.points.Count; j++)
            {
                var inLineOfsight = InLineOfSight(path.points[i].point, path.points[j].point);
                if(inLineOfsight)
                {
                    path.points[i].neighbours.Add(j);
                    path.points[j].neighbours.Add(i);
                    //Debug.DrawLine(path.points[i].point, path.points[j].point, Color.white, 1000);
                }
                //else
                //    Debug.DrawLine(points[i], points[j], Color.red, 100);
            }
        }
    }

    public bool InsideColliders(Vector2 point)
    {
        bool inside = false;
        foreach (var collider in colliders)
        {
            if (collider.OverlapPoint(point))
            {
                inside = true;
                for(int i = 0; i < collider.points.Length; i ++)
                {
                    if(collider.points[i] + new Vector2(collider.transform.position.x, collider.transform.position.y) == point)
                    {
                        inside = false;
                        break;
                    }
                }
                break;
            }
        }
        return inside;
    }
    public bool InLineOfSight(Vector2 start, Vector2 end)
    {
        if (start == end) return true;
        Vector3 point1 = start + (end - start).normalized * 0.1f;
        Vector3 point2 = end + (start - end).normalized * 0.1f;
        if (InsideColliders(point1) || InsideColliders(point2))
        {
            //Debug.DrawLine(point1, point2, Color.red);
            return false;
        }
        
        var hitted = Physics2D.Linecast(point1, point2, 1 << this.gameObject.layer) || Physics2D.Linecast(point2, point1, 1 << this.gameObject.layer);
        //foreach(var hitPoint in hitInfos)
        //{
        //    if (points.Contains(hitPoint.point)) count--;
        //}
        
        return !hitted;
    }

    public List<Vector2> FindPath(Vector2 start, Vector2 end)
    {
        if (InsideColliders(start) || InsideColliders(end)) return null;
        if (InLineOfSight(start, end))
        {
            return new List<Vector2> { start, end };
        }
        this.path.points.Add(new PointData(start));
        int startPointIndex = this.path.points.Count - 1;
        this.path.points.Add(new PointData(end));
        int endPointIndex = this.path.points.Count - 1;
        for (int i = 0; i < path.points.Count; i++)
        {
            if (InLineOfSight(start, path.points[i].point))
            {
                path.points[i].neighbours.Add(startPointIndex);
                path.points[startPointIndex].neighbours.Add(i);
                //DrawLine(i, startPointIndex);
            }
            if (InLineOfSight(end, path.points[i].point))
            {
                path.points[i].neighbours.Add(endPointIndex);
                path.points[endPointIndex].neighbours.Add(i);
                //DrawLine(i, endPointIndex);
            }
        }
        var pathIndexs = path.FindPath(startPointIndex, endPointIndex);
        List<Vector2> points = null;
        if (pathIndexs != null)
        {
            points = new List<Vector2>();
            foreach (var index in pathIndexs)
            {
                points.Add(path.points[index].point);
            }
        } 
        for(int i = 0; i <2; i ++) {
            this.path.points.RemoveAt(this.path.points.Count - 1);
        }
        for (int i = 0; i < path.points.Count; i++)
        {
            this.path.points[i].neighbours.Remove(startPointIndex);
            this.path.points[i].neighbours.Remove(endPointIndex);
        }
        
        return points;
    }

    private void DrawLine(int startIndex, int endIndex)
    {
        Debug.DrawLine(path.points[startIndex].point, path.points[endIndex].point, Color.white);

    }

    // Update is called once per frame
    void Update()
    {
        
    }   

    //public static bool LineSegmentsCross(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    //{
    //    float denominator = ((b.x - a.x) * (d.y - c.y)) - ((b.y - a.y) * (d.x - c.x));

    //    if (denominator == 0)
    //    {
    //        return false;
    //    }

    //    float numerator1 = ((a.y - c.y) * (d.x - c.x)) - ((a.x - c.x) * (d.y - c.y));

    //    float numerator2 = ((a.y - c.y) * (b.x - a.x)) - ((a.x - c.x) * (b.y - a.y));

    //    if (numerator1 == 0 || numerator2 == 0)
    //    {
    //        return false;
    //    }

    //    float r = numerator1 / denominator;
    //    float s = numerator2 / denominator;

    //    return (r > 0 && r < 1) && (s > 0 && s < 1);
    //}

    //public static bool IsVertexConcave(IList<Vector2> vertices, int vertex)
    //{
    //    Vector2 current = vertices[vertex];
    //    Vector2 next = vertices[(vertex + 1) % vertices.Count];
    //    Vector2 previous = vertices[vertex == 0 ? vertices.Count - 1 : vertex - 1];

    //    Vector2 left = new Vector2(current.x - previous.x, current.y - previous.y);
    //    Vector2 right = new Vector2(next.x - current.x, next.y - current.y);

    //    float cross = (left.x * right.y) - (left.y * right.x);

    //    return cross < 0;
    //}
    //public static bool Inside(IList<Vector2> polygon, Vector2 position, bool toleranceOnOutside = true)
    //{
    //    Vector2 point = position;

    //    const float epsilon = 0.5f;

    //    bool inside = false;

    //    // Must have 3 or more edges
    //    if (polygon.Count < 3) return false;

    //    Vector2 oldPoint = polygon[polygon.Count - 1];
    //    float oldSqDist = Vector2.Distance(oldPoint, point);

    //    for (int i = 0; i < polygon.Count; i++)
    //    {
    //        Vector2 newPoint = polygon[i];
    //        float newSqDist = Vector2.Distance(newPoint, point);

    //        if (oldSqDist + newSqDist + 2.0f * System.Math.Sqrt(oldSqDist * newSqDist) - Vector2.Distance(newPoint, oldPoint) < epsilon)
    //            return toleranceOnOutside;

    //        Vector2 left;
    //        Vector2 right;
    //        if (newPoint.x > oldPoint.x)
    //        {
    //            left = oldPoint;
    //            right = newPoint;
    //        }
    //        else
    //        {
    //            left = newPoint;
    //            right = oldPoint;
    //        }

    //        if (left.x < point.x && point.x <= right.x && (point.y - left.y) * (right.x - left.x) < (right.y - left.y) * (point.x - left.x))
    //            inside = !inside;

    //        oldPoint = newPoint;
    //        oldSqDist = newSqDist;
    //    }

    //    return inside;
    //}

    //bool InLineOfSight(IList<Vector2> polygon, Vector2 start, Vector2 end)
    //{
    //    // Not in LOS if any of the ends is outside the polygon
    //    if (!Inside(polygon, start) || !Inside(polygon, end)) return false;

    //    // In LOS if it's the same start and end location
    //    if (Vector2.Distance(start, end) < epsilon) return true;

    //    // Not in LOS if any edge is intersected by the start-end line segment
    //    int n = polygon.Count;
    //    for (int i = 0; i < n; i++)
    //        if (LineSegmentsCross(start, end, polygon[i], polygon[(i + 1) % n]))
    //            return false;

    //    // Finally the middle point in the segment determines if in LOS or not
    //    return Inside(polygon, (start + end) / 2f);
    //}

    [Serializable]
    public class PointData
    {
        public Vector2 point;
        public List<int> neighbours = new List<int>();
        public PointData()
        {
        }
        public PointData(Vector3 point)
        {
            this.point = point;
        }
    }
    [Serializable]
    public class PathData
    {
        public List<PointData> points;



        public List<int> FindPath(int from, int to)
        {
            SortedList<double, List<int>> paths = new SortedList<double, List<int>>();
            paths.Add(0, new List<int>() { from });
            int i = 0;

            while (paths.Count > 0)
            {
                var path = paths.Values[0];
                paths.RemoveAt(0);
                var lastPointIndex = path[path.Count - 1];
                if (lastPointIndex == to)
                {
                    return path;
                }

                PointData lastPoint = this.points[lastPointIndex];
                var crDistance = this.GetPathDistance(path);
                foreach (int neiPointIndex in lastPoint.neighbours)
                {
                    if (!path.Contains(neiPointIndex))
                    {
                        double distance = crDistance + GetDistance(lastPointIndex, neiPointIndex) + GetDistance(neiPointIndex, to);
                        while (paths.ContainsKey(distance))
                        {
                            distance += 0.000001;
                        }
                        var newPath = new List<int>(path);
                        newPath.Add(neiPointIndex);
                        paths.Add(distance, newPath);
                        //Debug.Log(distance + " "  + JsonMapper.ToJson(newPath) );
                    }
                }

            }
            return null;
        }

        private int GetClosestNodeIndex(Vector3 point)
        {
            double minDistance = 1000;
            int index = 0;
            for (int i = 0; i < this.points.Count; i++)
            {
                double dist = Vector3.Distance(this.GetPointAt(i), point);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    index = i;
                }
            }
            return index;
        }
        private Vector3 GetClosestPointInPath(Vector3 point, out int index, out int index2)
        {
            Vector3 p = point;
            double minDistance = 1000;
            index = -1;
            index2 = -1;
            for (int i = 0; i < this.points.Count - 1; i++)
            {
                for (int j = 0; j < this.points[i].neighbours.Count; j++)
                {

                    var closestPoint = GetClosestPointOnLine(GetPointAt(i), GetPointAt(this.points[i].neighbours[j]), point);

                    double dist = Vector3.Distance(closestPoint, point);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        p = closestPoint;
                        index = i;
                        index2 = this.points[i].neighbours[j];
                    }
                }
            }
            return p;
        }
        public bool CheckLineIntersection(Vector3 start, Vector3 end)
        {
            for (int i = 0; i < this.points.Count - 1; i++)
            {
                for (int j = 0; j < this.points[i].neighbours.Count; j++)
                {

                    if (LineIntersection(GetPointAt(i), GetPointAt(this.points[i].neighbours[j]), start, end)) return true;
                }
            }
            return false;
        }
        private double GetPathDistance(List<int> path)
        {
            double distance = 0;
            for (int i = 1; i < path.Count; i++)
            {
                distance += Vector3.Distance(this.points[path[i - 1]].point, this.points[path[i]].point);
            }
            return distance;

        }
        private double GetDistance(int from, int to)
        {
            return Vector3.Distance(this.points[from].point, this.points[to].point);
        }
        public Vector3 GetPointAt(int index)
        {
            return this.points[index].point;
        }
        public Vector3 GetClosestPointOnLine(Vector3 start, Vector3 end, Vector3 p)
        {
            float length = (start - end).sqrMagnitude;
            if (length == 0.0)
            {
                return start;
            }
            Vector3 v = end - start;
            float param = Vector3.Dot((p - start), v) / length;
            return (param < 0.0) ? start : (param > 1.0) ? end : (start + param * v);
        }
        private bool LineIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {

            float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num/*,offset*/;

            float x1lo, x1hi, y1lo, y1hi;



            Ax = p2.x - p1.x;

            Bx = p3.x - p4.x;



            // X bound box test/

            if (Ax < 0)
            {

                x1lo = p2.x; x1hi = p1.x;

            }
            else
            {

                x1hi = p2.x; x1lo = p1.x;

            }



            if (Bx > 0)
            {

                if (x1hi < p4.x || p3.x < x1lo) return false;

            }
            else
            {

                if (x1hi < p3.x || p4.x < x1lo) return false;

            }



            Ay = p2.z - p1.z;

            By = p3.z - p4.z;



            // Y bound box test//

            if (Ay < 0)
            {

                y1lo = p2.z; y1hi = p1.z;

            }
            else
            {

                y1hi = p2.z; y1lo = p1.z;

            }



            if (By > 0)
            {

                if (y1hi < p4.z || p3.z < y1lo) return false;

            }
            else
            {

                if (y1hi < p3.z || p4.z < y1lo) return false;

            }



            Cx = p1.x - p3.x;

            Cy = p1.z - p3.z;

            d = By * Cx - Bx * Cy;  // alpha numerator//

            f = Ay * Bx - Ax * By;  // both denominator//



            // alpha tests//

            if (f > 0)
            {

                if (d < 0 || d > f) return false;

            }
            else
            {

                if (d > 0 || d < f) return false;

            }



            e = Ax * Cy - Ay * Cx;  // beta numerator//



            // beta tests //

            if (f > 0)
            {

                if (e < 0 || e > f) return false;

            }
            else
            {

                if (e > 0 || e < f) return false;

            }



            // check if they are parallel

            if (f == 0) return false;

            // compute intersection coordinates //
            Vector3 intersection;
            num = d * Ax; // numerator //

            //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;   // round direction //

            //    intersection.x = p1.x + (num+offset) / f;
            intersection.x = p1.x + num / f;



            num = d * Ay;

            //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;

            //    intersection.y = p1.z + (num+offset) / f;
            intersection.z = p1.z + num / f;

            intersection.y = p1.y;

            return true;
        }

        public void AddPoint(Vector3 point, int index, int index2)
        {
            PointData p = new PointData();
            p.point = point;
            int pIndex = points.Count;
            p.neighbours = new List<int>();

            this.points[index].neighbours.Add(pIndex);
            this.points[index2].neighbours.Add(pIndex);
            p.neighbours.Add(index);
            p.neighbours.Add(index2);
            this.points.Add(p);
        }
        public void RemovePoint(int index, int index2)
        {
            int pIndex = points.Count - 1;
            this.points[index].neighbours.Remove(pIndex);
            this.points[index2].neighbours.Remove(pIndex);
            this.points.RemoveAt(pIndex);
        }
    }
}
