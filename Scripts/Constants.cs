using System;
using UnityEngine;

namespace Byjus.RockSalon.Util {
    public class Constants {
        public const float INPUT_DELAY = 0.8f;
        
    }

    public class CameraUtil {

        public static float MainWidth() {
            var cam = Camera.main;
            return MainHeight() * cam.aspect;
        }

        public static float MainHeight() {
            var cam = Camera.main;
            return cam.orthographicSize * 2;
        }

    }
}