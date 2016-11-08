using UnityEditor;

namespace Brightstone
{
    [CustomEditor(typeof(Actor))]
	public class ActorEditor : BaseEditor 
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

        protected override void OnEditorUpdate(BaseComponent targetComponent)
        {
            if(mScript != null)
            {
                AddProperty(mScript);
            }
            AutoSerializeProperties(typeof(Actor));
        }
    }
}