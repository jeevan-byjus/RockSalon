using System;
using UnityEngine;
using Byjus.RockSalon.Externals;

namespace Byjus.RockSalon.Verticals {
    public class Factory {
        static IVisionService visionService;

#if CC_STANDALONE
        public static void Init() {
            visionService = new StandaloneVisionService();
#elif UNITY_EDITOR
        public static void Init(IOsmoEditorVisionHelper editorVisionHelper) {
            visionService = new OsmoEditorVisionService(editorVisionHelper);
#else
        public static void Init(OsmoVisionService visionServiceView) {
            visionService = visionServiceView;
#endif
            visionService.Init();
        }

        public static IVisionService GetVisionService() {
            return visionService;
        }
    }
}