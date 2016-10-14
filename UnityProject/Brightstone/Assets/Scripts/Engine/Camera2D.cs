using UnityEngine;

namespace Brightstone
{
    public class Camera2D : BaseCamera
    {
        [SerializeField]
        private float mSpeed = 3.0f;
           
        private void Start()
        {
            InternalInit();
        }

        protected override void OnInit()
        {
            
            
        }

        private void Update()
        {
            InputMgr inputMgr = World.ActiveWorld.GetInputMgr();
            if(inputMgr != null)
            {
                float hMove = inputMgr.PeekAxisValue(InputCode.IC_PLAYER_MOVE_HORIZONTAL);
                float vMove = inputMgr.PeekAxisValue(InputCode.IC_PLAYER_MOVE_VERTICAL);

                Move(hMove * mSpeed, vMove * mSpeed);
            }
        }

        private void Move(float x, float y)
        {
            GetTransform().position += new Vector3(x, y, 0.0f);
        }

    }
}
