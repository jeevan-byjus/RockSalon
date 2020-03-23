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
            try {
                var objs = visionService.GetVisionObjects();
                inputListener.OnInputStart();
                OnInput(objs);
                inputListener.OnInputEnd();

                StartCoroutine(ListenForInput());

            } catch (System.Exception e) {
                Debug.LogError("Some error occured " + e.Message);
            }
        }

        void OnInput(List<ExtInput> objs) {
            Debug.Log("Before validating: " + objs.Count);
            ValidateInput(objs);
            Debug.Log("After validating: " + objs.Count);
            Segregate(objs, out List<ExtInput> extraOld, out List<ExtInput> extraNew);

            foreach (var newO in extraNew) {
                var min = FindClosest(newO, extraOld);

                if (min == null) {
                    // added new
                    int id = FindNextAvailableId(newO.type);
                    inputListener.OnCrystalAdded(newO.type, id, newO.position);
                    newO.id = id;
                    currentObjects.Add(newO);

                } else {
                    // moved something
                    var currObj = currentObjects.Find(x => x.id == min.id);
                    currObj.position = newO.position;
                    inputListener.OnCrystalMoved(min.type, min.id, min.position);
                    extraOld.Remove(min);
                }
            }

            foreach (var old in extraOld) {
                inputListener.OnCrystalRemoved(old.type, old.id);
                currentObjects.Remove(old);
            }

        }

        // have to improve this logic
        // but finding out which cubes belong to the same rod is a tough problem
        // group together blue cubes with the same slope
        // there are a lot of edge cases though
        void ValidateInput(List<ExtInput> objs) {
            int numBlues = 0;
            List<int> indices = new List<int>();

            for (int i = 0; i < objs.Count; i++) {
                if (objs[i].type == TileType.BLUE_ROD) {
                    indices.Add(i);
                    numBlues++;
                }
            }

            if (numBlues % 10 != 0) {
                Debug.Log("Validating input found non-multiple blues " + numBlues);
                // considering only full blue cubes, so don't consider the blue cubes in this input
                // so remove all new blue cubes
                for (int i = indices.Count - 1; i >= 0; i--) {
                    objs.RemoveAt(indices[i]);
                }
                Debug.Log("After removing indices count: " + objs.Count);
                // and add current blue cubes denoting no change in input
                objs.AddRange(currentObjects.FindAll(x => x.type == TileType.BLUE_ROD));
                Debug.Log("After adding back blues count: " + objs.Count);
            }
        }

        void Segregate(List<ExtInput> newObjs, out List<ExtInput> extraOld, out List<ExtInput> extraNew) {
            extraOld = new List<ExtInput>();
            extraNew = new List<ExtInput>();
            extraNew.AddRange(newObjs);

            foreach (var old in currentObjects) {
                bool found = false;
                foreach (var newO in extraNew) {
                    //Debug.Log("Comparing old: " + old + ", new: " + newO);
                    if (old.type == newO.type && GenUtil.EqualPositionSw(old.position, newO.position)) {
                        //Debug.Log("Found equal for old " + old + ", new " + newO);
                        old.position = newO.position;
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
            var camDimen = CameraUtil.MainDimens();
            float minWidth = GenUtil.Rounded(camDimen.x * Constants.SW_SAME_POINT_MOVED_DIFF_PERCENT);
            float minHeight = GenUtil.Rounded(camDimen.y * Constants.SW_SAME_POINT_MOVED_DIFF_PERCENT);
            float minDist = float.MaxValue;
            ExtInput min = null;

            foreach (var old in targetObjs) {
                if (old.type != obj.type) { continue; }

                var diffX = GenUtil.Rounded(Mathf.Abs(obj.position.x - old.position.x));
                var diffY = GenUtil.Rounded(Mathf.Abs(obj.position.y - old.position.y));

                if (diffX > minWidth || diffY > minHeight) {
                    continue;
                }

                var dist = Vector2.Distance(obj.position, old.position);
                if (dist < minDist) {
                    min = old;
                    minDist = dist;
                }
            }

            //Debug.Log("Returning closest: " + min + " for " + obj);
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