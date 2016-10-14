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
        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);

            //Option mClickToMove = World.ActiveWorld.GetOptionMgr().FindOption(OptionType.OT_CLICK_TO_MOVE);
        }
    }
}


