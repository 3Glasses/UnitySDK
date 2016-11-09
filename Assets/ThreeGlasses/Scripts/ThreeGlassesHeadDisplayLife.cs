using UnityEngine;
using System.Collections;

namespace ThreeGlasses
{ 
    public class ThreeGlassesHeadDisplayLife : MonoBehaviour {

	    // Use this for initialization
	    void Awake () {
            ThreeGlassesUtils.Log("ThreeGlassesHeadDisplayLife init");
            ThreeGlassesDllInterface.SZVRPluginInit();
        }
	
	    // Update is called once per frame
	    void OnDestroy() {
            ThreeGlassesUtils.Log("ThreeGlassesHeadDisplayLife destroy");
            ThreeGlassesDllInterface.SZVRPluginDestroy();        
        }
    }
}