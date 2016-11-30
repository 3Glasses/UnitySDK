using UnityEngine;
using System.Collections;

namespace ThreeGlasses
{ 
    public class ThreeGlassesHeadDisplayLife : MonoBehaviour
    {
	    void Awake ()
        {
            ThreeGlassesUtils.Log("ThreeGlassesHeadDisplayLife init");
            ThreeGlassesDllInterface.SZVRPluginInit();
        }

        void OnApplicationQuit()
        {
            ThreeGlassesUtils.Log("ThreeGlassesHeadDisplayLife application quit");
            ThreeGlassesDllInterface.SZVRPluginDestroy();
        }
    }
}