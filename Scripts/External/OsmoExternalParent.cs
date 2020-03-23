using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Byjus.RockSalon.Verticals;
using Byjus.RockSalon.Util;

#if !CC_STANDALONE
using Osmo.SDK;
using Osmo.Container.Common;
using Osmo.SDK.Internal;

namespace Byjus.RockSalon.Externals {
    public class OsmoExternalParent : OsmoGameBase, IOsmoEditorVisionHelper {
        [SerializeField] TangibleManager mManager;
        [SerializeField] OsmoVisionService osmoVisionServiceView;
        [SerializeField] HierarchyManager hierarchyManager;

        public TangibleManager tangibleManager { get { return mManager; } }

        public Vector2 GetCameraDimens() {
            return new Vector2(TangibleCamera.Width, TangibleCamera.Height);
        }

        void AssignRefs() {
            mManager = FindObjectOfType<TangibleManager>();
            osmoVisionServiceView = FindObjectOfType<OsmoVisionService>();
            hierarchyManager = FindObjectOfType<HierarchyManager>();
        }

        protected override void GameStart() {
            if (Bridge != null) {
                Bridge.Helper.SetOnMainMenuScreen(false);
                Bridge.Helper.OnSettingsButtonClick += OnSettingsButtonClicked;
                Bridge.Helper.SetSettingsButtonVisibility(true);
                Bridge.Helper.SetVisionActive(true);
                Bridge.Helper.SetOsmoWorldStickersAllowed(true);

                AssignRefs();

#if UNITY_EDITOR
                Factory.Init(this);
#else
                Factory.Init(osmoVisionServiceView);
#endif
                hierarchyManager.Setup();

            } else {
                Debug.LogWarning("[VisionTest] You are running without the Osmo bridge. No Osmo services will be loaded. Bridge.Helper will be null");
            }
        }

        void OnSettingsButtonClicked() {
            Debug.LogWarning("Settings Clicked");
        }
    }



    public interface IOsmoEditorVisionHelper {
        TangibleManager tangibleManager { get; }
        Vector2 GetCameraDimens();
    }


    public class OsmoEditorVisionService : IVisionService {
        IOsmoEditorVisionHelper visionHelper;

        public OsmoEditorVisionService(IOsmoEditorVisionHelper visionHelper) {
            this.visionHelper = visionHelper;
        }

        public List<ExtInput> GetVisionObjects() {
            var aliveObjs = visionHelper.tangibleManager.AliveObjects;

            var ret = new List<ExtInput>();
            foreach (var obj in aliveObjs) {

                var pos = GetWorldPos(new Vector2(obj.Location.X, obj.Location.Y));

                if (obj.Id < 10) {
                    for (int i = 0; i < 10; i++) {
                        ret.Add(new ExtInput { id = obj.Id, type = TileType.BLUE_ROD, position = pos + new Vector3(i * 1, 0) });
                    }
                } else {
                    ret.Add(new ExtInput { id = obj.Id, type = TileType.RED_CUBE, position = pos });
                }
            }
            return ret;
        }

        Vector3 GetWorldPos(Vector3 editorPos) {
            var mDimens = CameraUtil.MainDimens();
            var edDimens = visionHelper.GetCameraDimens();
            var x = editorPos.x * (mDimens.x / edDimens.x);
            var y = editorPos.y * (mDimens.y / edDimens.y);

            return new Vector2(x, y);
        }

        public void Init() {

        }
    }
}
#endif