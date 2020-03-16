using UnityEngine;
using Byjus.RockSalon.Views;
using Byjus.RockSalon.Verticals;
using System.Collections.Generic;

namespace Byjus.RockSalon.Ctrls {
    public class GameManagerCtrl : IGameManagerCtrl, IExtInputListener {
        public IGameManagerView view;

        List<Crystal> crystals;
        List<LevelInfo> allLevels;
        int currLevelId;

        public void Init() {
            crystals = new List<Crystal>();
            allLevels = new List<LevelInfo>();
            foreach (var vl in view.GetAllLevels()) { allLevels.Add(GetLevelInfoForViewInfo(vl)); }

            LoadLevel(0);
        }

        void LoadLevel(int levelIndex) {
            currLevelId = levelIndex;
            view.InstantiateLevel(allLevels[currLevelId]);
        }

        LevelInfo GetLevelInfoForViewInfo(LevelData level) {
            var info = new LevelInfo();
            info.monsterIndex = level.monsterIndex;

            if (level.generic) {
                info.totalReqt = level.totalReqt;
                info.blueReqt = new Reqt { comparison = Comparison.ANY };
                info.redReqt = new Reqt { comparison = Comparison.ANY };

            } else {
                info.totalReqt = level.numBlue + level.numRed;
                info.blueReqt = new Reqt { comparison = Comparison.EQUAL, numCrystals = level.numBlue };
                info.redReqt = new Reqt { comparison = Comparison.EQUAL, numCrystals = level.numRed };
            }

            return info;
        }

        public void OnInputStart() {

        }

        public void OnCrystalAdded(TileType type, int id, Vector2 position) {
            var crystal = new Crystal {
                id = id,
                type = TileTypeToCrystalType(type),
                position = position,
                confirmedInView = false,
            };

            view.CreateCrystal(crystal.type, crystal.position, (go) => { OnCrystalAddDone(crystal, go); });
        }

        public void OnCrystalMoved(TileType type, int id, Vector2 newPosition) {
            var crystal = FindCrystal(TileTypeToCrystalType(type), id);
            if (crystal == null) {
                Debug.LogError("Crystal is null for type: " + type + ", id: " + id);
            }

            crystal.confirmedInView = false;
            crystal.position = newPosition;
            view.MoveCrystal(crystal.go, newPosition, () => { OnCrystalMoveDone(crystal); });
        }

        public void OnCrystalRemoved(TileType type, int id) {
            var crystal = FindCrystal(TileTypeToCrystalType(type), id);
            if (crystal == null) {
                Debug.LogError("Crystal is null for type: " + type + ", id: " + id);
            }

            crystal.confirmedInView = false;
            view.RemoveCrystal(crystal.go, () => { OnCrystalRemoveDone(crystal); });
        }

        public void OnInputEnd() {

        }

        public void OnSubmitPressed() {
            // validate crystals
            // if proper, show win text, or congrats text
            // else, show dissatisfaction text
            view.DestroyLevel(allLevels[currLevelId]);
            LoadLevel((currLevelId + 1) % allLevels.Count);
        }

        void OnCrystalAddDone(Crystal crystal, GameObject crystalGo) {
            crystal.confirmedInView = true;
            crystal.go = crystalGo;
            crystals.Add(crystal);
        }

        void OnCrystalMoveDone(Crystal crystal) {
            crystal.confirmedInView = true;
        }

        void OnCrystalRemoveDone(Crystal crystal) {
            crystals.Remove(crystal);
        }

        Crystal FindCrystal(CrystalType type, int id) {
            return crystals.Find(x => x.type == type && x.id == id);
        }

        CrystalType TileTypeToCrystalType(TileType type) {
            return type == TileType.BLUE_ROD ? CrystalType.BLUE_CRYSTAL : CrystalType.RED_CRYSTAL;
        }
    }

    public interface IGameManagerCtrl {
        void Init();
        void OnSubmitPressed();
    }

    public enum CrystalType { BLUE_CRYSTAL, RED_CRYSTAL }

    public class Crystal {
        public CrystalType type;
        public int id;
        public GameObject go;
        public Vector2 position;
        public bool confirmedInView;
    }

    public enum Comparison {
        ANY,
        EQUAL
    }

    public class Reqt {
        public Comparison comparison;
        public int numCrystals;

        public override string ToString() {
            return comparison + ", " + numCrystals;
        }
    }

    public class LevelInfo {
        public int monsterIndex;
        public int totalReqt;
        public Reqt blueReqt;
        public Reqt redReqt;

        public GameObject monster;

        public override string ToString() {
            return "Monster Index: " + monsterIndex + ", totalReqt: " + totalReqt + ", BlueReqt: " + blueReqt + ", RedReqt: " + redReqt;
        }

        public string GetReqtString() {
            if (blueReqt.comparison == Comparison.ANY) {
                return "I want " + totalReqt + " crystals on my head";
            } else {
                return "I want " + blueReqt.numCrystals + " Blue crystals and " + redReqt.numCrystals + " Red crystals on my head";
            }
        }
    }
}