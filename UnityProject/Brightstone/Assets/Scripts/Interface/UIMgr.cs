using UnityEngine;
using System.Collections.Generic;

namespace Brightstone
{
    public class UIMgr : BaseObject
    {
        private List<UIBase>[] mElements = new List<UIBase>[Util.GetEnumCount<UIElement>()];

        public void Init()
        {
            for(int i = 0; i < mElements.Length; ++i)
            {
                mElements[i] = new List<UIBase>();
            }
        }

        public void Update(World world)
        {
            if (world.GetInputMgr().NativeGetKeyState(KeyCode.Alpha1))
            {
                Log.Game.Info("HideUI!");
                HideUI();
            }
            if (world.GetInputMgr().NativeGetKeyState(KeyCode.Alpha2))
            {
                Log.Game.Info("ShowUI!");
                ShowUI();
            }
        }

        public void Register(UIBase actor)
        {
            UIElement element = actor.GetElementType();
            int elementIndex = (int)element;
            mElements[elementIndex].Add(actor);
        }

        public void Unregister(UIBase actor)
        {
            UIElement element = actor.GetElementType();
            int elementIndex = (int)element;
            mElements[elementIndex].Remove(actor);
        }

        // Search by name
        public UIBase FindElement(string name)
        {
            int size = mElements.Length;
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0, count = mElements[i].Count; j < count; ++i)
                {
                    if (mElements[i][j].GetElementName() == name)
                    {
                        return mElements[i][j];
                    }
                }
            }
            return null;
        }

        public UIBase FindElement(UIElement element, string name)
        {
            List<UIBase> elements = mElements[(int)element];
            int size = elements.Count;
            for (int i = 0; i < size; ++i)
            {
                if (elements[i].GetElementName() == name)
                {
                    return elements[i];
                }
            }
            return null;
        }

        public UIBase GetFirstElement(UIElement element)
        {
            if (mElements[(int)element].Count > 0)
            {
                return mElements[(int)element][0];
            }
            return null;
        }

        public UIBase GetElement(UIElement element, int index)
        {
            if (index >= 0 && index < mElements[(int)element].Count)
            {
                return mElements[(int)element][index];
            }
            return null;
        }

        public void ShowUI()
        {
            for(int i = 0, iSize = mElements.Length; i < iSize; ++i)
            {
                for(int j = 0, jSize = mElements[i].Count; j < jSize; ++j)
                {
                    UIBase actor = mElements[i][j];
                    if(actor != null)
                    {
                        actor.Show();
                    }
                }
            }
        }

        public void HideUI()
        {
            for (int i = 0, iSize = mElements.Length; i < iSize; ++i)
            {
                for (int j = 0, jSize = mElements[i].Count; j < jSize; ++j)
                {
                    UIBase actor = mElements[i][j];
                    if (actor != null)
                    {
                        actor.Hide();
                    }
                }
            }
        }
        
        public void OnUnitSelected(Unit unit)
        {
            UIActionBar bar = (UIActionBar)GetFirstElement(UIElement.UE_ACTION_BAR);
            if(bar == null)
            {
                return;
            }
            bar.Clear();
            List<UnitAbility> abilities = unit.GetAbilities();
            List<UnitAbility> unregistered = new List<UnitAbility>();
            // Try and register abilities:
            for (int i = 0; i < abilities.Count; ++i)
            {
                int buttonIndex = abilities[i].GetButtonIndex();
                if(Util.Invalid(buttonIndex)) 
                {
                    continue;
                }
                UnitAbility ability = abilities[i];
                if (!bar.IsButtonFree(buttonIndex))
                {
                    unregistered.Add(ability);
                    continue;
                }
                bar.SetupButton(buttonIndex, ability.GetButtonDesc(), ability.GetUIHandler());
            }
            // Try again but this time find any spot.
            for(int i = 0; i < unregistered.Count; ++i)
            {
                int freeIndex = bar.GetFreeButtonIndex();
                if(Util.Invalid(freeIndex))
                {
                    break; // Nothing left..
                }
                UnitAbility ability = abilities[i];
                bar.SetupButton(freeIndex, ability.GetButtonDesc(), ability.GetUIHandler());
            }
        }

        // Theoretical code.
        private void Example()
        {
            // UIMgr ui = this;
            // 
            // ui.FindElement("ActionBar[0]"); // Find "ActionBar" .. return actionBar.GetElement(0)
            // ui.RegisterClickEvent(element, target);
            // 
            // // OnUnitSelected
            // UIActionButtonBar bar = ui.FindElement("ActionBar");
            // bar.ConfigureForUnit();
            // List<UnitAbility> abilities = unit.GetAbilities();
            // List<UnitAbility> nonIndexAbilities = new List<UnitAbility>();
            // for(int i = 0; i < abilities.Count; ++i)
            // {
            //     int buttonIndex = abilities[i].GetButtonIndex();
            //     if (Util.Invalid(buttonIndex))
            //     {
            //         nonIndexAbilities.Add(abilities[i]);
            //         continue;
            //     }
            //     ActionButtonDesc desc = abilities[i].GetButtonDesc();
            //     InputCode hotKey = abilities[i].GetHotkey();
            //     bar.SetButton(buttonIndex, desc, hotKey);
            //     ui.RegisterAction(bar.GetButton(buttonIndex), abilities[i].GetButtonHandler());
            // }
            // 
            // for(int i =0; i < nonIndexAbilities.Count; ++i)
            // {
            //     int buttonIndex = bar.GetNextFreeButton();
            //     if(Util.Invalid(buttonIndex))
            //     {
            //         // Full
            //         break;
            //     }
            //     ActionButtonDesc desc = nonIndexAbilities[i].GetHotkey();
            //     InputCode hotKey = nonIndexAbilities[i].GetHotkey();
            //     bar.SetButton(buttonIndex, desc, hotKey);
            //     ui.RegisterAction(bar.GetButton(buttonIndex), abilities[i].GetButtonHandler());
            // }
            // 
            // UISelectedPortrait portrait = ui.FindElement("SelectedPortrait");
            // portrait.SetRenderModel(unit.GetRenderModel());
            // 
            // 
            // // OnBuildingSelected
            // UIActionButton bar = ui.FindElement("ActionBar");
            // bar.ConfigureForBuilding();
            // List<BuildingTask> tasks = building.GetBuildTasks();
            // List<BuildingTask> nonIndexedTasks = new List<BuildTask>();
            // 
            // // -- same thing as the unit...
            // 
            // // OnNothingSelected
            // UIActionButton bar ui.FindElement("ActionBar");
            // bar.Clear();
        }
    }

}