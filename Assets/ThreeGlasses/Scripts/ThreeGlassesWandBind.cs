using UnityEngine;

namespace ThreeGlasses
{
    public class ThreeGlassesWandBind : MonoBehaviour
    {
        public InputType Type = InputType.LeftJoyPad;

        public void LateUpdate()
        {
            var pack = new ThreeGlassesWand.Wand(ThreeGlassesCamera.joyPad[(int)Type].pack);
            transform.localPosition = pack.position;
            transform.localRotation = pack.rotation;

            gameObject.BroadcastMessage("OnWandChange", pack, SendMessageOptions.DontRequireReceiver);
        }
    }
}
