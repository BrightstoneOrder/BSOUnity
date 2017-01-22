using UnityEngine;
using System;

namespace Brightstone
{
    [Serializable]
    public class EditorConfig
    {
        // World sets current at Init time...
        public static EditorConfig current = null;

        // Break naming convention to avoid creating properties & extra typing.
        public bool debugResourceBatching = false;
        public bool debugLogRegister = false;
        public bool debugWorldTerrain = false;


    }
}