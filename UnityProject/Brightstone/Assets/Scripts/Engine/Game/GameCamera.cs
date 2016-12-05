using UnityEngine;

namespace Brightstone
{
    public class GameCamera : Actor
    {
        // TODO: Clamping
        //private Vector3 mMinBounds = Vector3.zero;
        //private Vector3 mMaxBounds = Vector3.zero; 
        [SerializeField]
        private float mMoveSpeed = 5.0f;
        [SerializeField]
        private float mSmooth = 1.0f;
        [SerializeField]
        private float mMaxSpeed = 5.0f;

        private Vector3 mPositionDelta = Vector3.zero;
        private Vector3 mMoveVelocity = Vector3.zero;

        private void Start()
        {
            InternalInit();
        }

        private void OnDestroy()
        {
            InternalDestroy();
        }

        private void Update()
        {
            Vector3 current = GetPosition();
            Vector3 input = new Vector3(
                mWorld.GetInputMgr().PeekAxisValue(InputCode.IC_PLAYER_MOVE_HORIZONTAL),
                0.0f,
                mWorld.GetInputMgr().PeekAxisValue(InputCode.IC_PLAYER_MOVE_VERTICAL));

            input.Normalize();

            Vector3 euler = GetRotation().eulerAngles;
            euler.x = 0.0f;
            Quaternion quat = Quaternion.Euler(euler);

            input = quat * input;

            input *= mMoveSpeed;

            Vector3 result = Vector3.SmoothDamp(current, current + input, ref mMoveVelocity, mSmooth, mMaxSpeed, mWorld.GetGameDelta());
            SetPosition(result);
        }

    }

}

