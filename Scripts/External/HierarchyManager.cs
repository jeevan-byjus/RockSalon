using UnityEngine;
using Byjus.RockSalon.Ctrls;
using Byjus.RockSalon.Views;
using Byjus.RockSalon.Verticals;

namespace Byjus.RockSalon.Externals {
    public class HierarchyManager : MonoBehaviour {
        [SerializeField] InputParser inputParser;
        [SerializeField] GameManagerView gameManager;

        public void Setup() {
            GameManagerCtrl gameCtrl = new GameManagerCtrl();

            inputParser.inputListener = gameCtrl;

            gameManager.ctrl = gameCtrl;
            gameCtrl.view = gameManager;

            ((IGameManagerCtrl) gameCtrl).Init();
            inputParser.Init();
        }
    }
}