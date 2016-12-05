using UnityEngine;
using System;

namespace Brightstone
{
    public class PrefabViewAttribute : PropertyAttribute
    {
        public PrefabViewAttribute() : base()
        {
            constraint = typeof(Actor);
        }
        public PrefabViewAttribute(Type inConstraint) : base()
        {
            if(inConstraint != null && inConstraint.IsSubclassOf(typeof(Actor)))
            {
                constraint = inConstraint;
            }
            else
            {
                constraint = typeof(Actor);
            }
        }

        public Type constraint { get; set; }        
    }
}


