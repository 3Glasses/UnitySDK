using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ThreeGlasses
{

    public class ThreeGlassesMenuItem {

        
        const string kEnableHeadDisplay = "3Glasses/EnableHeadDisplay";

        [InitializeOnLoad]
        class HierarchyChanged
        {
            static HierarchyChanged ()
            {
                EditorApplication.hierarchyWindowChanged += Update;
            }
            static void Update ()
            {
                
                // 检测是否有使用3glasses sdk
                if (GameObject.FindObjectOfType(typeof(ThreeGlassesCamera)) != null)
                {
                    ThreeGlassesUtils.Log("ThreeGlassesCamera find");
                    Menu.SetChecked(kEnableHeadDisplay, true);   
                }
                else
                {
                    ThreeGlassesUtils.Log("no ThreeGlassesCamera find");
                    Menu.SetChecked(kEnableHeadDisplay, false);
                }
                
            }
        }
        
		[MenuItem(kEnableHeadDisplay)]
		public static void ToggleSimulationMode ()
		{
            if(Menu.GetChecked(kEnableHeadDisplay))
            {
                Menu.SetChecked(kEnableHeadDisplay, false);
                clear3Glasses();
            }
            else
            {
                clear3Glasses();
                add3Glasses();
                Menu.SetChecked(kEnableHeadDisplay, true);
            }
		}
        public static void add3Glasses()
        {
            // 加到最先绘制的相机上，一般此为主相机
            Camera[] cams = GameObject.FindObjectsOfType<Camera>();
            float depth = 0;
            Camera cam;
            if(cams.Length > 0)
            {
                depth = cams[0].depth;
                cam = cams[0];
                foreach (var item in cams)
                {
                    if(item.depth < depth)
                    {
                        depth = item.depth;
                        cam = item;
                    }
                }
            }
            else
            {
                GameObject mainCamera = new GameObject("MainCamera");
                cam = mainCamera.AddComponent<Camera>();
            }

            // 将脚本附加到选定的cam上
            cam.gameObject.AddComponent<ThreeGlassesCamera>();
            Selection.activeGameObject = cam.gameObject;
        }
        public static void clear3Glasses()
        {
            // 删除主相机上的管理类
            Component[] objects = GameObject.FindObjectsOfType<ThreeGlassesCamera>();
            foreach (var item in objects)
            {
                GameObject.DestroyImmediate(item);
            }
            // 删除用户自己绑定的相机类
            objects = GameObject.FindObjectsOfType<ThreeGlassesSubCamera>();
            foreach (var item in objects)
            {
                GameObject.DestroyImmediate(item);
            }
        }
    }
}
