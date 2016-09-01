using UnityEngine;
using System.Collections;
using ThreeGlasses;


namespace ThreeGlasses
{
    /// <summary>
    /// This Class control cube object and the chair  rotate
    /// </summary>
    public class RotateCube : MonoBehaviour
    {
        /// <summary>
        /// Rotate Speed
        /// </summary>
        private float _smooth = 0.5f;
        private float _middlePosition = 2f;
        private float _rotateSpeed = 100f;

        void Start()
        {
            ThreeGlassesInterfaces.IntChair();
        }

        void Update()
        {
            RotateControl();
        }

        /// <summary>
        /// Control Rotate
        /// </summary>
        private void RotateControl()
        {
            Vector3 pos = this.transform.position;
            if (Input.GetKeyDown("up"))
            {
                transform.Rotate(Vector3.left, 5, Space.Self);
                ThreeGlassesInterfaces.ChairCenterMove(transform.localPosition * _rotateSpeed, transform.up);
            }

            if (Input.GetKeyDown("down"))
            {
                transform.Rotate(Vector3.left, -5, Space.Self);
                ThreeGlassesInterfaces.ChairCenterMove(transform.localPosition * _rotateSpeed, transform.up);
            }

            if (Input.GetKeyDown("right"))
            {
                transform.Rotate(Vector3.forward, -5, Space.Self);
                ThreeGlassesInterfaces.ChairCenterMove(transform.localPosition * _rotateSpeed, transform.up);
            }

            if (Input.GetKeyDown("left"))
            {
                transform.Rotate(Vector3.forward, 5, Space.Self);
                ThreeGlassesInterfaces.ChairCenterMove(transform.localPosition * _rotateSpeed, transform.up);
            }
            if (Input.GetKeyDown("w"))
            {
                pos.y += _smooth;
                this.transform.position = pos;
                ThreeGlassesInterfaces.ChairCenterMove(transform.localPosition * _rotateSpeed, transform.up);
            }
            if (Input.GetKeyDown("s"))
            {
                pos.y -= _smooth;
                this.transform.position = pos;
                ThreeGlassesInterfaces.ChairCenterMove(transform.localPosition * _rotateSpeed, transform.up);
            }
            //Set chair to zero
            if (Input.GetKeyDown("r"))
            {
                ThreeGlassesInterfaces.ChairToZero();
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
            // Set chair to middle
            if (Input.GetKeyDown("m"))
            {
                ThreeGlassesInterfaces.ChairToMiddle();
                transform.localPosition = new Vector3(transform.localPosition.x, _middlePosition, transform.localPosition.z);
            }
        }
    }
}