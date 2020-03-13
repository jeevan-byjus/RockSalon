using Byjus.RockSalon.Verticals;
using UnityEngine;

#if CC_STANDALONE
namespace Byjus.RockSalon.Externals {
    public class StandaloneExternalParent : MonoBehaviour {
        public HierarchyManager hierarchyManager;

        void AssignRefs() {
            hierarchyManager = FindObjectOfType<HierarchyManager>();
        }

        private void Start() {
            AssignRefs();
            Factory.Init();
            hierarchyManager.Setup();
        }
    }
}
#endif