using UnityEngine.UI;
using UnityEngine;

namespace Brightstone
{
    public class UIImage : UIBase
    {

        private RawImage mImage = null;

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
            mImage = GetComponent<RawImage>();
        }

        public override UIElement GetElementType()
        {
            return UIElement.UE_IMAGE;
        }

        public void SetTexture(Texture texture)
        {
            mImage.texture = texture;
        }

    }
}
