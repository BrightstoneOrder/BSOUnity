using UnityEngine;

namespace Brightstone
{
    public class HoverEffect : Actor
    {

        [SerializeField]
        private float mHoverHeight = 0.0f;
        [SerializeField]
        private float mRotationSpeed = 0.0f;
        [SerializeField]
        private Vector3 mRotation = Vector3.zero;

        private Vector3 mHoverPosition = Vector3.zero;
        

        private void Start()
        {
            InternalInit();
        }

        private void OnDestroy()
        {
            InternalDestroy();
        }


        protected override void OnInit()
        {
            base.OnInit();
            mHoverPosition = GetPosition();
        }

        private void Update()
        {
            float time = mWorld.GetTime();
            Quaternion rotation = GetRotation();
            Vector3 rotationDirection = mRotation * mRotationSpeed * mWorld.GetGameDelta();
            GetTransform().Rotate(rotationDirection);
            SetPosition(mHoverPosition + new Vector3(0.0f, Mathf.Sin(time) * mHoverHeight, 0.0f));
        }

    }

}

