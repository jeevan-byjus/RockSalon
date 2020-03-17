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
        [SerializeField] Text jsonText;

        string lastJson;
        BoundingBox visionBoundingBox;
        bool jsonVisible;

        float ratio = 0.6f;

        public void ToggleJsonText() {
            jsonVisible = !jsonVisible;
            jsonText.gameObject.SetActive(jsonVisible);
        }

        public void OnRatioPlus() {
            ratio += 0.1f;
        }

        public void OnRatioMinus() {
            ratio -= 0.1f;
        }

        public void Init() {
            lastJson = "{}";
            //jsonText.text = lastJson;
            visionBoundingBox = new BoundingBox(new List<Vector2> { new Vector2(-90, 90), new Vector2(100, 90), new Vector2(190, -200), new Vector2(-150, -200) });

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
            lastJson = json;
            //jsonText.text = lastJson;
        }

        public List<ExtInput> GetVisionObjects() {
            try {
                var output = JsonUtility.FromJson<JOutput>(lastJson);
                if (output == null || output.items == null) {
                    Debug.LogError("Returning empty for json " + lastJson);
                    return new List<ExtInput>();
                }

                string outS = "Number of items: " + output.items.Count + "\n";

                var ret = new List<ExtInput>();
                int numBlues = 0, numReds = 0;
                foreach (var item in output.items) {
                    var pos = visionBoundingBox.GetScreenPoint(item.pt);
                    outS += "Item: " + item + ", screen position: " + pos + "\n";

                    if (string.Equals(item.color, "blue")) {
                        ret.Add(new ExtInput { id = numBlues++, type = TileType.BLUE_ROD, position = pos });
                    } else if (string.Equals(item.color, "red")) {
                        ret.Add(new ExtInput { id = 1000 + numReds++, type = TileType.RED_CUBE, position = pos });
                    }
                }

                jsonText.text = outS;

                return ret;

            } catch (Exception e) {
                Debug.LogError("Error while parsing lastJson: " + lastJson + "\nException: " + e.Message);
                return new List<ExtInput>();
            }
        }
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

        public Vector2 GetScreenPoint(Point point) {
            var vecPoint = new Vector2(point.x, point.y);
            var relPoint = GetRelativePosFromTL(vecPoint);
            var widthAtPoint = GetWidthAtYDist(-relPoint.y);
            var xDist = GetDistFromLeft(relPoint);

            var camW = CameraUtil.MainWidth();
            var camH = CameraUtil.MainHeight();

            // pos from top left
            var scrX = camW * xDist / widthAtPoint;
            var scrY = camH * relPoint.y / height;

            Debug.Log("Point: " + point + ", relPoint: " + relPoint + ", Width at point: " + widthAtPoint + ", xDist: " + xDist + ", camW: " + camW + ", camH: " + camH + ", scrX: " + scrX + " scrY: " + scrY);

            // pos from center
            scrX = -(camW / 2) + scrX;
            scrY = (camH / 2) + scrY;


            return new Vector2(scrX, scrY);
        }
    }
}
#endif