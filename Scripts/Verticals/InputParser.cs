using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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
            yield return new WaitForSeconds(3f);
        }
    }

    public interface IExtInputListener {
        void OnRedCubeAdded(int id, Vector2 position);
        void OnBlueRodAdded(int id, Vector2 position);
        void OnRedCubeRemoved(int id);
        void OnBlueRodRemoved(int id);
        void OnRedCubeMoved(int id, Vector2 newPosition);
        void OnBlueRodMoved(int id, Vector2 newPosition);
    }

}