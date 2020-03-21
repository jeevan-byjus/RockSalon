using UnityEngine;
using System;
using System.Collections.Generic;
using Byjus.RockSalon.Util;
using UnityEngine.UI;

#if !CC_STANDALONE
using Osmo.SDK.VisionPlatformModule;
using Osmo.SDK.Vision;

namespace Byjus.RockSalon.Verticals {
    public class OsmoVisionService : MonoBehaviour, IVisionService {
        [SerializeField] public Text jsonText;
        [SerializeField] Text xRatioText;
        [SerializeField] Text yRatioText;
        [SerializeField] Text epsilonText;

        List<string> lastJsons;
        BoundingBox visionBoundingBox;
        bool jsonVisible;

        float xRatio = 1f;
        float yRatio = 1f;
        
        const float ratioInc = 0.02f;
        const float epsilonInc = 1f;

        public void ToggleJsonText() {
            jsonVisible = !jsonVisible;
            jsonText.gameObject.SetActive(jsonVisible);
        }

        public void OnXRatioPlus() {
            xRatio += ratioInc;
            xRatioText.text = xRatio + "";
        }

        public void OnXRatioMinus() {
            xRatio -= ratioInc;
            xRatioText.text = xRatio + "";
        }

        public void OnYRatioPlus() {
            yRatio += ratioInc;
            yRatioText.text = yRatio + "";
        }

        public void OnYRatioMinus() {
            yRatio -= ratioInc;
            yRatioText.text = yRatio + "";
        }

        public void OnPointComparePlus() {
            visionBoundingBox.hwPointCompareEpsilon += epsilonInc;
            epsilonText.text = visionBoundingBox.hwPointCompareEpsilon + "";
        }

        public void OnPointCompareMinus() {
            visionBoundingBox.hwPointCompareEpsilon -= epsilonInc;
            epsilonText.text = visionBoundingBox.hwPointCompareEpsilon + "";
        }

        public void Init() {
            lastJsons = new List<string>();
            //jsonText.text = lastJson;
            visionBoundingBox = new BoundingBox(new List<Vector2> { new Vector2(-100, 90), new Vector2(100, 90), new Vector2(100, -200), new Vector2(-100, -200) });

            VisionConnector.Register(
                    apiKey: API.Key,
                    objectName: "OsmoVisionServiceView",
                    functionName: "DispatchEvent",
                    mode: 147,
                    async: false,
                    hires: false
                );
        }

        public void DispatchEvent(string json) {
            if (json == null) { return; }
            if (lastJsons.Count >= Constants.INPUT_FRAME_COUNT) {
                lastJsons.RemoveAt(0);
            }
            lastJsons.Add(json);
        }

        List<JItem> GetConsolidatedObjects() {
            if (lastJsons.Count < Constants.INPUT_FRAME_COUNT) {
                Debug.LogError(lastJsons.Count + " is less than " + Constants.INPUT_FRAME_COUNT);
                throw new TooSoonToTellException();
            }

            var tLastJsons = new List<string>();
            tLastJsons.AddRange(lastJsons);
            lastJsons.Clear();

            var outputs = new List<JOutput>();
            foreach (var js in tLastJsons) {
                var ot = JsonUtility.FromJson<JOutput>(js);
                outputs.Add(ot);
            }

            var ret = new List<JItem>();

            foreach (var it in outputs[outputs.Count - 1].items) {
                var pos = new Vector2(it.pt.x, it.pt.y);
                int foundCount = 0;

                for (int i = outputs.Count - 2; i >= 0; i--) {
                    int found = 0;
                    foreach (var it2 in outputs[i].items) {
                        if (it2.id != it.id) { continue; }

                        var pos2 = new Vector2(it2.pt.x, it2.pt.y);

                        if (visionBoundingBox.PositionEquals(pos, pos2)) {
                            found++;
                        }
                    }

                    if (found > 0) { foundCount++; }
                    if (foundCount == Constants.ITEM_DETECTION_FRAME_THRESHOLD - 1) {
                        ret.Add(it);
                    }
                }
            }

            return ret;
        }

