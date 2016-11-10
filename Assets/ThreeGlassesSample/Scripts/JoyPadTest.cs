using UnityEngine;
using System.Collections;
using ThreeGlasses;

public class JoyPadTest : MonoBehaviour {
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < 2; i++)
        {
            InputType type = (InputType)i;
            for (int j = 0; j < (int)InputKey.InputNum; j++)
            {
                
                InputKey key = (InputKey)j;
                bool keystatus = TGInput.GetKey(type, key);
                if(keystatus)
                {
                    //ThreeGlassesUtils.Log("type=" + (InputType)i + "key=" + key);
                }
            }

            ThreeGlassesUtils.Log("type=" + (InputType)i
                                      +"         trigger process=" + TGInput.GetTriggerProcess(type)
                                      + "         stick=" + TGInput.GetStick(type));
        }

        

        // left wand controll all cube's transform



        // right wand controll all cube's rotation
    }

    void OnWandChange(ThreeGlassesWand.Wand pack)
    {
        // you can also get the wand struct info here
        // must bind ThreeGlassesWandBind script
        ThreeGlassesUtils.Log("wand=" + pack);
    }
}
