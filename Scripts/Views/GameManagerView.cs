using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Byjus.RockSalon.Ctrls;
using DG.Tweening;
using System;
using System.Collections.Generic;
using Byjus.RockSalon.Util;

namespace Byjus.RockSalon.Views {

    public class GameManagerView : MonoBehaviour, IGameManagerView {
        [SerializeField] SpriteRenderer bg;
        [SerializeField] GameObject blueCrystalPrefab;
        [SerializeField] GameObject redCrystalPrefab;
        [SerializeField] List<LevelData> levels;
        [SerializeField] Text gameText;
        [SerializeField] Text epsilonText;
        [SerializeField] Text diffText;

        GameObject monster;

        const float epsilonInc = 0.05f / 100;
        const float diffInc = 0.5f / 100;

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

        public void OnEpsilonPlus() {
            Constants.SW_EQUAL_POSITION_DIFF_PERCENT += epsilonInc;
            epsilonText.text = (Constants.SW_EQUAL_POSITION_DIFF_PERCENT * 100) + " %";
        }

        public void OnEpsilonMinus() {
            Constants.SW_EQUAL_POSITION_DIFF_PERCENT -= epsilonInc;
            epsilonText.text = (Constants.SW_EQUAL_POSITION_DIFF_PERCENT * 100) + " %";
        }

        public void OnDiffThresholdPlus() {
            Constants.SW_SAME_POINT_MOVED_DIFF_PERCENT += diffInc;
            diffText.text = (Constants.SW_SAME_POINT_MOVED_DIFF_PERCENT * 100) + " %";
        }

        public void OnDiffThresholdMinus() {
            Constants.SW_SAME_POINT_MOVED_DIFF_PERCENT -= diffInc;
            diffText.text = (Constants.SW_SAME_POINT_MOVED_DIFF_PERCENT * 100) + " %";
        }

        public List<LevelData> GetAllLevels() {
            return levels;
        }

        public void InstantiateLevel(LevelInfo level, Action onDone) {
            monster = Instantiate(level.monsterPrefab, transform);
            gameText.text = level.GetReqtString();

            onDone();
        }

        public void DestroyLevel(LevelInfo level, Action onDone) {
            Destroy(monster.gameObject);
            onDone();
        }

        public void ShowCharacterAnimation(CharacterState state, Action onDone) {
            // do something with the monster here based on state
            onDone();
        }
    }

    public interface IGameManagerView {
        List<LevelData> GetAllLevels();
        void CreateCrystal(CrystalType type, Vector2 position, Action<GameObject> onCreateDone);
        void MoveCrystal(GameObject crystalGo, Vector2 newPosition, Action onMoveDone);
        void RemoveCrystal(GameObject crystalGo, Action onRemoveDone);
        void InstantiateLevel(LevelInfo level, Action onDone);
        void DestroyLevel(LevelInfo level, Action onDone);
        void ShowCharacterAnimation(CharacterState state, Action onDone);
    }
}