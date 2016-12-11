using UnityEngine;
using System.Collections.Generic;

namespace Brightstone
{
    public class Unit : Actor
    {

        List<UnitAbility> mAbilities = new List<UnitAbility>();


        public List<UnitAbility> GetAbilities()
        {
            return mAbilities;
        }

    }

}

