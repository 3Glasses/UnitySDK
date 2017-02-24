using UnityEngine;
using System.Collections;
using UnityEditor;

public class ThreeGlassesAbout : EditorWindow
{
	private string version = "7.1.0";
	private string url = "https://github.com/3Glasses/UnitySDK";
    private Texture texture;

    void Awake()
    {
        texture = Resources.Load("ThreeGlasses/Image/eye") as Texture;
    }
    
    [UnityEditor.MenuItem("3Glasses/About", false, 2)]
    static void Init()
    {
        Rect wr = new Rect(0, 0, 400, 150);
		ThreeGlassesAbout window = (ThreeGlassesAbout)EditorWindow.GetWindowWithRect (typeof(ThreeGlassesAbout),
                                                                              wr, true, "About SDK");
		window.Show ();
    }

    void OnGUI ()
    {
		EditorGUILayout.BeginVertical ();
		Rect rc = new Rect (30, 10, 100, 60);
		GUI.DrawTexture (rc, texture);
		rc.y = 70;
		rc.x = 80;
		GUI.Label (rc, "Version: " + version);
		rc.y += 20;
		rc.width = 500;
		GUIStyle custom = new GUIStyle();
		
		custom.hover.textColor = new Color (0, 0, 0.5f);
		custom.normal.textColor = new Color (1, 1, 1);
		custom.richText = true;
		if(GUI.Button(rc, url, custom))
		{
			Help.BrowseURL (url);
		}

		EditorGUILayout.EndVertical ();
	}
}
