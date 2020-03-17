using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Byjus.RockSalon.Util;

namespace Byjus.RockSalon.Verticals {
    public class InputParser : MonoBehaviour {
        public IExtInputListener inputListener;

        const float POINT_COMPARE_EPSILON_PERCENT = 0.1f / 100;
        const float SAME_POINT_DIST_THRESHOLD_PERCENT = 8.0f / 100;

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

            extraOld.Sort((x, y) => (int) (Vector2.Distance(x.position, Vector2.zero) - Vector2.Distance(y.position, Vector2.zero)));
            extraNew.Sort((x, y) => (int) (Vector2.Distance(x.position, Vector2.zero) - Vector2.Distance(y.position, Vector2.zero)));

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
                    if (old.type == newO.type && EqualPosition(old.position, newO.position)) {
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

        bool EqualPosition(Vector2 point1, Vector2 point2) {
            var widthEpsilon = CameraUtil.MainWidth() * POINT_COMPARE_EPSILON_PERCENT;
            var heightEpsilon = CameraUtil.MainHeight() * POINT_COMPARE_EPSILON_PERCENT;

            return Mathf.Abs(point1.x - point2.x) < widthEpsilon &&
                Mathf.Abs(point1.y - point2.y) < heightEpsilon;
        }

        ExtInput FindClosest(ExtInput obj, List<ExtInput> targetObjs) {
            float minWidth = CameraUtil.MainWidth() * SAME_POINT_DIST_THRESHOLD_PERCENT;
            float minHeight = CameraUtil.MainHeight() * SAME_POINT_DIST_THRESHOLD_PERCENT;
            float minDist = Mathf.Sqrt(Mathf.Pow(minWidth, 2) + Mathf.Pow(minHeight, 2));
            Debug.LogError("Min Width: " + minWidth + ", Min Height: " + minHeight + " Min Dist required: " + minDist);
            ExtInput min = null;

            foreach (var old in targetObjs) {
                if (old.type != obj.type) { continue; }

                var dist = Vector2.Distance(obj.position, old.position);
                Debug.LogError("Dist between obj: " + obj + ", and old: " + old + ", dist: " + dist);
                if (dist < minDist) {
                    min = old;
                    minDist = dist;
                }
            }

            Debug.LogError("Returning " + min + " for " + obj);
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