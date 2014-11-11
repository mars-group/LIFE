using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;

namespace SimPanInGoapModelDefinition.Actions
{
    public class AimlessWandering : AbstractGoapAction
    {
        public AimlessWandering(List<IGoapWorldProperty> preconditionWorldstates, List<IGoapWorldProperty> effectWorldstates) :
            base(preconditionWorldstates, effectWorldstates) {}

        public override bool ValidateContextPreconditions() {
            return true;
        }

        public override bool ExecuteContextEffects() {
            throw new NotImplementedException();
        }

        public override int GetExecutionCosts() {
            throw new NotImplementedException();
        }

        public override int GetPriority() {
            throw new NotImplementedException();
        }

        public override void Execute() {
            
        }
    }
}
