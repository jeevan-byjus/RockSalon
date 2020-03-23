using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using Byjus.RockSalon.Verticals;
using Byjus.RockSalon.Ctrls;
using Byjus.RockSalon.Views;
using Byjus.RockSalon.Util;
using System.IO;

namespace Byjus.RockSalon.Tests {
    public class IntegrationTestSuite : BaseTestSuite {
        OsmoVisionService osmoVisionService;
        TestView testView;

        const string FilePrefix = "VisionService";

        string ReadFile(string testId, string fileId) {
            var filesPath = Path.Combine("Assets", "Games", "container-tester-games", "RockSalon", "Tests", "TestFiles");
            var fileName = FilePrefix + testId + fileId + ".json";

            return File.ReadAllText(Path.Combine(filesPath, fileName));
        }

        [SetUp]
        public void Setup() {
            BaseInit();

            var go = new GameObject("Camera");
            BaseAddGo(go);
            go.AddComponent<Camera>();
            go.tag = "MainCamera";

            go = new GameObject("JsonText");
            BaseAddGo(go);
            var text = go.AddComponent<Text>();

            go = new GameObject("VisionService");
            BaseAddGo(go);
            osmoVisionService = go.AddComponent<OsmoVisionService>();
            osmoVisionService.jsonText = text;
            osmoVisionService.Init();

            Factory.SetVisionService(osmoVisionService);

            testView = new TestView();
            testView.Init();

            var gameCtrl = new GameManagerCtrl();
            gameCtrl.view = testView;
            gameCtrl.Init();

            go = new GameObject("InputParser");
            BaseAddGo(go);
            var inputParser = go.AddComponent<InputParser>();
            inputParser.inputListener = gameCtrl;
            inputParser.Init();
        }

        [TearDown]
        public void Teardown() {
            BaseTearDown();
            testView.TearDown();
        }

        [UnityTest]
        public IEnumerator FullTest() {
            var json1 = ReadFile("Test2", "Json1");
            var json2 = ReadFile("Test2", "Json2");
            var json4 = ReadFile("Test2", "Json4");

            osmoVisionService.DispatchEvent(json1);
            osmoVisionService.DispatchEvent(json1);
            osmoVisionService.DispatchEvent(json1);
            yield return new WaitForSeconds(Constants.INPUT_DELAY);

            Assert.AreEqual(10, testView.numCreate);
            Assert.AreEqual(0, testView.numMove);
            Assert.AreEqual(0, testView.numRemove);

            osmoVisionService.DispatchEvent(json2);
            osmoVisionService.DispatchEvent(json2);
            osmoVisionService.DispatchEvent(json2);
            yield return new WaitForSeconds(Constants.INPUT_DELAY);

            Assert.AreEqual(0, testView.numCreate - 10); // 10 already added before
            Assert.AreEqual(0, testView.numMove);
            Assert.AreEqual(0, testView.numRemove);

            // json4 has only 9 cubes, it should not consider this input
            osmoVisionService.DispatchEvent(json4);
            osmoVisionService.DispatchEvent(json4);
            osmoVisionService.DispatchEvent(json4);
            yield return new WaitForSeconds(Constants.INPUT_DELAY);

            Assert.AreEqual(0, testView.numCreate - 10); // 10 already added before
            Assert.AreEqual(0, testView.numMove);
            Assert.AreEqual(0, testView.numRemove);

        }
    }
}