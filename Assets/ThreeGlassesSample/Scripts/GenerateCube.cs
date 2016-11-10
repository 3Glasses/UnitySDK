using UnityEngine;
using System.Collections;

public class GenerateCube : MonoBehaviour {
    // cube每三个方向的数量和间距
    public int Num = 10;
    public int distance = 5;
    // 箱子使用默认大小1
    private int cubeSize = 1;

    // 临时材质
    public Material red, green; 
    
    void Awake()
    {
        float step = distance + cubeSize;
        float origin = -(step * Num - distance) / 2.0f;

        for (int x = 0; x < Num; x++ )
        {
            for (int y = 0; y < Num; y++)
            {
                for (int z = 0; z < Num; z++)
                {
                    Transform cube = GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<Transform>();
                    cube.position = new Vector3(origin + x * step, origin + y * step, origin + z * step);
                    cube.rotation = Quaternion.identity;
                    cube.parent = gameObject.transform;
                }
            }
        }
    }
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
        }
    }
}
