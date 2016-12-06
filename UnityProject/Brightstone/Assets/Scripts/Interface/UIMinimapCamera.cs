using UnityEngine;

namespace Brightstone
{
    public class UIMinimapCamera : UIBase
    {
        [Tooltip("Render texture used to render the top down scene.")]
        [SerializeField]
        private RenderTexture mRenderTexture = null;

        private Camera mCamera = null;
        void Start()
        {
            InternalInit();
        }
        void OnDestroy()
        {
            InternalDestroy();
        }

        protected override void OnInit()
        {
            base.OnInit();
            mCamera = GetComponent<Camera>();
            if(mCamera == null)
            {
                Log.Game.Error("UIMinimapCamera is expecting a camera but does not have one.");
            }
            else
            {
                mCamera.targetTexture = mRenderTexture;
            }
        }
    }

}

