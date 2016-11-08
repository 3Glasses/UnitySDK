using UnityEngine;
using System.Collections;
using ThreeGlasses;

public class JoyPadTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(TGInput.GetKey(InputType.LeftJoyPad, InputKey.Fire))
        {
            // do something
        }
    }
}
