using UnityEngine;
using System.Collections.Generic;

namespace Brightstone
{
    /**
    * ---Types---
    * class InputHandler
    * enum InputCode
    * delegate InputCallback
    * struct InputData
    * struct MouseData
    */
    public delegate void InputCallback(ref InputEventData data);
    public class InputMgr 
	{
        private int mHandlerCount = 0;
        private InputHandler[] mHandlers = null;
        private List<InputCallback>[] mCallbacks = null;

        public void RegisterCallback(InputCallback callback, InputCode code)
        {
            mCallbacks[(int)code].Add(callback);
        }

        public void UnregisterCallback(InputCallback callback, InputCode code)
        {
            mCallbacks[(int)code].Remove(callback);
        }

        public float PeekAxisValue(InputCode code)
        {
            InputHandler handler = mHandlers[(int)code];
            if(handler.GetInputHandlerType() == InputHandlerType.IHT_AXIS || handler.GetInputHandlerType() == InputHandlerType.IHT_DUAL_AXIS)
            {
                return mHandlers[(int)code].GetCurrentValue();
            }
            return 0.0f;
        }
        public float PeekLastAxisValue(InputCode code)
        {
            InputHandler handler = mHandlers[(int)code];
            if (handler.GetInputHandlerType() == InputHandlerType.IHT_AXIS || handler.GetInputHandlerType() == InputHandlerType.IHT_DUAL_AXIS)
            {
                return mHandlers[(int)code].GetLastValue();
            }
            return 0.0f;
        }

        private void RegisterButtonHandler(InputCode code, KeyCode primaryKey)
        {
            InputHandler handler = new InputHandler();
            handler.SetPrimaryKey(primaryKey);
            handler.SetInputCode(code);
            handler.SetInputHandlerType(InputHandlerType.IHT_BUTTON);
            mHandlers[mHandlerCount] = handler;
            ++mHandlerCount;
        }

        private void RegisterAxis(InputCode code, KeyCode primaryKey, float modifier, bool snapToZero)
        {
            InputHandler handler = new InputHandler();
            handler.SetPrimaryKey(primaryKey);
            handler.SetInputCode(code);
            handler.SetInputHandlerType(InputHandlerType.IHT_AXIS);
            handler.SetDeltaModifier(modifier);
            handler.SetSnapToZero(snapToZero);
            mHandlers[mHandlerCount] = handler;
            ++mHandlerCount;
        }

        private void RegisterDualAxis(InputCode code, KeyCode primaryKey, KeyCode secondaryeky, float modifier, bool snapToZero)
        {
            InputHandler handler = new InputHandler();
            handler.SetPrimaryKey(primaryKey);
            handler.SetSecondaryKey(secondaryeky);
            handler.SetInputCode(code);
            handler.SetInputHandlerType(InputHandlerType.IHT_DUAL_AXIS);
            handler.SetDeltaModifier(modifier);
            handler.SetSnapToZero(snapToZero);
            mHandlers[mHandlerCount] = handler;
            ++mHandlerCount;
        }

        private void RegisterInputs()
        {
            mHandlers = new InputHandler[Util.GetEnumCount<InputCode>()];
            mHandlers[0] = new InputHandler(); mHandlerCount = 1; // IC_NONE
            RegisterDualAxis(InputCode.IC_PLAYER_MOVE_VERTICAL, KeyCode.W, KeyCode.S, 1.0f, true);
            RegisterDualAxis(InputCode.IC_PLAYER_MOVE_HORIZONTAL, KeyCode.A, KeyCode.D, 1.0f, true);

            // TODO: Modify bindings with user data..
            // eg. Look for "IC_PLAYER_MOVE_VERTICAL.PrimaryKey and IC_PLAYER_MOVE_HORIZONTAL.SecondaryKey"
        }

        public bool NativeGetKeyState(KeyCode keyCode)
        {
            return Input.GetKey(keyCode);
        }

        public void Init(World world)
        {
            mCallbacks = new List<InputCallback>[Util.GetEnumCount<InputCode>()];
            for(int i = 0; i < mCallbacks.Length; ++i)
            {
                mCallbacks[i] = new List<InputCallback>();
            }
            RegisterInputs();
        }

        public void Update(World world)
        {
            UpdateHandlers(world);
        }

        private void UpdateMouse()
        {

        }

        private void UpdateHandlers(World world)
        {
            float delta = world.GetGameDelta();
            // Start at 1 and skip IC_NONE
            for(int i = 1; i < mHandlers.Length; ++i)
            {
                InputHandler handler = mHandlers[i];
                handler.Update(this, delta);
                if(handler.GetInputHandlerType() == InputHandlerType.IHT_BUTTON)
                {
                    float prev = handler.GetLastValue();
                    float current = handler.GetCurrentValue();
                    if(prev != current)
                    {
                        if(current != 0.0f)
                        {
                            InputEventData eventData = new InputEventData();
                            eventData.code = handler.GetInputCode();
                            eventData.press = true;
                            eventData.release = false;
                            // notify press
                            List<InputCallback> receievers = mCallbacks[(int)handler.GetInputCode()];
                            for(int j = 0; i < receievers.Count; ++j)
                            {
                                if(receievers[j] != null)
                                {
                                    receievers[j].Invoke(ref eventData);
                                }
                            }
                        }
                        else
                        {
                            InputEventData eventData = new InputEventData();
                            eventData.code = handler.GetInputCode();
                            eventData.press = false;
                            eventData.release = true;
                            // notify release
                            List<InputCallback> receievers = mCallbacks[(int)handler.GetInputCode()];
                            for (int j = 0; i < receievers.Count; ++j)
                            {
                                if (receievers[j] != null)
                                {
                                    receievers[j].Invoke(ref eventData);
                                }
                            }
                        }
                    }
                }
            }
            
        }

	}
}