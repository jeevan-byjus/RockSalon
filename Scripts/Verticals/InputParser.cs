using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Byjus.RockSalon.Util;

namespace Byjus.RockSalon.Verticals {
    public class InputParser : MonoBehaviour {
        public IExtInputListener inputListener;

        IVisionService visionService;

        List<ExtInput> currentObjects;

        int inputCount;

        public void Init() {
            visionService = Factory.GetVisionService();
            currentObjects = new List<ExtInput>();
            inputCount = 0;

            StartCoroutine(ListenForInput());
        }

        IEnumerator ListenForInput() {
            yield return new WaitForSeconds(Constants.INPUT_DELAY);

            inputCount++;
            var objs = visionService.GetVisionObjects();
            inputListener.OnInputStart();
            OnInput(objs);
            inputListener.OnInputEnd();

            StartCoroutine(ListenForInput());
        }

        void OnInput(List<ExtInput> objs) {
            Segregate(objs, out List<ExtInput> extraOld, out List<ExtInput> extraNew);

            foreach (var newO in extraNew) {
                // find closest old object (threshold 3 dist), indicating a move
                var min = FindClosest(newO, extraOld);

                if (min == null) {
                    // added new
                    int id = FindNextAvailableId(newO.type);
                    inputListener.OnCrystalAdded(newO.type, id, newO.position);
                    newO.id = id;
                    currentObjects.Add(newO);

                } else {
                    // moved something
                    newO.id = min.id;
                    inputListener.OnCrystalMoved(newO.type, newO.id, newO.position);
                    extraOld.Remove(min);
                }
            }

            foreach (var old in extraOld) {
                inputListener.OnCrystalRemoved(old.type, old.id);
                currentObjects.Remove(old);
            }

        }

        void Segregate(List<ExtInput> newObjs, out List<ExtInput> extraOld, out List<ExtInput> extraNew) {
            extraOld = new List<ExtInput>();
            extraNew = new List<ExtInput>();
            extraNew.AddRange(newObjs);

            foreach (var old in currentObjects) {
                bool found = false;
                foreach (var newO in extraNew) {
                    if (old.type == newO.type && old.position == newO.position) {
                        found = true;
                        extraNew.Remove(newO);
                        break;
                    }
                }

                if (!found) {
                    extraOld.Add(old);
                }
            }
        }

        ExtInput FindClosest(ExtInput obj, List<ExtInput> targetObjs) {
            float minDist = 3.0f;
            ExtInput min = null;

            foreach (var old in targetObjs) {
                if (old.type != obj.type) { continue; }

                var dist = Vector2.Distance(obj.position, old.position);
                if (dist < minDist) {
                    min = old;
                    minDist = dist;
                }
            }

            return min;
        }

        int FindNextAvailableId(TileType tileType) {
            int currMax = 0;
            foreach (var tile in currentObjects) {
                if (tile.type == tileType && tile.id > currMax) {
                    currMax = tile.id;
                }
            }

            return currMax + 1;
        }
    }

    public interface IExtInputListener {
        void OnInputStart();
        void OnCrystalAdded(TileType type, int id, Vector2 position);
        void OnCrystalRemoved(TileType type, int id);
        void OnCrystalMoved(TileType type, int id, Vector2 newPosition);
        void OnInputEnd();
    }

}