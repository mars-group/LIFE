using System;
using System.Collections.Generic;
using GoapBetaCommon.Abstract;
using GoapBetaCommon.Implementation;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Goals
{

    public class GoalGetRich : AbstractGoapGoal
    {
        public GoalGetRich()
            : base(new List<WorldstateSymbol> {
                new WorldstateSymbol(WorldProperties.HasMoney, true, typeof (Boolean))
            },
                1) { }

        public override int UpdateRelevancy(List<WorldstateSymbol> actualWorldstate)
        {
            if (IsSatisfied(actualWorldstate))
            {
                return Relevancy = 0;
            }
            else
            {
                if (Relevancy < 10)
                {
                    return Relevancy += 1;
                }
                else
                {
                    return Relevancy;
                }
            }
        }
    }



}
