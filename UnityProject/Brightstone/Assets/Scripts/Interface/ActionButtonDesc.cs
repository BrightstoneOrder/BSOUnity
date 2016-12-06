using UnityEngine;
using UnityEngine.UI;
using System;

namespace Brightstone
{
    [Serializable]
    public class ActionButtonDesc : BaseObject
    {
        [SerializeField]
        private Sprite mImage= null;
        [SerializeField]
        private Sprite mPressed= null;
        [SerializeField]
        private Sprite mHighlight = null;
        [SerializeField]
        private Sprite mDisabled = null;
        [SerializeField]
        private Color mOverlayColor = Color.white;
        [SerializeField]
        private string mTooltip = string.Empty;
        [SerializeField]
        private string mDescription = string.Empty;
        

        public Sprite image { get { return mImage; }set { mImage = value; } }
        public Sprite pressed { get { return mPressed; }set { mImage = value; } }
        public Sprite highlight { get { return mHighlight; }set { mHighlight = value; } }
        public Sprite disabled { get { return mDisabled; }set { mDisabled = value; } }
        public Color overlayColor { get { return mOverlayColor; }set { mOverlayColor = value; } }
        public string tooltip { get { return mTooltip; }set { mTooltip = value; } }
        public string description { get { return mDescription; }set { mDescription = value; } }
    }
}

