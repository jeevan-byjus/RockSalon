using UnityEngine;
using Byjus.RockSalon.Views;
using Byjus.RockSalon.Verticals;
using System.Collections.Generic;

namespace Byjus.RockSalon.Ctrls {
    public class GameManagerCtrl : IGameManagerCtrl, IExtInputListener {
        public IGameManagerView view;

        List<Crystal> crystals;

        public void Init() {
            crystals = new List<Crystal>();
        }

        public void OnCrystalAdded(TileType type, int id, Vector2 position) {
            Debug.Log("Adding crystal " + type + ", id: " + id + ", at pos: " + position);
            var crystal = new Crystal {
                id = id,
                type = TileTypeToCrystalType(type),
                position = position,
                confirmedInView = false,
            };

            view.CreateCrystal(crystal.type, crystal.position, (go) => { OnCrystalAddDone(crystal, go); });
        }

        public void OnCrystalMoved(TileType type, int id, Vector2 newPosition) {
            Debug.Log("Moving crystal " + type + ", id: " + id + ", newPos " + newPosition);
            var crystal = FindCrystal(TileTypeToCrystalType(type), id);
            if (crystal == null) {
                Debug.LogError("Crystal is null for type: " + type + ", id: " + id);
            }

            crystal.confirmedInView = false;
            crystal.position = newPosition;
            view.MoveCrystal(crystal.go, newPosition, () => { OnCrystalMoveDone(crystal); });
        }

        public void OnCrystalRemoved(TileType type, int id) {
            Debug.Log("Remove crystal " + type + ", id: " + id);
            var crystal = FindCrystal(TileTypeToCrystalType(type), id);
            if (crystal == null) {
                Debug.LogError("Crystal is null for type: " + type + ", id: " + id);
            }

            crystal.confirmedInView = false;
            view.RemoveCrystal(crystal.go, () => { OnCrystalRemoveDone(crystal); });
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

        public void OnInputStart() {

        }

        public void OnInputEnd() {

        }
    }

    public interface IGameManagerCtrl {
        void Init();
    }

    public enum CrystalType { BLUE_CRYSTAL, RED_CRYSTAL }

    class Crystal {
        public CrystalType type;
        public int id;
        public GameObject go;
        public Vector2 position;
        public bool confirmedInView;
    }
}