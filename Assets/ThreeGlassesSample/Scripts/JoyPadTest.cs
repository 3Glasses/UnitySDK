using UnityEngine;
using System.Collections;
using ThreeGlasses;

public class JoyPadTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < (int)InputKey.InputNum; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                InputType type = (InputType)j;
                InputKey key = (InputKey)i;
                bool keystatus = TGInput.GetKey(type, key);
                ThreeGlassesUtils.Log("type=" + (InputType)j + "         "+(InputKey)i + "keypress"
                                      + "         trigger process=" + TGInput.GetTriggerProcess(type)
                                      + "         stick=" + TGInput.GetStick(type));
            }   
        }
    }

    void OnWandChange(ThreeGlassesJoypad.Wand pack)
    {
        ThreeGlassesUtils.Log("wand=" + pack);
    }
}
