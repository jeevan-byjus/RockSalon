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
            for (int i = 0; i < numBlue; i++) {
                ret.Add(new ExtInput {
                    type = TileType.BLUE_ROD,
                    id = i,
                    position = GeneratePos(ret)
                });
            }

            for (int i = 0; i < numRed; i++) {
                ret.Add(new ExtInput {
                    type = TileType.RED_CUBE,
                    id = (numBlue + i) + 1000,
                    position = GeneratePos(ret)
                });
            }


            var str = "Sending input:\n";
            foreach (var x in ret) { str += x.ToString() + "\n"; }
            Debug.LogError(str);

            return ret;
        }

        Vector2 GeneratePos(List<ExtInput> objs) {
            var pos = GetRandomPos();
            while (ExistsPosition(pos, objs)) {
                pos = GetRandomPos();
            }

            return pos;
        }

        Vector2 GetRandomPos() {
            var x = Random.Range(-7, 7);
            var y = Random.Range(-8, 8);
            return new Vector2(x, y);
        }

        bool ExistsPosition(Vector2 testPos, List<ExtInput> objs) {
            foreach (var obj in objs) { if (obj.position == testPos) { return true; } }
            return false;
        }

        public void Init() {

        }
    }

    public enum TileType { RED_CUBE, BLUE_ROD }

    public class ExtInput {
        public TileType type;
        public int id;
        public Vector2 position;

        public ExtInput() { }

        public ExtInput(TileType type, int id, Vector2 position) {
            this.type = type;
            this.id = id;
            this.position = position;
        }

        public override string ToString() {
            return id + ", " + type + ", " + position;
        }
    }
}