using System;
using System.Collections.Generic;
using UnityEngine;

namespace Byjus.RockSalon.Util {
    public class PointHash {
        Comparer<Vector2> vecComparable;
        List<Vector2> points;

        public PointHash(Comparer<Vector2> vecComparable) {
            this.vecComparable = vecComparable;
            points = new List<Vector2>();
        }

        public void AddPoint(Vector2 point) {
            points.Add(point);
        }

        public List<Vector2> GetEqualPoints(Vector2 point) {
            var ret = new List<Vector2>();
            foreach (var p in points) {
                if (vecComparable.Compare(p, point) == 0) {
                    ret.Add(p);
                }
            }

            return ret;
        }
    }
}