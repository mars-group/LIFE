using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using HumanLayer.Agents;
using SimPanInGoapModelDefinition.Worldstates;

namespace SimPanInGoapModelDefinition.Actions {

    public class SuccessiveApproximation : AbstractGoapAction {
        private readonly Human _human;
        private const int MaximumAttemps = 30;
        private int _previousAttemps;


        public SuccessiveApproximation
            (Human human) :
                base(
                //new List<WorldstateSymbol> {new WorldstateSymbol(Properties.IsInExitArea, true, typeof (Boolean))},
                new List<WorldstateSymbol>(),
                new List<WorldstateSymbol> {new WorldstateSymbol(Properties.IsOnExit, true, typeof (Boolean))}) {
            _human = human;
            _previousAttemps = 0;
        }

        public override bool ValidateContextPreconditions() {
            if (_previousAttemps <= MaximumAttemps){
                return true;
                
            }
            return false;
        }

        public override bool ExecuteContextEffects() {
            return true;
        }

        public override int GetExecutionCosts() {
            return 1;
        }
        
        public override void Execute() {
            _previousAttemps += 1;
            Console.WriteLine("SuccessiveApproximation executing with " + _previousAttemps + " try");
            if (_human.HumanBlackboard.Get(Human.MovementFailed)) {
                _human.MotorAndNavigation.ApproximateToTarget(true);
            }
            else {
                _human.MotorAndNavigation.ApproximateToTarget();
                
            }
        }

        public override bool IsFinished() {
            return _human.HumanBlackboard.Get(Human.IsOnExit);
        }

        public override AbstractGoapAction GetResetCopy() {
            return new SuccessiveApproximation(_human);
        }


    }

}