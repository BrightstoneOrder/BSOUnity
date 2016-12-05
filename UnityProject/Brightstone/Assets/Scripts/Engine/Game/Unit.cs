using UnityEngine;

namespace Brightstone
{
    public class Unit : Actor
    {
        [PrefabView]
        [SerializeField]
        private Prefab mItemType = new Prefab();

    }

}

