using System;
using UnityEngine;

namespace Byjus.RockSalon.Util {
    public class Constants {
        public const float INPUT_DELAY = 0.8f;
        
    }

    public class CameraUtil {
        public static float Width(Camera cam) {
            return Height(cam) * cam.aspect;
        }

        public static float Height(Camera cam) {
            return cam.orthographicSize * 2;
        }

    }
}