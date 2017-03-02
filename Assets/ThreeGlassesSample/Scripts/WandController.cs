using UnityEngine;
using System.Collections;
using ThreeGlasses;


public class WandController : MonoBehaviour {
    private Vector3 origin;
    public float fireRate = 1.0f;
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 20f;
    private float currRate = 0.0f;
	public float moveScale = 2.0f;
    public enum UseType
    {
        UseGet = 0,
        UseCallback = 1
    }
    // two way for controll
    public UseType useType = UseType.UseGet;
    public InputType inputType = InputType.LeftWand;
    private int bulletType = 0;
    public float bulletSpeed = 20;

    public Transform firePos;

    private Transform trans;
    private Material mat;
    public Transform headDisplay;
    // Use this for initialization
    void Start () {
        trans = GetComponent<Transform>();
        origin = trans.localPosition;
        mat = GetComponent<Renderer>().material;
    }
	
	// Update is called once per frame
	void Update ()
    {
        ThreeGlassesUtils.Log("HMD's name is " + TGInput.GetHMDName());
        ThreeGlassesUtils.Log("HMD's touchpad " + TGInput.GetHMDTouchPad());
        if (TGInput.GetKey(InputType.HMD, InputKey.HmdMenu))
            ThreeGlassesUtils.Log("HMD's key menu is pressed ");
        if (TGInput.GetKey(InputType.HMD, InputKey.HmdExit))
            ThreeGlassesUtils.Log("HMD's key exit is pressed ");
        for (int i = (int)InputKey.WandMenu; i <= (int)InputKey.WandTriggerStrong; i++)
        {
            if (TGInput.GetKey(InputType.LeftWand, (InputKey)i))
                ThreeGlassesUtils.Log((InputKey)i + " is pressed");
        }
        // by get way
        if (useType == UseType.UseGet)
        {
            // set transform
            transform.localPosition = origin + TGInput.GetPosition(inputType)*moveScale;
            transform.localRotation = TGInput.GetRotation(inputType);

            float intensity = TGInput.GetTriggerProcess(inputType);
            mat.SetColor("_Color", new Color(intensity, intensity, intensity, 1));

            // change bullet type
            if (TGInput.GetKey(inputType, InputKey.WandLeftSide))
            {
                bulletType = (bulletType+3)%4;
            }
            if (TGInput.GetKey(inputType, InputKey.WandRightSide))
            {
                bulletType = (++bulletType) % 4;
            }
            // create a bullet
            currRate += Time.deltaTime;
            if (currRate > fireRate)
            {
                currRate -= fireRate;
                

                if (TGInput.GetKey(inputType, InputKey.WandTriggerStrong))
                {
                    GameObject bullet = GameObject.CreatePrimitive((PrimitiveType)bulletType);
                    bullet.transform.position = firePos.position;
                    bullet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    Rigidbody rb = bullet.AddComponent<Rigidbody>();
                    rb.AddForce(transform.forward*bulletSpeed, ForceMode.VelocityChange);
                    Destroy(bullet, 10.0f);
                }
            }
        }

        // move control
        if(headDisplay != null)
        {
            Vector2 dir = TGInput.GetStick(inputType);
            if (inputType == InputType.LeftWand)
            {
                dir = dir * moveSpeed * Time.deltaTime;
                headDisplay.Translate(new Vector3(dir.x, 0, dir.y));
            }
            else if (inputType == InputType.RightWand)
            {
                headDisplay.Rotate(headDisplay.up, dir.x * rotateSpeed * Time.deltaTime);
                headDisplay.Rotate(headDisplay.right, -dir.y * rotateSpeed * Time.deltaTime);
            }  
        }
    }

    // by callback way
    void OnWandChange(ThreeGlassesWand.Wand pack)
    {
        // you can also get the wand struct info here
        // must bind ThreeGlassesWandBind script

        // it is same as the get version which is in the Update function. just a sample
        if (useType == UseType.UseCallback && inputType == InputType.RightWand)
        {

            float intensity = pack.triggerProcess;
            mat.SetColor("_Color", new Color(intensity, intensity, intensity, 1));
            // create a bullet
            currRate += Time.deltaTime;
            if (currRate > fireRate)
            {
                currRate -= fireRate;


                if (pack.GetKey(InputKey.WandTriggerStrong))
                {
                    GameObject bullet = GameObject.CreatePrimitive((PrimitiveType)bulletType);
                    bullet.transform.position = firePos.position;
                    bullet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    Rigidbody rb = bullet.AddComponent<Rigidbody>();
                    rb.AddForce(transform.forward * bulletSpeed, ForceMode.VelocityChange);
                    Destroy(bullet, 10.0f);
                }
            }
        }
    }
}
