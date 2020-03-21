using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Byjus.RockSalon.Views;
using Byjus.RockSalon.Ctrls;
using System;
using Byjus.RockSalon.Verticals;
using Byjus.RockSalon.Externals;

namespace Byjus.RockSalon.Tests {

    class TestView : IGameManagerView {
        public List<GameObject> crystals;

        public void Init() {
            crystals = new List<GameObject>();
        }

        public void TearDown() {
            foreach (var crystal in crystals) {
                Debug.Log("Destroying  " + crystal);
                GameObject.Destroy(crystal);
            }
            crystals.Clear();
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

        public List<LevelData> GetAllLevels() {
            var ld = ScriptableObject.CreateInstance<LevelData>();
            ld.monsterIndex = 0;
            ld.generic = true;
            ld.totalReqt = 20;

            return new List<LevelData>() { ld };
        }

        public void InstantiateLevel(LevelInfo level) {
            Debug.Log("Instantiating " + level);
        }

        public void DestroyLevel(LevelInfo level) {
            Debug.Log("Destroying " + level);
        }
    }

    class TestVisionService : IVisionService {
        List<List<ExtInput>> outputs;

        int inputCount;

        public void Init() {
            outputs = new List<List<ExtInput>>();
            inputCount = 0;
        }

        public void SetOutputs(List<List<ExtInput>> outputs) {
            this.outputs = outputs;
        }

        public List<ExtInput> GetVisionObjects() {
            if (inputCount < outputs.Count) {
                return outputs[inputCount++];
            }

            return new List<ExtInput>();
        }
    }

    public class TestInputListener : IExtInputListener {
        public List<int> ids;

        public void OnCrystalAdded(TileType type, int id, Vector2 position) {
            ids.Add(id);
            Debug.Log("Added " + type + ", id: " + id + ", at: " + position);
        }

        public void OnCrystalMoved(TileType type, int id, Vector2 newPosition) {
            Debug.Log("Moved " + type + ", id: " + id + ", to: " + newPosition);
        }

        public void OnCrystalRemoved(TileType type, int id) {
            ids.Remove(id);
            Debug.Log("Removed: " + type + ", id: " + id);
        }

        public void OnInputEnd() {
            Debug.Log("End");
        }

        public void OnInputStart() {
            Debug.Log("Start");
        }
    }
}
