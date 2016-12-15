using UnityEngine;
using UnityEditor;

namespace Brightstone
{
    [CustomEditor(typeof(World))]
    public class WorldEditor : BaseEditor
    {
        SerializedProperty mScript = null;

        void OnEnable()
        {
            InitProperties();
        }

        protected override void InitProperties()
        {
            base.InitProperties();
            mScript = serializedObject.FindProperty("m_Script");
        }

        protected override void OnGameUpdate(BaseComponent targetComponent)
        {
            base.OnGameUpdate(targetComponent);
            World world = targetComponent as World;
            if(world != null)
            {
                if(world.GetInputMgr() != null)
                {
                    InputMouseData mouseData = world.GetInputMgr().GetMouseData();
                    PushGroup("Input");
                    AddVector("World Position", mouseData.worldPosition);
                    AddVector("Screen Position", mouseData.screenPosition);
                    if(mouseData.hitGameObject != null)
                    {
                        AddString("Hit Actor", mouseData.hitGameObject.name);
                    }
                    else
                    {
                        AddString("Hit Actor", string.Empty);
                    }
                    PopGroup();
                }
                if(world.GetPhysicsMgr() != null)
                {
                    PushGroup("Physics");
                    AddInt("Queries", world.GetPhysicsMgr().GetQueriesMade());
                    AddInt("Queries Last Frame", world.GetPhysicsMgr().GetQueriesMadeLastFrame());
                    PopGroup();
                }

                
            }
        }

        protected override void OnEditorUpdate(BaseComponent targetComponent)
        {
            base.OnEditorUpdate(targetComponent);
            if (mScript != null)
            {
                AddProperty(mScript);
                
            }
            AutoSerializeProperties(typeof(World));
        }
    }
}

