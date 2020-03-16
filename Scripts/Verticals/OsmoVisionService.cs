using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if !CC_STANDALONE
using Osmo.SDK;
using Osmo.SDK.VisionPlatformModule;
using Osmo.SDK.Vision;

namespace Byjus.RockSalon.Verticals {
    public class OsmoVisionService : MonoBehaviour, IVisionService {
        string lastJson;

        public void Init() {
            lastJson = "{}";
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
        }

        public List<ExtInput> GetVisionObjects() {
            try {
                var output = JsonUtility.FromJson<JOutput>(lastJson);
                if (output == null || output.items == null) {
                    Debug.LogError("Returning empty for json " + lastJson);
                }

                if (!output.foundFrame) {
                    output.frameCorners = new List<Point> { new Point(-56, 81), new Point(91, 82), new Point(91, -116), new Point(-56, -116) };
                }

                BoundingBox frameBox = new BoundingBox(output.frameCorners);
                float frameTheta = CalculateAngle(frameBox);
                
                List<JItem> transformedCubes = output.items.Select(cube => {
                    var transformedCube = new JItem(cube);
                    transformedCube.pt = GetRelativePos(frameTheta, frameBox.bottomLeft, cube.pt);
                    return transformedCube;
                }).ToList();

                var ret = new List<ExtInput>();
                int numBlues = 0, numReds = 0;
                foreach (var item in output.items) {
                    if (string.Equals(item.color, "blue")) {
                        numBlues++;
                    } else if (string.Equals(item.color, "red")) {
                        numReds++;
                    }
                }

                //numBlues = Mathf.CeilToInt(numBlues / 10);
                //Debug.LogError("Returning vision objects: Number of Blues " + numBlues + ", Number of reds: " + numReds);

                for (int i = 0; i < numBlues; i++) {
                    ret.Add(new ExtInput {
                        id = i,
                        type = TileType.BLUE_ROD,
                        position = new Vector2()
                    });
                }
                for (int i = 0; i < numReds; i++) { ret.Add(new ExtInput { id = (i + numBlues) + 1000, type = TileType.RED_CUBE }); }

                return ret;

            } catch (System.Exception e) {
                Debug.LogError("Error while parsing lastJson: " + lastJson + "\nException: " + e.Message);
                return new List<ExtInput>();
            }
        }

        private Vector2 PointToVec(Point point) {
            return new Vector2(point.x, point.y);
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
        private Point GetGlobalPos(float theta, Point relativeOrigin, Point relativePos) {
            Quaternion rotation = Quaternion.AngleAxis(theta, new Vector3(0, 0, 1));
            Vector2 globalPosition = PointToVec(relativeOrigin) + (Vector2) (rotation * (new Vector3(relativePos.x, relativePos.y, 0)));
            return VecToPoint(globalPosition);
        }
    }

    [Serializable]
    public class JOutput {
        public List<JItem> items;
        public bool foundFrame;
        public List<Point> frameCorners;
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