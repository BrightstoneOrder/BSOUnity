using UnityEngine;
namespace Brightstone
{
    public class UnitAbility : BaseObject
    {
        [SerializeField]
        private ActionButtonDesc mActionButtonDesc = null;
        [SerializeField]
        private int mButtonIndex = Util.INVALID_INT;

        private UIActionHandler mUIHandler = null;

        public void OnInit()
        {
            UnitAbilityActionHandler handler = new UnitAbilityActionHandler();
            handler.ability = this;
            mUIHandler = handler;
        }

        public override void Serialize(BaseStream stream)
        {
            base.Serialize(stream);
            stream.SerializeObject("ActionButtonDesc", mActionButtonDesc);
            stream.SerializeInt("ButtonIndex", ref mButtonIndex);
        }

        public ActionButtonDesc GetButtonDesc()
        {
            return mActionButtonDesc;
        }

        public int GetButtonIndex()
        {
            return mButtonIndex;
        }

        public UIActionHandler GetUIHandler()
        {
            return mUIHandler;
        }

        public void Activate()
        {

        }
    }
}

