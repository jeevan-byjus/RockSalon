using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

                var ret = new List<ExtInput>();
                int numBlues = 0, numReds = 0;
                foreach (var item in output.items) {
                    if (string.Equals(item.color, "blue")) {
                        numBlues++;
                    } else if (string.Equals(item.color, "red")) {
                        numReds++;
                    }
                }

                numBlues = Mathf.CeilToInt(numBlues / 10);
                //Debug.LogError("Returning vision objects: Number of Blues " + numBlues + ", Number of reds: " + numReds);

                for (int i = 0; i < numBlues; i++) { ret.Add(new ExtInput { id = i, type = ExtInput.TileType.BLUE_ROD }); }
                for (int i = 0; i < numReds; i++) { ret.Add(new ExtInput { id = (i + numBlues) + 1000, type = ExtInput.TileType.RED_CUBE }); }

                return ret;

            } catch (System.Exception e) {
                Debug.LogError("Error while parsing lastJson: " + lastJson + "\nException: " + e.Message);
                return new List<ExtInput>();
            }
        }
    }

    [System.Serializable]
    public class JOutput {
        public int frameCounter;
        public float handProbability;
        public List<JItem> items;
    }

    [System.Serializable]
    public class JItem {
        public string color;
        public int id;
    }
}
#endif