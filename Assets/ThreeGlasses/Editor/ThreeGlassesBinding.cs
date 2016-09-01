using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedMember.Local


namespace ThreeGlasses
{
    public class ThreeGlassesBinding : ScriptableWizard
    {
        private static Texture2D logo;

        private static bool headBinding;
        private static bool LWandBinding;
        private static bool RWandBinding;

        private static GameObject s_headObj;
        private static GameObject s_leftWandObj;
        private static GameObject s_rightWandObj;

        private GameObject headObj;
        private GameObject leftWandObj;
        private GameObject rightWandObj;

        static string showMsg = "";
        static string showErrorMsg = "";

        static ThreeGlassesBinding self;

        [InitializeOnLoad]
        static class Listen
        {
            private static string currentScene;
            static Listen()
            {
                logo = Resources.Load<Texture2D>(ThreeGlassesConst.ImageResourcesPath + "logo");
                currentScene = SceneManager.GetActiveScene().path;
                EditorApplication.hierarchyWindowChanged += Update;
            }

            static void Update()
            {
                var nowScne = SceneManager.GetActiveScene().path;
                if (currentScene == nowScne) return;
                currentScene = nowScne;

                findBindCamera();

                if (!headBinding)
                {
                    Init();
                }
            }
        }

        [MenuItem("3Glasses/Binding GameObject")]
        static void Init()
        {
            findBindCamera();
            findBindWand();

            showMsg = "";
            showErrorMsg = "";

            if (self == null) {
                self = DisplayWizard<ThreeGlassesBinding>("Binding GameObject");
            }
            self.Show();
        }

        static void findBindCamera()
        {
            s_headObj = null;
            foreach (var c in Camera.allCameras)
            {
                var _vrc = c.transform.root.GetComponentInChildren<ThreeGlassesHeadset>();
                if (_vrc == null) continue;
                s_headObj = _vrc.gameObject;
                break;
            }

            headBinding = s_headObj != null;
        }

        static void findBindWand()
        {
            LWandBinding = false;
            RWandBinding = false;

            var wands = FindObjectsOfType<ThreeGlassesWand>();

            foreach (var w in wands)
            {
                if(w.LeftOrRight == ThreeGlassesInterfaces.LeftOrRight.Left)
                {
                    s_leftWandObj = w.gameObject;
                }
                if (w.LeftOrRight == ThreeGlassesInterfaces.LeftOrRight.Right)
                {
                    s_rightWandObj = w.gameObject;
                }
            }

            LWandBinding = s_leftWandObj != null;

            RWandBinding = s_rightWandObj != null;
        }

