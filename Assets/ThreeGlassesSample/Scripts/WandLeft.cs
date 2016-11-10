using UnityEngine;
using System.Collections;
using ThreeGlasses;

public class WandLeft : MonoBehaviour {
    private Transform origin;
    public float fireRate = 1.0f;
    private float currRate = 0.0f;

	// Use this for initialization
	void Start () {
        origin = GetComponent<Transform>();
    }
	
	// Update is called once per frame
	void Update () {
        // set transform
        transform.position = origin.position + TGInput.GetPosition(InputType.LeftWand);
        transform.rotation = TGInput.GetRotation(InputType.LeftWand);

        // get strigger process
        origin.localScale =new Vector3(origin.localScale.x, TGInput.GetTriggerProcess(InputType.LeftWand), origin.localScale.z);

        // create a bullet
        currRate += Time.deltaTime;
        if(currRate > fireRate)
        {
            currRate -= fireRate;
            if (TGInput.GetKey(InputType.LeftWand, InputKey.WandTriggerWeak))
            {
                GameObject bullet = new GameObject("bullet");
                bullet.transform.position = transform.position;
                bullet.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                Rigidbody rb = bullet.AddComponent<Rigidbody>();
                bullet.AddComponent<ForDestroy>();
                if (TGInput.GetKey(InputType.LeftWand, InputKey.WandTriggerStrong))
                {
                    rb.AddForce(transform.forward, ForceMode.VelocityChange);
                }
            }
        }
       

        // menu

        // back

        // left side

        // right side
    }
}
