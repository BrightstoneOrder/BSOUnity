using UnityEngine;
using System.Collections;

namespace Brightstone
{
    /**
     * This type of motion controller will take input from InputMgr and translate that into movement for the character.
     * Movement is based on camera orientation.
     * IC_PLAYER_MOVE_VERTICAL - This will move the player forwards / backwards. (Default W / S key)
     * IC_PLAYER_MOVE_HORIZONTAL - This will move the player left / right. (Default A / D key)
     * If 'Click to Move' option is enabled then
     */
    public class ActionRPGMotionController : SubComponent
    {
        private Option mClickToMove = null;
        private GameObject mClickEffect = null;
        
        private void CreateEffect(ref InputEventData eventData)
        {
            if(eventData.press)
            {
                if (mClickEffect != null && World.ActiveWorld.GetInputMgr().NativeGetButtonState(InputButton.IB_LEFT))
                {
                    InputMouseData mouseData = World.ActiveWorld.GetInputMgr().GetMouseData();
                    Instantiate(mClickEffect, mouseData.worldPosition, Quaternion.identity);
                }
            }
        }

        protected override void OnInit()
        {
            mClickToMove = World.ActiveWorld.GetOptionsMgr().GetOption(OptionName.ON_CLICK_TO_MOVE);
            if(mClickToMove.GetOptionType() != OptionType.OT_BOOL)
            {
                Log.Game.Error("ActionRPGMotionController expects " + mClickToMove.GetName() + " option to be a click to move.");
            }
            InputMgr inputMgr = World.ActiveWorld.GetInputMgr();
            inputMgr.RegisterCallback(CreateEffect, InputCode.IC_PLAYER_CLICK);
        }

        private void Start()
        {
            InternalInit();
            mClickEffect = Resources.Load("Game/Effects/ClickEffect", typeof(GameObject)) as GameObject;
            if(mClickEffect == null)
            {
                Log.Game.Error("Failed to load click effect!");
            }
        }

        private void Update()
        {
            
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            if(mClickToMove.GetBoolValue())
            {
                UpdateClickToMove(delta);
            }
            else
            {
                UpdateControllerMove(delta);
            }
        }

        private void UpdateClickToMove(float delta)
        {

        }

        private void UpdateControllerMove(float delta)
        {

        }
    }
}


