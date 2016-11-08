using UnityEngine;
using System.Collections;
using System.Reflection;

namespace ThreeGlasses
{
    public class ThreeGlassesCamera : MonoBehaviour {
        // 相机
        const int CAMERA_NUM = 2;
        private GameObject[] subCamera = new GameObject[CAMERA_NUM];
        private float near, far;
        private const float fieldOfView = 90;
        private string[] cameraName = new string[]{"leftCamera", "rightCamera"};
        
        // 渲染纹理
        private static RenderTexture[] renderTexture = new RenderTexture[CAMERA_NUM];
        private const int renderWidth = 2880;
        private const int renderHeight = 1440;
        // 视距
        public float eyeDistance = 0.1f;
        public LayerMask layerMask = -1;

        // 是否需要支持手柄
        public bool enableJoypad = true;
        const int JOYPAD_NUM = 2;
        static ThreeGlassesJoypad[] joyPad = new ThreeGlassesJoypad[JOYPAD_NUM];

        // 主相机的显示
        public bool onlyHeadDisplay = false;

        // 是否需要手动翻转
        public bool flipDisplay = false;

        void Awake()
        {
            // 初始化dll
            ThreeGlassesDllInterface.SZVRPluginInit();
            // 初始化渲染纹理
            for (int i = 0; i < CAMERA_NUM; i++)
            {
                renderTexture[i] = new RenderTexture(renderWidth / 2, renderHeight, 24,
                                                     RenderTextureFormat.BGRA32,
                                                     RenderTextureReadWrite.Default);
                renderTexture[i].Create();
            }            
        }
        IEnumerator Start ()
        {
            ThreeGlassesUtils.Log("MainCamera init");
            // 初始化相机
            VRCameraInit();
            if (enableJoypad)
            {
                // 初始化手柄
                ThreeGlassesUtils.Log("init joypad");
                joyPad[0] = new ThreeGlassesJoypad(InputType.LeftJoyPad);
                joyPad[1] = new ThreeGlassesJoypad(InputType.RightJoyPad);
            }

            yield return StartCoroutine("CallPluginAtEndOfFrames");
        }

        public void Update()
        {
            
        }
        
        void VRCameraInit ()
        {
            // 获取原有相机的远近裁剪面
            Camera thiscam = GetComponent<Camera>();
            near = thiscam.nearClipPlane;
            far = thiscam.farClipPlane;

            // 获取要添加的component
            ArrayList needAdd = new ArrayList();
            System.Type[] needAddTypes = new System.Type[] { typeof(GUILayer), typeof(FlareLayer)};
            Component[] coms = gameObject.GetComponents<Component>();
            // 普通component
            foreach (var com in coms)
            {
                foreach(var type in needAddTypes)
                {
                    if(com.GetType() == type)
                    {
                        needAdd.Add(com);
                    }
                }
            }
            // 脚本component
            foreach (var com in coms)
            {
               
                if (com is MonoBehaviour)
                {
                        
                    if (com != this)
                    {
                        System.Type t = com.GetType();


                        MemberInfo meth = t.GetMethod("OnRenderImage", BindingFlags.Instance |
                                                      BindingFlags.NonPublic | BindingFlags.Public);
                        if (meth != null)
                        {
                            needAdd.Add(com);
                        }
                    }
                      
                }
                
            }

            // 创建并设置相机
            for (int i=0; i<CAMERA_NUM; i++)
            {
                // 创建左右相机
                subCamera[i] = new GameObject();
                
                // 重命名，添加ThreeGlassesSubCamera
                subCamera[i].name = cameraName[i];
                Camera cam = subCamera[i].AddComponent<Camera>();
                cam.fieldOfView = fieldOfView;
                cam.nearClipPlane = near;
                cam.farClipPlane = far;
                cam.cullingMask = thiscam.cullingMask;
                cam.depth = thiscam.depth;
                subCamera[i].transform.SetParent(this.transform);

                // 添加主相机中需要copy过来的component
                foreach(var item in needAdd)
                {
                    UnityEditorInternal.ComponentUtility.CopyComponent((Component)item);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(subCamera[i]);
                }

                // 添加完所有component包含ImageEffect后再添加subCamera，因为需要blit翻转
                ThreeGlassesSubCamera subCameraScript = subCamera[i].AddComponent<ThreeGlassesSubCamera>();
                subCameraScript.type = (ThreeGlassesSubCamera.CameraType)i;
                if(!flipDisplay)
                {
                    subCameraScript.FLIP = true;
                }
                

            }
            var eyeDis = new Vector3(eyeDistance/2, 0, 0);
            subCamera[0].transform.localPosition = -eyeDis;
            subCamera[1].transform.localPosition = eyeDis;

            // 对场景下的所有添加了subCamera脚本的相机做设置
            ThreeGlassesSubCamera[] cams = GameObject.FindObjectsOfType(typeof(ThreeGlassesSubCamera)) as ThreeGlassesSubCamera[];
            foreach(var cam in cams)
            {
                Camera tempCamera = cam.gameObject.GetComponent<Camera>();
                if (ThreeGlassesSubCamera.CameraType.Screen == cam.type)
                {
                    tempCamera.targetTexture = null;
                    continue;
                }

                tempCamera.targetTexture = renderTexture[(int)cam.type];
            }

            thiscam.enabled = !onlyHeadDisplay;
        }
        
        private IEnumerator CallPluginAtEndOfFrames()
        {
            while (true)
            {
                // 在所有绘制之后，OnRenderImage之后
                yield return new WaitForEndOfFrame();

                ThreeGlassesDllInterface.UpdateTextureFromUnity(renderTexture[0].GetNativeTexturePtr(),
                                                                renderTexture[1].GetNativeTexturePtr());

                GL.IssuePluginEvent(ThreeGlassesDllInterface.GetRenderEventFunc(), 1);


                // 更新主相机旋转角度
                var input = new float[] { 0, 0, 0, 0 };
                ThreeGlassesDllInterface.GetHMDQuaternion(input);
                input[0] = input[0];
                input[1] = input[1];
                input[2] = input[2];
                input[3] = input[3];
                transform.localRotation = new Quaternion(input[0], input[1], input[2], input[3]);

                // 更新手柄信息
                if (enableJoypad)
                {
                    for (int i = 0; i < JOYPAD_NUM; i++)
                    {
                        joyPad[i].Update();
                    }
                }
            }
        }

        void OnDestroy()
        {
            ThreeGlassesDllInterface.SZVRPluginDestroy();
        }

        // get
        public RenderTexture LeftEyeRT
        {
            get { return renderTexture[0]; }
        }
        public RenderTexture RightEyeRT
        {
            get { return renderTexture[1]; }
        }

        public static bool getJoypadKey(InputType type, InputKey key)
        {
            return joyPad[(int)type].getKey(key);
        }

    }
}
