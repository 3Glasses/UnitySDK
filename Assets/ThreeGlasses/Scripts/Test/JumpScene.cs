using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class JumpScene : MonoBehaviour
{
    public string scene;

	void Start () {
        Invoke("Jump", 4.0f);
	}
	
	// Update is called once per frame
	void Jump () {
	    SceneManager.LoadScene(scene);
	}
}
