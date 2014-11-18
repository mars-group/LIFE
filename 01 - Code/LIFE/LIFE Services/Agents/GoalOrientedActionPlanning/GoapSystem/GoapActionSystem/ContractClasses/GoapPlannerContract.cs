using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoapActionSystem.Implementation;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;

namespace GoapActionSystem.ContractClasses
{
    [ContractClassFor(typeof(GoapPlanner))]
    class GoapPlannerContract
    {

        List<AbstractGoapAction> GetPlan(IGoapGoal goal) {
            Contract.Requires(goal != null);
            return default(List<AbstractGoapAction>);
        }
    }
}
