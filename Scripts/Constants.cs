using System;
using UnityEngine;

namespace Byjus.RockSalon.Util {
    public class Constants {
        public const float INPUT_DELAY = 0.8f;
        public static float SW_POINT_COMPARE_EPSILON_PERCENT = 0.1f / 100;
        public static float SW_SAME_POINT_DIST_THRESHOLD_PERCENT = 8.0f / 100;

        public const int ITEM_DETECTION_FRAME_THRESHOLD = 3;
        public const int INPUT_FRAME_COUNT = 3;
        
    }

    public class CameraUtil {
        public static Vector2 MainDimens() {
            var cam = Camera.main;
            var h = cam.orthographicSize * 2;
            var w = cam.aspect * h;

            return new Vector2(w, h);
        }

    }

    public class GeneralUtil {

        public static bool EqualPositionSw(Vector2 point1, Vector2 point2) {
            var dimen = CameraUtil.MainDimens();
            var widthEpsilon = dimen.x * Constants.SW_POINT_COMPARE_EPSILON_PERCENT;
            var heightEpsilon = dimen.y * Constants.SW_POINT_COMPARE_EPSILON_PERCENT;

            return Mathf.Abs(point1.x - point2.x) < widthEpsilon &&
                Mathf.Abs(point1.y - point2.y) < heightEpsilon;
        }
    }
}