        public List<ExtInput> GetVisionObjects() {
                var items = GetConsolidatedObjects();

                var ret = new List<ExtInput>();
                int numBlues = 0, numReds = 0;
                foreach (var item in items) {
                    var pos = visionBoundingBox.GetScreenPoint(CameraUtil.MainDimens(), item.pt);
                    pos = CameraAdjustments(pos);

                    if (string.Equals(item.color, "blue")) {
                        ret.Add(new ExtInput { id = numBlues++, type = TileType.BLUE_ROD, position = pos });
                    } else if (string.Equals(item.color, "red")) {
                        ret.Add(new ExtInput { id = 1000 + numReds++, type = TileType.RED_CUBE, position = pos });
                    }
                }

                return ret;
        }

        Vector2 CameraAdjustments(Vector2 screenPoint) {
            var x = screenPoint.x * xRatio;
            var y = screenPoint.y * yRatio;

            return new Vector2(x, y);
        }
    }


    public class TooSoonToTellException : Exception {
        public override string Message => "Enough data was not accumulated for proper input";
    }

    public class FluctuatingInputException : Exception {

    }

    [Serializable]
    public class JOutput {
        public List<JItem> items;
    }

    [Serializable]
    public class JItem {
        public string color;
        public int id;
        public Point pt;

        public JItem() {
        }

        public JItem(JItem other) {
            color = other.color;
            id = other.id;
            pt = other.pt;
        }

        public override string ToString() {
            return "Color: " + color + ", id: " + id + ", pos: (" + pt + ")";
        }
    }

    [Serializable]
    public class Point {
        public float x;
        public float y;

        public Point(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public override string ToString() {
            return x + ", " + y;
        }
    }

    public class BoundingBox {
        public Vector2 topLeftRef;
        public Vector2 newTL;
        public Vector2 newTR;
        public Vector2 newBL;
        public Vector2 newBR;

        public float topWidth;
        public float bottomWidth;
        public float height;

        public float hwPointCompareEpsilon = 5f;

        public BoundingBox(List<Vector2> points) {
            topLeftRef = points[0];
            newTL = GetRelativePosFromTL(points[0]);
            newTR = GetRelativePosFromTL(points[1]);
            newBR = GetRelativePosFromTL(points[2]);
            newBL = GetRelativePosFromTL(points[3]);

            topWidth = newTR.x - newTL.x;
            bottomWidth = newBR.x - newBL.x;
            height = newTR.y - newBR.y;
        }

        override public string ToString() {
            return newTL + ", " + newTR + ", " + newBR + ", " + newBL;
        }

        public Vector2 GetRelativePosFromTL(Vector2 point) {
            return point - topLeftRef;
        }

        // w3 = ( w1 * h - w1 * y + w2 * y ) / h
        public float GetWidthAtYDist(float y) {
            var width = (topWidth * height - topWidth * y + bottomWidth * y) / height;
            return width;
        }


        public float GetLeftMostX(float y) {
            var x = newTL.x + (y - newTL.y) * (newBL.x - newTL.x) / (newBL.y - newTL.y);
            return x;
        }
        // by slope of 2 points formula
        public float GetDistFromLeft(Vector2 relPoint) {
            var leftPoint = GetLeftMostX(relPoint.y);
            var dist = relPoint.x - leftPoint;
            return dist;
        }

        public Vector2 GetScreenPoint(Vector2 screenDimens, Point point) {
            var vecPoint = new Vector2(point.x, point.y);
            var relPoint = GetRelativePosFromTL(vecPoint);

            // pos from top left
            var scrX = screenDimens.x * relPoint.x / topWidth;
            var scrY = screenDimens.y * relPoint.y / height;

            // pos from center
            scrX = -(screenDimens.x / 2) + scrX;
            scrY = (screenDimens.y / 2) + scrY;


            return new Vector2(scrX, scrY);
        }

        public bool PositionEquals(Vector2 point1, Vector2 point2) {
            return Mathf.Abs(point1.x - point2.x) < hwPointCompareEpsilon &&
                Mathf.Abs(point1.y - point2.y) < hwPointCompareEpsilon;
        }
    }
}
#endif