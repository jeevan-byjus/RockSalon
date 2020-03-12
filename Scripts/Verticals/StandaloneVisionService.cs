using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Byjus.RockSalon.Verticals {
    public interface IVisionService {
        void Init();
        List<ExtInput> GetVisionObjects();
    }

    public class StandaloneVisionService : IVisionService {
        public List<ExtInput> GetVisionObjects() {
            var numRed = Random.Range(0, 5);
            var numBlue = Random.Range(0, 5);

            var ret = new List<ExtInput>();
            for (int i = 0; i < numBlue; i++) { ret.Add(new ExtInput { type = ExtInput.TileType.BLUE_ROD, id = i }); }
            for (int i = 0; i < numRed; i++) { ret.Add(new ExtInput { type = ExtInput.TileType.RED_CUBE, id = (numBlue + i) + 1000 }); }

            return ret;
        }

        public void Init() {

        }
    }

    public class ExtInput {
        public enum TileType { RED_CUBE, BLUE_ROD }
        public TileType type;
        public int id;

        public override string ToString() {
            return id + ", " + type;
        }
    }
}