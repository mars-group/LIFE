using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using HumanLayer.Agents;
using SimPanInGoapModelDefinition.Worldstates;

namespace SimPanInGoapModelDefinition.Actions {

    /// <summary>
    ///     Follow the path across the cell field to the exit position.
    /// </summary>
    public class FollowPathToExit : AbstractGoapAction {
        private readonly Human _human;
        
        // Maximum of steps the action can use.
        private const int MaximumAttemps = 5;
        private int _previousAttemps;

        
        public FollowPathToExit
            (Human human) :
                base(
                new List<WorldstateSymbol> {new WorldstateSymbol(Properties.HasPath, true, typeof (Boolean))},
                new List<WorldstateSymbol> {new WorldstateSymbol(Properties.IsOnExit, true, typeof (Boolean))}) {
            _human = human;
        }

        public override bool ValidateContextPreconditions() {
            if (_previousAttemps <= MaximumAttemps) {
                return true;
            }
            return false;
        }

        public override void Execute() {
            _previousAttemps += 1;
            _human.MotorAndNavigation.TryWalkNextDirectionOfPlan();

            if (_human.HumanBlackboard.Get(Human.MovementFailed)) {
                _human.MotorAndNavigation.DeletePath();
            }
            
        }

        public override bool IsFinished() {
            return _human.HumanBlackboard.Get(Human.IsOnExit);
        }

        public override AbstractGoapAction GetResetCopy() {
            return new FollowPathToExit(_human);
        }
    }

}