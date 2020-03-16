using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Byjus.RockSalon {
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
    public class LevelData : ScriptableObject {
        public int monsterIndex;
        public bool generic;
        public int totalReqt;
        public int numBlue;
        public int numRed;
    }
}