using UnityEngine;

namespace Brightstone
{
    public struct RaycastQuery
    {
        private int mID;
        private Vector3 mOrigin;
        private Vector3 mDirection;
        private float mDistance;
        private int mMask;
        private RaycastCallback mCallback;
        private IRaycastListener mListener;
        private bool mOrderByDistance;

        public int id
        {
            get { return mID; }
            set { mID = value; }
        }
        public Vector3 origin
        {
            get { return mOrigin; }
            set { mOrigin = value; }
        }
        public Vector3 direction
        {
            get { return mDirection; }
            set { mDirection = value; }
        }
        public float distance
        {
            get { return mDistance; }
            set { mDistance = value; }
        }
        public int mask
        {
            get { return mMask; }
            set { mMask = value; }
        }
        public RaycastCallback callback
        {
            get { return mCallback; }
            set { mCallback = value; }
        }
        public IRaycastListener listener
        {
            get { return mListener; }
            set { mListener = value; }
        }

        public bool orderByDistance
        {
            get { return mOrderByDistance; }
            set { mOrderByDistance = value; }
        }
    }

    public delegate void RaycastCallback(ref RaycastQuery query, ref RaycastResult result);

    public interface IRaycastListener
    {
        void OnRaycastCallback(ref RaycastQuery query, ref RaycastResult result);
    }

    public struct RaycastResult
    {
        private RaycastHit[] mHits;
        private bool mAsync;

        public RaycastResult(RaycastHit[] hits, bool async)
        {
            mHits = hits;
            mAsync = async;
        }

        public RaycastHit[] hits
        {
            get { return mHits; }
        }

        public bool async
        {
            get { return mAsync; }
        }
    }

}

