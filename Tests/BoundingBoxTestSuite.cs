using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Byjus.RockSalon.Verticals;
using Byjus.RockSalon.Util;

namespace Byjus.RockSalon.Tests {
    public class BoundingBoxTestSuite {
        [UnityTest]
        public IEnumerator TestWithCamera() {
            var camDimens = new Vector2(30, 40);

            yield return null;

            var boundingBox = new BoundingBox(new List<Vector2> { new Vector2(-90, 90), new Vector2(100, 90), new Vector2(190, -200), new Vector2(-150, -200) });

            Debug.Log("Top: " + boundingBox.topWidth + ", Bottom: " + boundingBox.bottomWidth + ", height: " + boundingBox.height + "\n");

            Assert.AreEqual(new Vector2(-15, 20), boundingBox.GetScreenPoint(camDimens, new Point(-90, 90)));
            Assert.AreEqual(new Vector2(15, 20), boundingBox.GetScreenPoint(camDimens, new Point(100, 90)));
            Assert.AreEqual(new Vector2(15, -20), boundingBox.GetScreenPoint(camDimens, new Point(190, -200)));
            Assert.AreEqual(new Vector2(-15, -20), boundingBox.GetScreenPoint(camDimens, new Point(-150, -200)));
        }
    }
}
