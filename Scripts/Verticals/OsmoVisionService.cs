﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Byjus.RockSalon.Util;
using UnityEngine.UI;

#if !CC_STANDALONE
using Osmo.SDK;
using Osmo.SDK.VisionPlatformModule;
using Osmo.SDK.Vision;

namespace Byjus.RockSalon.Verticals {
    public class OsmoVisionService : MonoBehaviour, IVisionService {
        [SerializeField] Text jsonText;

        string lastJson;
        BoundingBox visionBoundingBox;
        float frameTheta;
        bool jsonVisible;

        float ratio = 0.6f;

        public void ToggleJsonText() {
            jsonVisible = !jsonVisible;
            jsonText.gameObject.SetActive(jsonVisible);
        }

        public void OnRatioPlus() {
            ratio += 0.05f;
        }

        public void OnRatioMinus() {
            ratio -= 0.05f;
        }

        public void Init() {
            lastJson = "{}";
            jsonText.text = lastJson;
            visionBoundingBox = new BoundingBox(new List<Point> { new Point(-56, 81), new Point(91, 82), new Point(91, -116), new Point(-56, -116) });
            frameTheta = CalculateAngle(visionBoundingBox);

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
            jsonText.text = lastJson;
        }

        public List<ExtInput> GetVisionObjects() {
            try {
                var output = JsonUtility.FromJson<JOutput>(lastJson);
                if (output == null || output.items == null) {
                    Debug.LogError("Returning empty for json " + lastJson);
                    return new List<ExtInput>();
                }
                
                Debug.LogError("Json: " + lastJson);

                foreach (var it in output.items) {
                    it.pt = GetRelativePos(frameTheta, visionBoundingBox.bottomLeft, it.pt);
                }

                var ret = new List<ExtInput>();
                int numBlues = 0, numReds = 0;
                foreach (var item in output.items) {
                    var hwPos = new Vector2(item.pt.x, item.pt.y);
                    var pos = GetPositionAccordingToCamera(hwPos);
                    Debug.LogError("Item: " + item + ", hwPos: " + hwPos + ", WorldPos: " + pos);

                    if (string.Equals(item.color, "blue")) {
                        ret.Add(new ExtInput { id = numBlues++, type = TileType.BLUE_ROD, position = pos  });
                    } else if (string.Equals(item.color, "red")) {
                        ret.Add(new ExtInput { id = 1000 + numReds++, type = TileType.RED_CUBE, position = pos });
                    }
                }

                Debug.LogError("\n\n");
                return ret;

            } catch (Exception e) {
                Debug.LogError("Error while parsing lastJson: " + lastJson + "\nException: " + e.Message);
                return new List<ExtInput>();
            }
        }

        private Point VecToPoint(Vector2 vec) {
            return new Point(vec.x, vec.y);
        }

        private float CalculateAngle(BoundingBox boundingBox) {
            Vector2 top = new Vector2(boundingBox.topRight.x - boundingBox.topLeft.x,
                boundingBox.topRight.y - boundingBox.topLeft.y);
            Vector2 bottom = new Vector2(boundingBox.bottomRight.x - boundingBox.bottomLeft.x,
                boundingBox.bottomRight.y - boundingBox.bottomLeft.y);
            float angleTop = Mathf.Rad2Deg * Mathf.Atan2(top.y, top.x);
            float angleBottom = Mathf.Rad2Deg * Mathf.Atan2(bottom.y, bottom.x);
            float angle = (angleTop + angleBottom) / 2;
            return angle;
        }

        private Point GetRelativePos(float theta, Point relativeOrigin, Point globalPos) {
            Quaternion rotation = Quaternion.AngleAxis(theta, new Vector3(0, 0, 1));
            Vector2 playmatPosition = (Vector2) (Quaternion.Inverse(rotation)
                * (Vector3) (new Vector2(globalPos.x - relativeOrigin.x,
                    globalPos.y - relativeOrigin.y)));
            return VecToPoint(playmatPosition);
        }

        private Vector2 GetPositionAccordingToCamera(Vector2 point) {
            var hwWidth = visionBoundingBox.topRight.x - visionBoundingBox.topLeft.x;
            var widthRatio = CameraUtil.MainWidth() / hwWidth * ratio;

            var hwHeight = visionBoundingBox.topLeft.y - visionBoundingBox.bottomLeft.y;
            var heightRatio = CameraUtil.MainHeight() / hwHeight * ratio;

            return new Vector2(point.x * widthRatio, point.y * heightRatio);
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

    [Serializable]
    public class BoundingBox {
        public Point topLeft;
        public Point topRight;
        public Point bottomRight;
        public Point bottomLeft;

        public BoundingBox(List<Point> points) {
            topLeft = points[0];
            topRight = points[1];
            bottomRight = points[2];
            bottomLeft = points[3];
        }

        override public string ToString() {
            return topLeft + ", " + topRight + ", " + bottomRight + ", " + bottomLeft;
        }
    }
}
#endif