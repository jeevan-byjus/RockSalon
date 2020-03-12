using UnityEngine;
using Byjus.RockSalon.Views;
using Byjus.RockSalon.Verticals;

namespace Byjus.RockSalon.Ctrls {
    public class GameManagerCtrl : IGameManagerCtrl, IExtInputListener {
        public IGameManagerView view;

        public void Init() {

        }

        public void OnBlueRodAdded(int id, Vector2 position) {
            throw new System.NotImplementedException();
        }

        public void OnBlueRodMoved(int id, Vector2 newPosition) {
            throw new System.NotImplementedException();
        }

        public void OnBlueRodRemoved(int id) {
            throw new System.NotImplementedException();
        }

        public void OnRedCubeAdded(int id, Vector2 position) {
            throw new System.NotImplementedException();
        }

        public void OnRedCubeMoved(int id, Vector2 newPosition) {
            throw new System.NotImplementedException();
        }

        public void OnRedCubeRemoved(int id) {
            throw new System.NotImplementedException();
        }
    }

    public interface IGameManagerCtrl {
        void Init();
        
    }
}