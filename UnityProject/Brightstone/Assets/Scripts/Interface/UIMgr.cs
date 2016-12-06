using UnityEngine;
using System.Collections.Generic;

namespace Brightstone
{
    public class UIMgr : BaseObject
    {
        private List<UIBase> mUIActors = new List<UIBase>();

        

        public void Register(UIBase actor)
        {
            mUIActors.Add(actor);
        }

        public void Unregister(UIBase actor)
        {
            mUIActors.Remove(actor);
        }

        public void Init()
        {

        }

        public void Update(World world)
        {
            if(world.GetInputMgr().NativeGetKeyState(KeyCode.Alpha1))
            {
                Log.Game.Info("HideUI!");
                HideUI();
            }
            if(world.GetInputMgr().NativeGetKeyState(KeyCode.Alpha2))
            {
                Log.Game.Info("ShowUI!");
                ShowUI();
            }
        }

        public void ShowUI()
        {
            for(int i = 0; i < mUIActors.Count; ++i)
            {
                if(mUIActors[i] != null)
                {
                    mUIActors[i].Show();
                }
            }
        }

        public void HideUI()
        {
            for(int i = 0;i < mUIActors.Count; ++i)
            {
                if(mUIActors[i] != null)
                {
                    mUIActors[i].Hide();
                }
            }
        }
    }

}