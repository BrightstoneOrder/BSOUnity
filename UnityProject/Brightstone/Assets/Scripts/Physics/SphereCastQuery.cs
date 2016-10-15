using UnityEngine;
namespace Brightstone
{

    public struct SphereCastQuery
    {
        private bool mAsync;
        private int mID;
        private Vector3 mOrigin;
        private Vector3 mDirection;
        private float mDistance;
        private float mRadius;
        private int mMask;
        private SphereCastCallback mCallback;
        private ISphereCastListener mListener;
        private bool mOrderByDistance;

        public bool async
        {
            get { return mAsync; }
            set { mAsync = value; }
        }
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
        public float radius
        {
            get { return mRadius; }
            set { mRadius = value; }
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
        public SphereCastCallback callback
        {
            get { return mCallback; }
            set { mCallback = value; }
        }
        public ISphereCastListener listener
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

    public delegate void SphereCastCallback(ref SphereCastQuery query, ref SphereCastResult result);

    public interface ISphereCastListener
    {
        void OnSphereCastCallback(ref SphereCastQuery query, ref SphereCastResult result);
    }

    public struct SphereCastResult
    {
        private RaycastHit[] mHits;
        private bool mAsync;

        public SphereCastResult(RaycastHit[] hits, bool async)
        {
            mHits = hits;
            mAsync = async;
        }

        public RaycastHit[] hits
        {
            get { return mHits; }
        }

        public bool aSync
        {
            get { return mAsync; }
        }
    }

}