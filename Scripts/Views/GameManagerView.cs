using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Byjus.RockSalon.Ctrls;

namespace Byjus.RockSalon.Views {

    public class GameManagerView : MonoBehaviour, IGameManagerView {
        [SerializeField] GameObject blueRodPrefab;
        [SerializeField] GameObject redCubePrefab;

        public GameObject CreateBlueRod(Vector2 position) {
            var go = Instantiate(blueRodPrefab, position, Quaternion.identity, transform);
            return go;
        }

        public GameObject CreateRedCube(Vector2 position) {
            var go = Instantiate(redCubePrefab, position, Quaternion.identity, transform);
            return go;
        }

        public void MoveBlueRod(GameObject blueRod, Vector2 newPosition) {
            throw new System.NotImplementedException();
        }

        public void MoveRedCube(GameObject redCube, Vector2 newPosition) {
            throw new System.NotImplementedException();
        }
    }

    public interface IGameManagerView {
        GameObject CreateBlueRod(Vector2 position);
        GameObject CreateRedCube(Vector2 position);
        void MoveBlueRod(GameObject blueRod, Vector2 newPosition);
        void MoveRedCube(GameObject redCube, Vector2 newPosition);
    }
}