using UnityEngine;
using System.Collections;
using ThreeGlasses;

public class JoyPadTest : MonoBehaviour {
	// Update is called once per frame
	void Update ()
    {
//         for (int i = 0; i < (int)InputKey.InputNum; i++)
//         {
//             for (int j = 0; j < 2; j++)
//             {
//                 InputType type = (InputType)j;
//                 InputKey key = (InputKey)i;
//                 bool keystatus = TGInput.GetKey(type, key);
//                 ThreeGlassesUtils.Log("type=" + (InputType)j + "         "+(InputKey)i + "keypress"
//                                       + "         trigger process=" + TGInput.GetTriggerProcess(type)
//                                       + "         stick=" + TGInput.GetStick(type));
//             }   
//         }

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
