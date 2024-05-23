using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SwiftNPCs.Core.Pathing
{
    public class Path
    {
        public Path() { }

        public Path(NavMeshPath path)
        {
            Waypoints = [.. path.corners];
        }

        public readonly List<Vector3> Waypoints = [];

        public float WaypointRadius = 0.25f;

        public void AddWaypoint(Vector3 point) => Waypoints.Add(point);

        public void RemoveWaypoint(int index) => Waypoints.RemoveAt(index);

        public void RemoveWaypoint(Vector3 position) => Waypoints.Remove(position);

        public void ClearWaypoints() => Waypoints.Clear();

        public void ChangeWaypoint(int index, Vector3 position)
        {
            if (index >= 0 && index < Waypoints.Count)
                Waypoints[index] = position;
        }

        public bool TryGetWaypoint(int index, out Vector3 waypoint)
        {
            if (index >= 0 && index < Waypoints.Count)
            {
                waypoint = Waypoints[index];
                return true;
            }
            waypoint = Vector3.zero;
            return false;
        }

        public bool TryGetNextDirection(int current, out Vector3 direction, out Vector3 next)
        {
            if (!TryGetWaypoint(current, out Vector3 curr) || !TryGetWaypoint(current + 1, out next))
            {
                direction = Vector3.zero;
                next = Vector3.zero;
                return false;
            }

            direction = (next - curr).normalized;
            return true;
        }

        public bool TryGetNextDirection(int current, out Vector3 direction)
        {
            return TryGetNextDirection(current, out direction, out _);
        }

        public bool TryGetDistance(Vector3 currentPos, int current, out float distance)
        {
            if (!TryGetWaypoint(current, out Vector3 waypoint))
            {
                distance = 0f;
                return false;
            }

            distance = Vector3.Distance(currentPos, waypoint);
            return true;
        }

        public int GetNearestIndex(Vector3 currentPos)
        {
            int index = -1;
            float smallestDistance = Mathf.Infinity;
            for (int i = 0; i < Waypoints.Count; i++)
            {
                if (TryGetDistance(currentPos, i, out float dist) && dist < smallestDistance)
                {
                    smallestDistance = dist;
                    index = i;
                }
            }

            return index;
        }

        public override string ToString()
        {
            return "Start Position: " + (Waypoints.Count > 0 ? Waypoints[0] : "None");
        }
    }
}
