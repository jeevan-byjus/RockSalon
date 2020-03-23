using System;
using UnityEngine;

namespace Byjus.RockSalon.Util {
    public class Constants {
        public const float INPUT_DELAY = 0.8f;
        public static float SW_EQUAL_POSITION_DIFF_PERCENT = 0.5f / 100;
        public static float SW_SAME_POINT_MOVED_DIFF_PERCENT = 30.0f / 100;

        public const int ITEM_DETECTION_FRAME_THRESHOLD = 3;
        public const int INPUT_FRAME_COUNT = 3;
        public static float CAMERA_ADJUST_X_RATIO = 0.8f;
        public static float CAMERA_ADJUST_Y_RATIO = 0.8f;
        public static float HW_POINT_COMPARE_EPSILON = 5f;

        public const int POSITION_ROUND_OFF_TO_DIGITS = 4;
    }

    public class CameraUtil {
        public static Vector2 MainDimens() {
            var cam = Camera.main;
            var h = cam.orthographicSize * 2;
            var w = cam.aspect * h;

            return new Vector2(w, h);
        }

    }

    public class GenUtil {

        public static bool EqualPositionSw(Vector2 point1, Vector2 point2) {
            var dimen = CameraUtil.MainDimens();
            var widthEpsilon = Rounded(dimen.x * Constants.SW_EQUAL_POSITION_DIFF_PERCENT);
            var heightEpsilon = Rounded(dimen.y * Constants.SW_EQUAL_POSITION_DIFF_PERCENT);

            var xDiff = Rounded(Mathf.Abs(point1.x - point2.x));
            var yDiff = Rounded(Mathf.Abs(point1.y - point2.y));

            Debug.Log("comparison sw: diff: (" + xDiff + ", " + yDiff + "), epsilon: (" + widthEpsilon + ", " + heightEpsilon + ")");

            return xDiff <= widthEpsilon && yDiff <= heightEpsilon;
        }

        public static string PosToString(Vector2 pos) {
            return "(" + pos.x + ", " + pos.y + ")";
        }

        public static Vector2 RoundedPos(Vector2 pos) {
            return new Vector2(Rounded(pos.x), Rounded(pos.y));
        }

        public static float Rounded(float x) {
            return (float) Math.Round(x, Constants.POSITION_ROUND_OFF_TO_DIGITS);
        }
    }
}