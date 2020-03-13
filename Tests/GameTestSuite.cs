using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Byjus.RockSalon.Verticals;
using Byjus.RockSalon.Ctrls;
using Byjus.RockSalon.Views;
using System;

namespace Byjus.RockSalon.Tests {
    public class GameTestSuite {
        IGameManagerCtrl ctrl;
        IExtInputListener inputListener;
        TestView tv;

        [SetUp]
        public void Setup() {
            var gameCtrl = new GameManagerCtrl();
            tv = new TestView();
            gameCtrl.view = tv;

            gameCtrl.Init();
            tv.Init();

            ctrl = gameCtrl;
            inputListener = gameCtrl;
        }

        [TearDown]
        public void TearDown() {
            ctrl = null;
            inputListener = null;
            tv = null;
        }

        [UnityTest]
        public IEnumerator TestAddition() {
            inputListener.OnInputStart();
            inputListener.OnCrystalAdded(TileType.BLUE_ROD, 0, new Vector2(0, 0));
            inputListener.OnCrystalAdded(TileType.RED_CUBE, 0, new Vector2(1, 1));
            inputListener.OnCrystalAdded(TileType.BLUE_ROD, 0, new Vector2(2, 2));
            inputListener.OnCrystalAdded(TileType.RED_CUBE, 0, new Vector2(3, 3));
            inputListener.OnInputEnd();

            yield return null;

            Assert.AreEqual(4, tv.crystals.Count);
        }

        [UnityTest]
        public IEnumerator TestMoveCrystals() {
            inputListener.OnInputStart();
            inputListener.OnCrystalAdded(TileType.BLUE_ROD, 0, new Vector2(0, 0));
            inputListener.OnInputEnd();

            yield return null;

            inputListener.OnInputStart();
            inputListener.OnCrystalMoved(TileType.BLUE_ROD, 0, new Vector2(1, 1));
            inputListener.OnInputEnd();

            yield return null;

            Assert.AreEqual(1, tv.crystals.Count);
            Assert.AreEqual(new Vector3(1, 1), tv.crystals[0].transform.position);
        }

        [UnityTest]
        public IEnumerator TestRemoveCrystals() {
            inputListener.OnInputStart();
            inputListener.OnCrystalAdded(TileType.BLUE_ROD, 0, new Vector2(0, 0));
            inputListener.OnInputEnd();

            yield return null;

            inputListener.OnInputStart();
            inputListener.OnCrystalRemoved(TileType.BLUE_ROD, 0);
            inputListener.OnInputEnd();

            yield return null;

            Assert.AreEqual(0, tv.crystals.Count);
        }

        [UnityTest]
        public IEnumerator TestWithRedCubes() {
            inputListener.OnInputStart();
            inputListener.OnCrystalAdded(TileType.BLUE_ROD, 0, new Vector2(-4, 0));
            inputListener.OnCrystalAdded(TileType.BLUE_ROD, 1, new Vector2(-4, 0));
            inputListener.OnCrystalAdded(TileType.BLUE_ROD, 2, new Vector2(0, 3));
            inputListener.OnCrystalAdded(TileType.BLUE_ROD, 3, new Vector2(-2, -2));
            inputListener.OnInputEnd();

            yield return null;

            inputListener.OnInputStart();
            inputListener.OnCrystalAdded(TileType.RED_CUBE, 1004, new Vector2(2, 1));
            inputListener.OnCrystalAdded(TileType.RED_CUBE, 1005, new Vector2(-2, -1));
            inputListener.OnCrystalAdded(TileType.RED_CUBE, 1006, new Vector2(2, 0));
            inputListener.OnInputEnd();
        }


        [UnityTest]
        public IEnumerator TestFailCase1() {
            inputListener.OnInputStart();
            inputListener.OnCrystalAdded(TileType.BLUE_ROD, 1, new Vector2(3, -4));
            inputListener.OnCrystalAdded(TileType.BLUE_ROD, 2, new Vector2(-1, -5));
            inputListener.OnCrystalAdded(TileType.BLUE_ROD, 3, new Vector2(-2, -1));
            inputListener.OnInputEnd();

            yield return null;

            inputListener.OnInputStart();
            inputListener.OnCrystalMoved(TileType.BLUE_ROD, 1, new Vector2(4, -4));
            inputListener.OnCrystalAdded(TileType.BLUE_ROD, 4, new Vector2(4, -4));
            inputListener.OnCrystalAdded(TileType.RED_CUBE, 1, new Vector2(-4, -3));
            inputListener.OnCrystalAdded(TileType.RED_CUBE, 2, new Vector2(0, 0));
            inputListener.OnCrystalAdded(TileType.RED_CUBE, 3, new Vector2(1, 1));
            inputListener.OnCrystalAdded(TileType.RED_CUBE, 4, new Vector2(-2, -1));
            inputListener.OnCrystalRemoved(TileType.BLUE_ROD, 2);
            inputListener.OnCrystalRemoved(TileType.BLUE_ROD, 3);
            inputListener.OnInputEnd();

            yield return null;

            inputListener.OnInputStart();
            inputListener.OnCrystalAdded(TileType.BLUE_ROD, 5, new Vector2(0, 3));
            inputListener.OnCrystalMoved(TileType.RED_CUBE, 1, new Vector2(-3, -2));
            inputListener.OnCrystalMoved(TileType.RED_CUBE, 4, new Vector2(-3, -2));
            inputListener.OnCrystalAdded(TileType.BLUE_ROD, 5, new Vector2(0, 3));
        }
    }

    class TestView : IGameManagerView {
        public List<GameObject> crystals;

        public void Init() {
            crystals = new List<GameObject>();
        }

        public void CreateCrystal(CrystalType type, Vector2 position, Action<GameObject> onCreateDone) {
            var go = new GameObject(type + "");
            go.transform.position = position;
            crystals.Add(go);
            onCreateDone(go);
        }

        public void MoveCrystal(GameObject crystalGo, Vector2 newPosition, Action onMoveDone) {
            crystalGo.transform.position = newPosition;
            onMoveDone();
        }

        public void RemoveCrystal(GameObject crystalGo, Action onRemoveDone) {
            crystals.Remove(crystalGo);
            onRemoveDone();
        }
    }
}
