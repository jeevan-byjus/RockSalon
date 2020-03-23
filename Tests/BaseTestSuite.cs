using System;
using System.Collections.Generic;
using UnityEngine;

namespace Byjus.RockSalon.Tests {
    public class BaseTestSuite {
        List<GameObject> setupGOs;

        protected void BaseInit() {
            setupGOs = new List<GameObject>();
        }

        protected void BaseAddGo(GameObject go) {
            setupGOs.Add(go);
        }

        protected void BaseTearDown() {
            foreach (var go in setupGOs) {
                GameObject.Destroy(go);
            }
            setupGOs.Clear();
        }

        protected void CreateMainCamera() {
            var cam = new GameObject("Camera");
            cam.AddComponent<Camera>();
            cam.tag = "MainCamera";
            BaseAddGo(cam);
        }
    }
}