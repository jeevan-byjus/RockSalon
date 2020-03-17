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
            yield return null;

            var obj = new GameObject("Cam");
            var cam = obj.AddComponent<Camera>();
            cam.orthographicSize = 20;
            cam.tag = "MainCamera";

            yield return null;

            Assert.NotNull(cam);
            Assert.NotNull(Camera.main);

            var boundingBox = new BoundingBox(new List<Vector2> { new Vector2(-90, 90), new Vector2(90, 90), new Vector2(190, -200), new Vector2(-190, -200) });

            Debug.Log("Top: " + boundingBox.topWidth + ", Bottom: " + boundingBox.bottomWidth + ", height: " + boundingBox.height + "\n");

            Assert.AreEqual(new Vector2(-15, 20), boundingBox.GetScreenPoint(new Point(-90, 90)));
            Assert.AreEqual(new Vector2(15, 20), boundingBox.GetScreenPoint(new Point(90, 90)));
            Assert.AreEqual(new Vector2(15, -20), boundingBox.GetScreenPoint(new Point(190, -200)));
            Assert.AreEqual(new Vector2(-15, -20), boundingBox.GetScreenPoint(new Point(-190, -200)));
        }
    }
}
