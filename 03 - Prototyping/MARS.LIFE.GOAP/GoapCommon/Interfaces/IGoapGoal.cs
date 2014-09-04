﻿using System.Collections.Generic;
using NUnit.Framework;

namespace GoapCommon.Interfaces
{
    public interface IGoapGoal {

        bool IsSatisfied(List<IGoapWorldstate> worldstate);

        int GetRelevancy();

        int UpdateRelevancy(List<IGoapWorldstate> actualWorldstate);

    }
}