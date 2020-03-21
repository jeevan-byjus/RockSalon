using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Byjus.RockSalon.Verticals;
using Byjus.RockSalon.Ctrls;
using Byjus.RockSalon.Views;
using Byjus.RockSalon.Util;
using System;

namespace Byjus.RockSalon.Tests {
    public class InputParserTestSuite : BaseTestSuite {
        TestVisionService vs;
        InputParser ip;
        TestInputListener il;

        [SetUp]
        public void TestSetup() {
            BaseInit();

            vs = new TestVisionService();
            Factory.SetVisionService(vs);

            il = new TestInputListener();
            il.ids = new List<int>();
            var go = new GameObject("TestInputParser");
            BaseAddGo(go);
            ip = go.AddComponent<InputParser>();
            ip.inputListener = il;
        }

        [TearDown]
        public void Teardown() {
            BaseTearDown();
        }

        [UnityTest]
        public IEnumerator TestBothCubesScenario() {
            var cam = new GameObject("Cam");
            cam.AddComponent<Camera>();
            cam.tag = "MainCamera";

            yield return null;

            Assert.NotNull(Camera.main, null);

            var outputs = new List<List<ExtInput>>();

            var input1 = new List<ExtInput> {
                new ExtInput(TileType.BLUE_ROD, 0, new Vector2(3, -4)),
                new ExtInput(TileType.BLUE_ROD, 1, new Vector2(-1, -5)),
                new ExtInput(TileType.BLUE_ROD, 2, new Vector2(-2, -1))
            };

            var input2 = new List<ExtInput> {
                new ExtInput(TileType.BLUE_ROD, 0, new Vector2(4, -4)),
                new ExtInput(TileType.BLUE_ROD, 1, new Vector2(4, -4)),
                new ExtInput(TileType.RED_CUBE, 1002, new Vector2(-4, -3)),
                new ExtInput(TileType.RED_CUBE, 1003, new Vector2(0, 0)),
                new ExtInput(TileType.RED_CUBE, 1004, new Vector2(1, 1)),
                new ExtInput(TileType.RED_CUBE, 1005, new Vector2(-2, -1)),
            };

            var input3 = new List<ExtInput>() {
                new ExtInput(TileType.BLUE_ROD, 0, new Vector2(-2, -1)),
                new ExtInput(TileType.BLUE_ROD, 1, new Vector2(0, 3)),
                new ExtInput(TileType.RED_CUBE, 1002, new Vector2(-3, -2)),
                new ExtInput(TileType.RED_CUBE, 1003, new Vector2(-3, -2)),
            };

            outputs.Add(input1);
            outputs.Add(input2);
            outputs.Add(input3);

            vs.SetOutputs(outputs);

            ip.Init();

            yield return new WaitForSeconds(outputs.Count * Constants.INPUT_DELAY + 1);
        }
    }
}
