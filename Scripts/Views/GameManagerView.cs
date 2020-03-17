using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Byjus.RockSalon.Ctrls;
using DG.Tweening;
using System;
using System.Collections.Generic;
using Byjus.RockSalon.Verticals;

namespace Byjus.RockSalon.Views {

    public class GameManagerView : MonoBehaviour, IGameManagerView {
        [SerializeField] SpriteRenderer bg;
        [SerializeField] GameObject blueCrystalPrefab;
        [SerializeField] GameObject redCrystalPrefab;
        [SerializeField] List<GameObject> monsterPrefabs;
        [SerializeField] List<LevelData> levels;
        [SerializeField] Text gameText;

        public OsmoVisionService osmoVisionService;

        public IGameManagerCtrl ctrl;

        public void CreateCrystal(CrystalType type, Vector2 position, Action<GameObject> onCreateDone) {
            var go = Instantiate(type == CrystalType.BLUE_CRYSTAL ? blueCrystalPrefab : redCrystalPrefab,
                position, Quaternion.identity, transform);
            onCreateDone(go);
        }

        public void MoveCrystal(GameObject crystalGo, Vector2 newPosition, Action onMoveDone) {
            crystalGo.transform.DOMove(newPosition, 0.4f).OnComplete(() => onMoveDone());
        }

        public void RemoveCrystal(GameObject crystalGo, Action onRemoveDone) {
            Destroy(crystalGo);
            onRemoveDone();
        }

        public void OnSubmitPressed() {
            ctrl.OnSubmitPressed();
        }

        public void OnCameraPlus() {
            Camera.main.orthographicSize += 5;
            var height = Camera.main.orthographicSize * 2;
            var width = Camera.main.aspect * height;
            bg.size = new Vector2(width, height);
        }

        public void OnCameraMinus() {
            Camera.main.orthographicSize -= 5;
            var height = Camera.main.orthographicSize * 2;
            var width = Camera.main.aspect * height;
            bg.size = new Vector2(width, height);
        }

        public void OnCrystalPlus() {
            osmoVisionService.ratio += 0.1f;
        }

        public void OnCrystalMinus() {
            osmoVisionService.ratio -= 0.1f;
        }

        public List<LevelData> GetAllLevels() {
            return levels;
        }

        public void InstantiateLevel(LevelInfo level) {
            var monsterPrefab = monsterPrefabs[level.monsterIndex];
            level.monster = Instantiate(monsterPrefab, transform);
            gameText.text = level.GetReqtString();
        }

        public void DestroyLevel(LevelInfo level) {
            Destroy(level.monster.gameObject);
        }
    }

    public interface IGameManagerView {
        List<LevelData> GetAllLevels();
        void CreateCrystal(CrystalType type, Vector2 position, Action<GameObject> onCreateDone);
        void MoveCrystal(GameObject crystalGo, Vector2 newPosition, Action onMoveDone);
        void RemoveCrystal(GameObject crystalGo, Action onRemoveDone);
        void InstantiateLevel(LevelInfo level);
        void DestroyLevel(LevelInfo level);
    }
}