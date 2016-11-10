using UnityEngine;
using System.Collections;

namespace ThreeGlasses
{
    public class ThreeGlassesWandBind : MonoBehaviour
    {
        public InputType type = InputType.LeftJoyPad;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void LateUpdate()
        {
            ThreeGlassesWand.Wand pack = new ThreeGlassesWand.Wand(ThreeGlassesCamera.joyPad[(int)type].pack);
            gameObject.BroadcastMessage("OnWandChange", pack, SendMessageOptions.DontRequireReceiver);
        }


    }
}
