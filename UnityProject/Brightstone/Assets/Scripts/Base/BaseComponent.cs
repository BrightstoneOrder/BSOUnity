using UnityEngine;
using System.Collections.Generic;

namespace Brightstone
{
    /**
    * The base class of all future components to come. 
    */
    public class BaseComponent : MonoBehaviour
    {
        /** A type called default constructor.. To be used with structs because we can't define out own. :/ */
        public struct DefaultConstructor { };
        public static DefaultConstructor DEFAULT_CONSTRUCTOR = new DefaultConstructor();
        /** Cached unity xform.*/
        private Transform mTransform = null;
        /** Flags related to this components state. */
        private ComponentFlagsBitfield mFlags = new ComponentFlagsBitfield();
#if UNITY_EDITOR
        /** List of internal editor variables. Editor ONLY!*/
        private List<EditorVariable> mEditorVariables = new List<EditorVariable>();
#endif
        // Cached world var. cached @ InternalInit before OnInit
        protected World mWorld = null;

        /** Internal initialize method. Calling this will mark the component as initialized and it cannot be initialized again.*/
        public void InternalInit()
        {
            // Already initialized.
            if (mFlags.Has(ComponentFlags.CF_INITIALIZED))
            {
                return;
            }
            // Cache components
            mTransform = GetComponent<Transform>();
            mWorld = World.ActiveWorld;
            // Set 'Type' flag.

            // Allow derived classes to initialize.
            OnInit();
        }

        public void InternalDestroy()
        {
            // Garbage components SHOULD not destroy.
            if (mFlags.Has(ComponentFlags.CF_GARBAGE))
            {
                return;
            }
            mFlags.Set(ComponentFlags.CF_GARBAGE);
            // Allow derived classes to destroy.
            OnDestroyed();
            
        }

        public void InternalRecycle()
        {
            // Allow derived classes to reset their values.
            OnRecycle();

            mTransform = null;
            mFlags.SetZero();
        }

        /** Called once to initialize the component. Acquire resources. */
        protected virtual void OnInit() { }
        /** Called once to destroy the component. Release resources. */
        protected virtual void OnDestroyed() { }
        /** Called once to recycle the component. Set defaults. */
        protected virtual void OnRecycle() { }

        /** Called to queue resources for batch loading. (eg. Call Prepare on Prefabs) */
        public virtual void OnBatchSubmit() { }
        /** Called to signal that your submitted batch is complete and the resources are ready for use. */
        public virtual void OnBatchComplete() { }

        /** Called to serialize object properties.*/
        public virtual void Serialize(BaseStream stream) { }


        public void QueueBatchLoad()
        {
            if(IsBatchLoadRegistered())
            {
                return;
            }
            mWorld.InternalRegisterBatchLoad(this);
            mFlags.Set(ComponentFlags.CF_BATCH_LOAD_REGISTERED);
        }

        public void SetBatchLoadComplete()
        {
            mFlags.Unset(ComponentFlags.CF_BATCH_LOAD_REGISTERED);
        }

        public Transform GetTransform()
        {
            return mTransform;
        }

        public bool IsGarbage()
        {
            return mFlags.Has(ComponentFlags.CF_GARBAGE);
        }

        public bool IsBatchLoadRegistered()
        {
            return mFlags.Has(ComponentFlags.CF_BATCH_LOAD_REGISTERED);
        }

#if UNITY_EDITOR
        public int GetEditorInt(string name)
        {
            EditorVariable editorVar = mEditorVariables.Find(e => e.GetName() == name);
            if (editorVar != null)
            {
                return editorVar.GetInt();
            }
            return 0;
        }

        public float GetEditorFloat(string name)
        {
            EditorVariable editorVar = mEditorVariables.Find(e => e.GetName() == name);
            if (editorVar != null)
            {
                return editorVar.GetFloat();
            }
            return 0.0f;
        }

        public string GetEditorString(string name)
        {
            EditorVariable editorVar = mEditorVariables.Find(e => e.GetName() == name);
            if (editorVar != null)
            {
                return editorVar.GetString();
            }
            return string.Empty;
        }

        public void SetEditorInt(string name, int value)
        {
            EditorVariable editorVar = mEditorVariables.Find(e => e.GetName() == name);
            if (editorVar != null)
            {
                editorVar.SetValue(value);
            }
            else
            {
                editorVar = new EditorVariable(name, value);
                mEditorVariables.Add(editorVar);
            }
        }

        public void SetEditorFloat(string name, float value)
        {
            EditorVariable editorVar = mEditorVariables.Find(e => e.GetName() == name);
            if (editorVar != null)
            {
                editorVar.SetValue(value);
            }
            else
            {
                editorVar = new EditorVariable(name, value);
                mEditorVariables.Add(editorVar);
            }
        }

        public void SetEditorString(string name, float value)
        {
            EditorVariable editorVar = mEditorVariables.Find(e => e.GetName() == name);
            if (editorVar != null)
            {
                editorVar.SetValue(value);
            }
            else
            {
                editorVar = new EditorVariable(name, value);
                mEditorVariables.Add(editorVar);
            }
        }
#endif
    }
}