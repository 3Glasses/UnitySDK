using UnityEngine;
using System.Collections;
using ThreeGlasses;

public class JoyPadTest : MonoBehaviour {
    public float rate = 1.0f;
    private float currRate = 0.0f;
	// Update is called once per frame
	void Update ()
    {
        currRate += Time.deltaTime;
        if(currRate > rate)
        {
            currRate = 0.0f;
            int wandKeyNum = InputKey.WandTriggerStrong - InputKey.WandMenu + 1;
            for (int i = 0; i < 2; i++)
            {
                InputType type = (InputType)i;
                for (int j = 0; j < (int)wandKeyNum; j++)
                {

                    InputKey key = (InputKey)j;
                    bool keystatus = TGInput.GetKey(type, key);
                    if (keystatus)
                    {
                        ThreeGlassesUtils.Log("type=" + (InputType)i + "key=" + key);
                    }
                }

                //                 ThreeGlassesUtils.Log("type=" + (InputType)i
                //                                           + "         trigger process=" + TGInput.GetTriggerProcess(type)
                //                                           + "         stick=" + TGInput.GetStick(type));
                ThreeGlassesUtils.Log("type=" + (InputType)i + "         trigger position=" + TGInput.GetPosition(type) + "    rotation" + TGInput.GetRotation(type));
            }
        }

        //transform.position = TGInput.GetPosition(InputType.LeftWand);
        transform.rotation = TGInput.GetRotation(InputType.RightWand);
    }

    void OnWandChange(ThreeGlassesWand.Wand pack)
    {
        // you can also get the wand struct info here
        // must bind ThreeGlassesWandBind script
        ThreeGlassesUtils.Log("wand=" + pack);
    }
}