        void OnGUI()
        {
            if (headObj == null)
            {
                headObj = Selection.activeGameObject;
            }

            var r = EditorGUILayout.BeginVertical(GUILayout.Height(128));
            EditorGUILayout.Space();

            GUI.DrawTexture(r, logo, ScaleMode.ScaleToFit);

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("SDK Version: " + ThreeGlassesInterfaces.getVersion, EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Bind Head Object:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            if (!headBinding)
            {
                headObj = (GameObject)EditorGUILayout.ObjectField(headObj, typeof(GameObject), true);
                if (s_headObj != null && headObj != null)
                {
                    if (s_headObj.GetInstanceID() == headObj.GetInstanceID())
                    {
                        headBinding = true;
                    }
                }

                if (GUILayout.Button("Bind", EditorStyles.miniButton))
                {
                    UnbindCamera();
                    BindCamera(headObj);

                    showMsg = "Binding camera is successful \n";
                    foreach (var c in Camera.allCameras)
                    {
                        var vrc = c.gameObject.GetComponent<ThreeGlassesVRCamera>();
                        if(vrc == null && c.targetTexture == null)
                        {
                            showErrorMsg = "Warning: multiple camera on the scene\n";
                            Debug.LogError(showErrorMsg);
                        }
                    }
                }
            }
            else
            {
                headObj = (GameObject)EditorGUILayout.ObjectField(s_headObj, typeof(GameObject), true);
                if (s_headObj != null && headObj != null)
                {
                    if (s_headObj.GetInstanceID() != headObj.GetInstanceID())
                    {
                        headBinding = false;
                    }
                }

                if (GUILayout.Button("Unbind", EditorStyles.miniButton))
                {
                    UnbindCamera();
                    showMsg = "Unbinding is successful \nDisable LeftCamera and RightCamera GameObject\n";
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Left Wand
            EditorGUILayout.LabelField("Bind Left Wand Object:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (!LWandBinding)
            {
                leftWandObj = (GameObject)EditorGUILayout.ObjectField(leftWandObj, typeof(GameObject), true);
                if (s_leftWandObj != null && leftWandObj != null)
                {
                    if (s_leftWandObj.GetInstanceID() == leftWandObj.GetInstanceID())
                    {
                        LWandBinding = true;
                    }
                }

                if (GUILayout.Button("Bind", EditorStyles.miniButton))
                {
                    UnbindWand(s_leftWandObj);
                    BindWand(leftWandObj, ThreeGlassesInterfaces.LeftOrRight.Left);
                    s_leftWandObj = leftWandObj;
                    LWandBinding = true;
                    showMsg = "Binding left wand is successful \n";
                }
            }
            else
            {
                leftWandObj = (GameObject)EditorGUILayout.ObjectField(s_leftWandObj, typeof(GameObject), true);
                if (s_leftWandObj != null && leftWandObj != null)
                {
                    if (s_leftWandObj.GetInstanceID() != leftWandObj.GetInstanceID())
                    {
                        LWandBinding = false;
                    }
                }

                if (GUILayout.Button("Unbind", EditorStyles.miniButton))
                {
                    UnbindWand(s_leftWandObj);
                    s_leftWandObj = null;
                    LWandBinding = false;
                    showMsg = "Unbinding left wand is successful \n";
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Right Wand
            EditorGUILayout.LabelField("Bind Right Wand Object:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (!RWandBinding)
            {
                rightWandObj = (GameObject)EditorGUILayout.ObjectField(rightWandObj, typeof(GameObject), true);
                if (s_rightWandObj != null && rightWandObj != null)
                {
                    if (s_rightWandObj.GetInstanceID() == rightWandObj.GetInstanceID())
                    {
                        RWandBinding = true;
                    }
                }

                if (GUILayout.Button("Bind", EditorStyles.miniButton))
                {
                    UnbindWand(s_rightWandObj);
                    BindWand(rightWandObj, ThreeGlassesInterfaces.LeftOrRight.Right);
                    s_rightWandObj = rightWandObj;
                    RWandBinding = true;
                    showMsg = "Binding right wand is successful \n";
                }
            }
            else
            {
                rightWandObj = (GameObject)EditorGUILayout.ObjectField(s_rightWandObj, typeof(GameObject), true);
                if (s_rightWandObj != null && rightWandObj != null)
                {
                    if (s_rightWandObj.GetInstanceID() != rightWandObj.GetInstanceID())
                    {
                        RWandBinding = false;
                    }
                }

                if (GUILayout.Button("Unbind", EditorStyles.miniButton))
                {
                    UnbindWand(s_rightWandObj);
                    s_rightWandObj = null;
                    RWandBinding = false;
                    showMsg = "Unbinding right wand is successful \n";
                }
            }
            EditorGUILayout.EndHorizontal();

            var msgStyles = new GUIStyle(EditorStyles.boldLabel)
            {
                wordWrap = false,
                clipping = TextClipping.Overflow
            };

            EditorGUILayout.Space();
            msgStyles.normal.textColor = Color.green;
            EditorGUILayout.LabelField(showMsg, msgStyles);

            EditorGUILayout.Space();

            EditorGUILayout.Space();
            msgStyles.normal.textColor = Color.red;
            EditorGUILayout.LabelField(showErrorMsg, msgStyles);
        }

        void BindCamera(GameObject obj)
        {
            if (obj != null)
            {
                var headset = obj.AddComponent<ThreeGlassesHeadset>();
                obj.AddComponent<ThreeGlassesEvents>();
                obj.AddComponent<AudioListener>();

                var leftCameraObj = new GameObject {name = ThreeGlassesConst.DefaultLeftCameraName};
                leftCameraObj.transform.parent = obj.transform;
                leftCameraObj.transform.localPosition = Vector3.zero;
                leftCameraObj.transform.localRotation = Quaternion.identity;

                var rightCameraObj = new GameObject {name = ThreeGlassesConst.DefaultRightCameraName};
                rightCameraObj.transform.parent = obj.transform;
                rightCameraObj.transform.localPosition = Vector3.zero;
                rightCameraObj.transform.localRotation = Quaternion.identity;

                leftCameraObj.AddComponent<Camera>();
                leftCameraObj.AddComponent<FlareLayer>();
                headset.leftCamera = leftCameraObj.AddComponent<ThreeGlassesVRCamera>();

                rightCameraObj.AddComponent<Camera>();
                rightCameraObj.AddComponent<FlareLayer>();
                headset.rightCamera = rightCameraObj.AddComponent<ThreeGlassesVRCamera>();

                headset.SetCameraPos();

                s_headObj = obj;
                headBinding = true;
            }
        }

        void UnbindCamera()
        {
            if (s_headObj != null)
            {
                foreach (var h in s_headObj.GetComponents<ThreeGlassesHeadset>())
                {
                    DestroyImmediate(h);
                }

                foreach (var e in s_headObj.GetComponents<ThreeGlassesEvents>())
                {
                    DestroyImmediate(e);
                }

                foreach (var a in s_headObj.GetComponents<AudioListener>())
                {
                    DestroyImmediate(a);
                }

                foreach (var c in s_headObj.GetComponentsInChildren<ThreeGlassesVRCamera>())
                {
                    c.gameObject.SetActive(false);
                    DestroyImmediate(c);
                }

                s_headObj = null;
                headBinding = false;
            }
        }

        void BindWand(GameObject obj, ThreeGlassesInterfaces.LeftOrRight LR)
        {
            if (obj != null)
            {
                var wand = obj.AddComponent<ThreeGlassesWand>();
                wand.LeftOrRight = LR;
            }
        }

        void UnbindWand(GameObject obj)
        {
            if (obj != null)
            {
                var wands = obj.GetComponents<ThreeGlassesWand>();
                foreach (var w in wands)
                {
                    DestroyImmediate(w);
                }
            }
        }
    }
}