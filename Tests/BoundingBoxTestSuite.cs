using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Byjus.RockSalon.Verticals;
using Byjus.RockSalon.Util;

namespace Byjus.RockSalon.Tests {
    public class BoundingBoxTestSuite : BaseTestSuite {
        [Test]
        public void TestWithCamera() {
            var camDimens = new Vector2(30, 40);

            var boundingBox = new BoundingBox(new List<Vector2> { new Vector2(-100, 90), new Vector2(100, 90), new Vector2(100, -200), new Vector2(-100, -200) });

            Debug.Log("Top: " + boundingBox.topWidth + ", Bottom: " + boundingBox.bottomWidth + ", height: " + boundingBox.height + "\n");

            Assert.AreEqual(boundingBox.topWidth, boundingBox.bottomWidth);
            Assert.AreEqual(new Vector2(-15, 20), boundingBox.GetScreenPoint(camDimens, new Point(-100, 90)));
            Assert.AreEqual(new Vector2(15, 20), boundingBox.GetScreenPoint(camDimens, new Point(100, 90)));
            Assert.AreEqual(new Vector2(15, -20), boundingBox.GetScreenPoint(camDimens, new Point(100, -200)));
            Assert.AreEqual(new Vector2(-15, -20), boundingBox.GetScreenPoint(camDimens, new Point(-100, -200)));
            Assert.AreEqual(new Vector2(0, 0), boundingBox.GetScreenPoint(camDimens, new Point(0, -55)));

            var top = boundingBox.GetScreenPoint(camDimens, new Point(-90, 80));
            var bottom = boundingBox.GetScreenPoint(camDimens, new Point(-90, -100));
            Assert.AreEqual(top.x, bottom.x);
        }
    }
}
