using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using NUnit.Framework;
using Byjus.RockSalon.Verticals;
using System.IO;
using System.Collections;
using System.Collections.Generic;

#if !CC_STANDALONE

namespace Byjus.RockSalon.Tests {
    public class VisionServiceTestSuite : BaseTestSuite {
        OsmoVisionService osmoVisionService;

        [SetUp]
        public void Setup() {
            BaseInit();

            var cam = new GameObject("Camera");
            cam.AddComponent<Camera>();
            cam.tag = "MainCamera";
            BaseAddGo(cam);

            var textGo = new GameObject("JsonText");
            BaseAddGo(textGo);
            var text = textGo.AddComponent<Text>();

            var vs = new GameObject("VisionService");
            BaseAddGo(vs);
            osmoVisionService = vs.AddComponent<OsmoVisionService>();
            osmoVisionService.Init();
        }

        [TearDown]
        public void Teardown() {
            BaseTearDown();
        }

        const string FilePrefix = "VisionService";

        string ReadFile(string testId, string fileId) {
            var filesPath = Path.Combine("Assets", "Games", "container-tester-games", "RockSalon", "Tests", "TestFiles");
            var fileName = FilePrefix + testId + fileId + ".json";

            return File.ReadAllText(Path.Combine(filesPath, fileName));
        }

        [UnityTest]
        public IEnumerator TestRedCube() {
            var json1 = ReadFile("Test1", "Json1");
            var json2 = ReadFile("Test1", "Json2");
            var json3 = ReadFile("Test1", "Json3");

            osmoVisionService.DispatchEvent(json1);
            osmoVisionService.DispatchEvent(json2);
            osmoVisionService.DispatchEvent(json3);

            yield return null;

            var objs = osmoVisionService.GetVisionObjects();
            // assert what is expected out of the jsons

            Assert.AreEqual(1, objs.Count);
        }

        [UnityTest]
        public IEnumerator TestBlueRod() {
            var json1 = ReadFile("Test2", "Json1");
            var json2 = ReadFile("Test2", "Json2");
            var json3 = ReadFile("Test2", "Json3");

            osmoVisionService.DispatchEvent(json1);
            osmoVisionService.DispatchEvent(json2);
            osmoVisionService.DispatchEvent(json3);

            yield return null;

            var objs = osmoVisionService.GetVisionObjects();
            Assert.AreEqual(10, objs.Count);
        }

        [UnityTest]
        public IEnumerator TestFlicker() {
            var json1 = ReadFile("Test2", "Json1");
            var json2 = ReadFile("Test2", "Json2");
            var json3 = ReadFile("Test2", "Json3");

            osmoVisionService.DispatchEvent(json1);
            osmoVisionService.DispatchEvent(json1);
            osmoVisionService.DispatchEvent(json1);

            yield return null;

            var objs1 = osmoVisionService.GetVisionObjects();

            osmoVisionService.DispatchEvent(json2);
            osmoVisionService.DispatchEvent(json2);
            osmoVisionService.DispatchEvent(json2);

            yield return null;

            var objs2 = osmoVisionService.GetVisionObjects();

            osmoVisionService.DispatchEvent(json3);
            osmoVisionService.DispatchEvent(json3);
            osmoVisionService.DispatchEvent(json3);

            yield return null;

            var objs3 = osmoVisionService.GetVisionObjects();

            Assert.AreEqual(10, objs1.Count);
            Assert.AreEqual(10, objs2.Count);
            Assert.AreEqual(10, objs3.Count);
        }
    }
}

#endif