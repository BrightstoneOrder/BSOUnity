using UnityEngine;
using UnityEngine.EventSystems;
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
        struct InputCallbackPair
        {
            public BaseComponent targetComponent;
            public BaseObject targetObject;
            public InputCallback callback;

            public bool IsValid()
            {
                return callback != null && (targetComponent != null || targetObject != null);
            }
        }

        // Input Handler Vars. Input handlers act as "buttons, axis, dual axis" but translated into enum game codes.
        private int mHandlerCount = 0;
        private InputHandler[] mHandlers = null;
        // List of callbacks associated with a InputCode
        private List<InputCallbackPair>[] mCallbacks = null;
        // Input and Mouse data vars to gather mouse data.
        // Mouse data is gathered once each frame. (Reduces the number of raycasts.)
        private EventSystem mEventSystem = null;
        private InputMouseData mMouseData = new InputMouseData();
        private InputMouseData mPreviousMouseData = new InputMouseData();
        private List<ObjectType> mIgnoreTypes = new List<ObjectType>();
        private List<Actor> mIgnoreActors = new List<Actor>();

        public void RegisterCallback(InputCallback callback, InputCode code)
        {
            InputCallbackPair pair = new InputCallbackPair();
            pair.targetComponent = callback.Target as BaseComponent;
            pair.targetObject = callback.Target as BaseObject;
            pair.callback = callback;

            mCallbacks[(int)code].Add(pair);
        }

        public void UnregisterCallback(InputCallback callback, InputCode code)
        {
            int index = mCallbacks[(int)code].FindIndex(x => x.callback == callback);
            if(index != -1)
            {
                mCallbacks[(int)code].RemoveAt(index);
            }
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

        private void RegisterMouse(InputCode code, InputButton button)
        {
            InputHandler handler = new Brightstone.InputHandler();
            handler.SetInputButton(button);
            handler.SetInputCode(code);
            handler.SetInputHandlerType(InputHandlerType.IHT_MOUSE);
            mHandlers[mHandlerCount] = handler;
            ++mHandlerCount;
        }

        private void RegisterInputs()
        {
            mHandlers = new InputHandler[Util.GetEnumCount<InputCode>()];
            mHandlers[0] = new InputHandler(); mHandlerCount = 1; // IC_NONE
            RegisterDualAxis(InputCode.IC_PLAYER_MOVE_VERTICAL, KeyCode.W, KeyCode.S, 1.0f, true);
            RegisterDualAxis(InputCode.IC_PLAYER_MOVE_HORIZONTAL, KeyCode.D, KeyCode.A, 1.0f, true);

            // TODO: Modify bindings with user data..
            // eg. Look for "IC_PLAYER_MOVE_VERTICAL.PrimaryKey and IC_PLAYER_MOVE_HORIZONTAL.SecondaryKey"
        }

        public bool NativeGetKeyState(KeyCode keyCode)
        {
            return Input.GetKey(keyCode);
        }
        public bool NativeGetButtonState(InputButton button)
        {
            return Input.GetMouseButton((int)button);
        }

        public void Init(World world)
        {
            mCallbacks = new List<InputCallbackPair>[Util.GetEnumCount<InputCode>()];
            for(int i = 0; i < mCallbacks.Length; ++i)
            {
                mCallbacks[i] = new List<InputCallbackPair>();
            }
            RegisterInputs();
            mEventSystem = EventSystem.current;
        }

        public void Update(World world)
        {
            UpdateHandlers(world);
            UpdateMouse();
        }

        private void UpdateMouse()
        {
            // Get Mouse to world data
            mPreviousMouseData = mMouseData;
            mMouseData = new InputMouseData();
            mMouseData.screenPosition = Input.mousePosition;
            Ray screenToWorldRay = World.ActiveWorld.GetGameCamera().ScreenPointToRay(mMouseData.screenPosition);
            mMouseData.screenToWorldDirection = screenToWorldRay.direction;
            mMouseData.screenToWorldPosition = screenToWorldRay.origin;

            if(mEventSystem == null)
            {
                mEventSystem = EventSystem.current;
            }
            if(mEventSystem)
            {
                if(mEventSystem.currentSelectedGameObject == null)
                {
                    mMouseData.target = InputMouseData.Target.World;
                }
                else
                {
                    mMouseData.target = InputMouseData.Target.Interface;
                }
            }
            // Create query and Raycast to get some data.
            RaycastQuery query = new RaycastQuery();
            query.direction = mMouseData.screenToWorldDirection;
            query.origin = mMouseData.screenToWorldPosition;
            query.distance = 100.0f;
            query.mask = PhysicsMgr.RAYCAST_ALL;
            query.orderByDistance = true;
            RaycastResult result = World.ActiveWorld.GetPhysicsMgr().Raycast(query);
            
            Vector3 hitPoint = query.origin;
            // Find the actual hitPoint
            for (int i = 0; i < result.hits.Length; ++i)
            {
                Actor actor = Actor.GetRootActor(result.hits[i].transform);
                // Hit first non-actor
                if(!actor)
                {
                    hitPoint = result.hits[i].point;
                    mMouseData.hitGameObject = result.hits[i].transform.gameObject;
                    break;
                }
                bool ignore = false;
                for (int j = 0; j < mIgnoreTypes.Count; ++j)
                {
                    if (actor.IsType(mIgnoreTypes[j]))
                    {
                        ignore = true;
                        break;
                    }
                }
                if (!ignore)
                {
                    for (int j = 0; j < mIgnoreActors.Count; ++j)
                    {
                        if (actor == mIgnoreActors[j])
                        {
                            ignore = true;
                            break;
                        }
                    }
                }
                // Or Closest not ignored actor!
                if (!ignore && actor != null)
                {
                    hitPoint = result.hits[i].point;
                }
            }

            Actor[] actors = World.ActiveWorld.GetPhysicsMgr().GetRootActors(ref result);
            mMouseData.hitActor = actors != null && actors.Length > 0 ? actors[0] : null;
            if(mMouseData.hitActor != null)
            {
                mMouseData.hitGameObject = mMouseData.hitActor.gameObject;
            }
            mMouseData.worldPosition = hitPoint;



        }

        private void UpdateHandlers(World world)
        {
            float delta = world.GetGameDelta();
            // Start at 1 and skip IC_NONE
            for(int i = 1; i < mHandlers.Length; ++i)
            {
                InputHandler handler = mHandlers[i];
                handler.Update(this, delta);
                if(handler.GetInputHandlerType() == InputHandlerType.IHT_BUTTON || handler.GetInputHandlerType() == InputHandlerType.IHT_MOUSE)
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
                            List<InputCallbackPair> receievers = mCallbacks[(int)handler.GetInputCode()];
                            for(int j = 0; i < receievers.Count; ++j)
                            {
                                if(receievers[j].IsValid())
                                {
                                    receievers[j].callback.Invoke(ref eventData);
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
                            List<InputCallbackPair> receievers = mCallbacks[(int)handler.GetInputCode()];
                            for (int j = 0; i < receievers.Count; ++j)
                            {
                                if (receievers[j].IsValid())
                                {
                                    receievers[j].callback.Invoke(ref eventData);
                                }
                            }
                        }
                    }
                }
            }
            
        }

        /** Get mouse data. This is data retrieved based on a raycast each frame. */
        public InputMouseData GetMouseData() { return mMouseData; }
        /** Get previous mouse data. The previous frames mouse data. */
        public InputMouseData GetPreviousMouseData() { return mPreviousMouseData; }
        /** Get the ignored types. These are types that are ignored when doing the mouse data raycast.*/
        public List<ObjectType> GetIgnoredTypes() { return mIgnoreTypes; }
        /** Get the ignored actors. These are the ignored actors when doing the mouse data raycast.*/
        public List<Actor> GetIgnoredActors() { return mIgnoreActors; }
        /** Adds a type to the ignore list. These are the ignored types when doing the mouse data raycast. */
        public void AddIgnore(ObjectType type)
        {
            mIgnoreTypes.Add(type);
        }
        /** Adds a actor to the ignore list. These are the ignored actors when doing the mouse data raycast. */
        public void AddIgnore(Actor actor)
        {
            mIgnoreActors.Add(actor);
        }
        /** Removes a type from the ignore list. */
        public void RemoveIgnore(ObjectType type)
        {
            mIgnoreTypes.Remove(type);
        }
        /** Removes a actor from the ignore list. */
        public void RemoveIgnore(Actor actor)
        {
            mIgnoreActors.Remove(actor);
        }

	}
}