using UnityEngine;
using System.Collections;

namespace ThreeGlasses
{
    public class ThreeGlassesWandBind : MonoBehaviour
    {
        public InputType type = InputType.LeftWand;
        public bool sendToChildren = true;
        public bool updateSelf = true;
        private Vector3 origin;
        public enum UpdateType
        {
            Local,
            World
        };
        public UpdateType updateType = UpdateType.Local;
        private Transform tran;
        // Use this for initialization
        void Start()
        {
            tran = GetComponent<Transform>();
            origin = tran.localPosition;
        }
        // Update is called once per frame
        void LateUpdate()
        {
            if (ThreeGlassesCamera.joyPad[(int)type] == null)
                return;

            ThreeGlassesWand.Wand pack = new ThreeGlassesWand.Wand(ThreeGlassesCamera.joyPad[(int)type].pack);
            if (sendToChildren)
            {
                gameObject.BroadcastMessage("OnWandChange", pack, SendMessageOptions.DontRequireReceiver);
            }
            if (updateSelf)
            {
                if (updateType == UpdateType.Local)
                {
                    tran.localPosition = origin + pack.position;
                    tran.localRotation = pack.rotation;
                }
                else
                {
                    tran.position = origin + pack.position;
                    tran.rotation = pack.rotation;
                }
                
            }
        }
    }
}
