namespace Brightstone
{
    public class UnitAbilityActionHandler : UIActionHandler
    {
        public UnitAbility ability { get; set; }

        public override void OnAction(UIAction action, UIBase sender)
        {
            if(ability != null && action == UIAction.UA_CLICK)
            {
                ability.Activate();
            }
        }
    }

}