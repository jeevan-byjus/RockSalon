using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Byjus.RockSalon.Ctrls;
using DG.Tweening;
using System;
using System.Collections.Generic;

namespace Byjus.RockSalon.Views {

    public class GameManagerView : MonoBehaviour, IGameManagerView {
        [SerializeField] GameObject blueCrystalPrefab;
        [SerializeField] GameObject redCrystalPrefab;
        [SerializeField] List<GameObject> monsterPrefabs;
        [SerializeField] List<LevelData> levels;
        [SerializeField] Text gameText;

        public IGameManagerCtrl ctrl;

        public void CreateCrystal(CrystalType type, Vector2 position, Action<GameObject> onCreateDone) {
            var go = Instantiate(type == CrystalType.BLUE_CRYSTAL ? blueCrystalPrefab : redCrystalPrefab,
                position, Quaternion.identity, transform);
            onCreateDone(go);
        }

        public void MoveCrystal(GameObject crystalGo, Vector2 newPosition, Action onMoveDone) {
            crystalGo.transform.DOMove(newPosition, 0.5f).OnComplete(() => onMoveDone());
        }

        public void RemoveCrystal(GameObject crystalGo, Action onRemoveDone) {
            Destroy(crystalGo);
            onRemoveDone();
        }

        public void OnSubmitPressed() {

        }

        public List<LevelData> GetAllLevels() {
            return levels;
        }

        public void InstantiateLevel(LevelInfo level) {
            var monsterPrefab = monsterPrefabs[level.monsterIndex];
            level.monster = Instantiate(monsterPrefab, transform);
            gameText.text = level.GetReqtString();
        }
    }

    public interface IGameManagerView {
        List<LevelData> GetAllLevels();
        void CreateCrystal(CrystalType type, Vector2 position, Action<GameObject> onCreateDone);
        void MoveCrystal(GameObject crystalGo, Vector2 newPosition, Action onMoveDone);
        void RemoveCrystal(GameObject crystalGo, Action onRemoveDone);
        void InstantiateLevel(LevelInfo level);
    }
}