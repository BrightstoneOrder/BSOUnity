using UnityEngine;
using System.Collections;

namespace Brightstone
{
    public class KinematicBox : MonoBehaviour
    {
        public enum DrawType
        {
            None,
            Fill,
            Wire
        }
        // private static readonly Vector3 PLANE_SIZE = new Vector3(10.0f, 0.01f, 10.0f);
        public float mDesiredHeight = 0.0f;
        public float mGravity = 9.81f;
        public float mViscosity = 6.0f;
        public bool mDebugIsBelow = false;
        public DrawType mDebugDraw = DrawType.None;

        public Vector3 mVelocity = Vector3.zero;
        

        

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            UpdateSimulation();
            DebugDraw();
        }

        void UpdateSimulation()
        {
            // if below desired height.. add force of velocity * 1 * viscosity
            Vector3 position = transform.position;
            Vector3 velocity = mVelocity;
            if (position.y < mDesiredHeight)
            {
                mVelocity += velocity * mViscosity * Time.fixedDeltaTime;
            }
            else
            {
                mVelocity -= Vector3.up * mGravity * Time.fixedDeltaTime;
            }
            transform.position += mVelocity * Time.fixedDeltaTime;
        }

        void DebugDraw()
        {
            Vector3 position = transform.position;
            if(mDebugDraw == DrawType.Fill)
            {
                //Utils.DrawCube(new Vector3(position.x, mDesiredHeight, position.z), PLANE_SIZE, new Color(153.0f/255.0f, 15.0f/255.0f, 20.0f/255.0f,1.0f));
            }
            else if(mDebugDraw == DrawType.Wire)
            {
                //Utils.DrawWireCube(new Vector3(position.x, mDesiredHeight, position.z), PLANE_SIZE, new Color(153.0f/255.0f, 15.0f/255.0f, 20.0f/255.0f));
            }
            mDebugIsBelow = position.y < mDesiredHeight;
        }
    }
}

