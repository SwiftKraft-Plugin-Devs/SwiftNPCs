using System.Collections.Generic;
using UnityEngine;

namespace SwiftNPCs.Core.Pathing
{
    public class Path
    {
        public readonly List<Vector3> Waypoints = new();

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

        public bool TryGetNextDirection(int current, out Vector3 direction)
        {
            if (!TryGetWaypoint(current, out Vector3 curr) || !TryGetWaypoint(current + 1, out Vector3 next))
            {
                direction = Vector3.zero;
                return false;
            }

            direction = (next - curr).normalized;
            return true;
        }

        public bool TryGetNextDirection(Vector3 currentPos, int current, out Vector3 direction, out float distance)
        {
            TryGetDistance(currentPos, current + 1, out distance);
            return TryGetNextDirection(current, out direction);
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
    }
}
