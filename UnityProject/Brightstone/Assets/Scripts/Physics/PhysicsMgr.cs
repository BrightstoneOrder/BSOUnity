using UnityEngine;
using System.Collections.Generic;
using System.Linq;
namespace Brightstone
{
    /**
    * Manages physics operations. Can do deferred raycasts. (Eg raycast when optimal..)
    */
    public class PhysicsMgr : BaseObject
    {
        public enum UpdateQuery
        {
            Raycast,
            SphereCast
        }
        public const int RAYCAST_ALL = Physics.AllLayers;

        private Option mQueriesPerUpdate = null;
        private UpdateQuery mUpdateQuery = UpdateQuery.Raycast;
        private Queue<RaycastQuery> mAsyncRaycasts = new Queue<RaycastQuery>();
        private Queue<SphereCastQuery> mAsyncSphereCasts = new Queue<SphereCastQuery>();
        private int mAsyncRequests = 0;

        private int mQueriesMade = 0;
        private int mQueriesMadeLastFrame = 0;

        protected void Init()
        {
            Log.Sys.Info("Initialized PhysicsMgr...");
            mQueriesPerUpdate = World.ActiveWorld.GetOptionsMgr().GetOption(OptionName.ON_PHYSICS_QUERIES_PER_UPDATE);
            if(mQueriesPerUpdate.GetOptionType() != OptionType.OT_INT)
            {
                Log.Phys.Error("Option " + mQueriesPerUpdate.GetName() + " should be type OT_INT but its " + mQueriesPerUpdate.GetOptionType().ToString());
            }
        }
        protected void Shutdown()
        {

        }

        public RaycastResult Raycast(RaycastQuery query)
        {
            RaycastHit[] hits = Physics.RaycastAll(query.origin, query.direction, query.distance, query.mask);
            if (query.orderByDistance)
            {
                hits = hits.OrderBy(h => h.distance).ToArray();
            }
            RaycastResult result = new RaycastResult(hits, false);
            if (query.callback != null)
            {
                query.callback(ref query, ref result);
            }
            if (query.listener != null)
            {
                query.listener.OnRaycastCallback(ref query, ref result);
            }
            ++mQueriesMade;
            return result;
        }
        public void AsyncRaycast(RaycastQuery query)
        {
            mAsyncRaycasts.Enqueue(query);
            ++mAsyncRequests;
        }

        public SphereCastResult SphereCast(SphereCastQuery query)
        {
            RaycastHit[] hits = Physics.SphereCastAll(query.origin, query.radius, query.direction, query.distance, query.mask);
            if (query.orderByDistance)
            {
                hits = hits.OrderBy(h => h.distance).ToArray();
            }
            SphereCastResult result = new SphereCastResult(hits, false);
            if (query.callback != null)
            {
                query.callback(ref query, ref result);
            }
            if (query.listener != null)
            {
                query.listener.OnSphereCastCallback(ref query, ref result);
            }
            ++mQueriesMade;
            return result;
        }
        public void AsyncSphereCast(SphereCastQuery query)
        {
            mAsyncSphereCasts.Enqueue(query);
            ++mAsyncRequests;
        }

        private void Update()
        {
            DeferredUpdate();
            mQueriesMadeLastFrame = mQueriesMade;
            mQueriesMade = 0;
        }

        private void DeferredUpdate()
        {
            int numQueries = Mathf.Min(mQueriesPerUpdate.GetIntValue(), mAsyncRequests);
            for (int i = 0; i < numQueries; ++i)
            {
                switch (mUpdateQuery)
                {
                    case UpdateQuery.Raycast:
                        {
                            if (mAsyncRaycasts.Count != 0)
                            {
                                RaycastQuery query = mAsyncRaycasts.Dequeue();
                                RaycastHit[] hits = Physics.RaycastAll(query.origin, query.direction, query.distance, query.mask);
                                if (query.orderByDistance)
                                {
                                    hits = hits.OrderBy(h => h.distance).ToArray();
                                }
                                RaycastResult result = new RaycastResult(hits, true);
                                if (query.callback != null)
                                {
                                    query.callback(ref query, ref result);
                                }
                                if (query.listener != null)
                                {
                                    query.listener.OnRaycastCallback(ref query, ref result);
                                }
                                --mAsyncRequests;
                            }
                            ++mQueriesMade;
                        }
                        break;
                    case UpdateQuery.SphereCast:
                        {
                            if (mAsyncSphereCasts.Count != 0)
                            {
                                SphereCastQuery query = mAsyncSphereCasts.Dequeue();
                                RaycastHit[] hits = Physics.SphereCastAll(query.origin, query.radius, query.direction, query.distance, query.mask);
                                if (query.orderByDistance)
                                {
                                    hits = hits.OrderBy(h => h.distance).ToArray();
                                }
                                SphereCastResult result = new SphereCastResult(hits, true);
                                if (query.callback != null)
                                {
                                    query.callback(ref query, ref result);
                                }
                                if (query.listener != null)
                                {
                                    query.listener.OnSphereCastCallback(ref query, ref result);
                                }
                                --mAsyncRequests;
                            }
                            ++mQueriesMade;
                        }
                        break;

                }
                NextUpdateQuery();
            }


        }


        private void NextUpdateQuery()
        {
            switch (mUpdateQuery)
            {
                case UpdateQuery.Raycast:
                    {
                        if (mAsyncSphereCasts.Count != 0)
                        {
                            mUpdateQuery = UpdateQuery.SphereCast;
                        }
                    }
                    break;
                case UpdateQuery.SphereCast:
                    {
                        if (mAsyncRaycasts.Count != 0)
                        {
                            mUpdateQuery = UpdateQuery.Raycast;
                        }
                    }
                    break;
                default:

                    break;
            }
        }

        public Actor[] GetRootActors(ref RaycastResult result)
        {
            RaycastHit[] hits = result.hits;
            List<Actor> actors = new List<Actor>();
            for (int i = 0; i < hits.Length; ++i)
            {
                Actor actor = Actor.GetRootActor(hits[i].transform);
                if (actor != null)
                {
                    actors.Add(actor);
                }
            }
            return actors.ToArray();
        }

        public Actor[] GetRootActors(ref SphereCastResult result)
        {
            RaycastHit[] hits = result.hits;
            List<Actor> actors = new List<Actor>();
            for (int i = 0; i < hits.Length; ++i)
            {
                Actor actor = Actor.GetRootActor(hits[i].transform);
                if (actor != null)
                {
                    actors.Add(actor);
                }
            }
            return actors.ToArray();
        }

        public int Cull(ref RaycastResult result, ObjectType type)
        {
            if (type == null)
            {
                return 0;
            }
            List<RaycastHit> culled = new List<RaycastHit>();
            for (int i = 0; i < result.hits.Length; ++i)
            {
                Actor actor = Actor.GetRootActor(result.hits[i].transform);
                if (actor != null)
                {
                    if (!actor.IsType(type))
                    {
                        culled.Add(result.hits[i]);
                    }
                }
                else
                {
                    culled.Add(result.hits[i]);
                }
            }
            int amountCulled = result.hits.Length - culled.Count;
            result = new RaycastResult(culled.ToArray(), result.async);
            return amountCulled;
        }
    }

}