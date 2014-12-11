using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapCommon.Interfaces;
using HumanLayer.Agents;
using SimPanInGoapModelDefinition.Actions;
using SimPanInGoapModelDefinition.Goals;
using SimPanInGoapModelDefinition.Worldstates;
using TypeSafeBlackboard;

namespace SimPanInGoapModelDefinition {

    public class DeliberativeConfig : IGoapAgentConfig {
        private readonly Human _human;
        private readonly Blackboard _blackboard;

        public DeliberativeConfig(Human human, Blackboard blackboard) {
            _human = human;
            _blackboard = blackboard;
        }

        #region IGoapAgentConfig Members

        public List<WorldstateSymbol> GetStartWorldstate() {
            return GetUpdatedSymbols();
        }

        public List<AbstractGoapAction> GetAllActions() {
            return new List<AbstractGoapAction> {
                new ChooseExit(_human),
                new FindPathToExit(_human),
                new FollowPathToExitArea(_human),
                new AggressiveApproximation(_human),
                new GoOut(_human),
            };
        }

        public List<IGoapGoal> GetAllGoals() {
            return new List<IGoapGoal> {
                new BeOutOfDanger(),
            };
        }

        public int GetMaxGraphSearchDepth() {
            return 20;
        }

        public bool ForceSymbolsUpdateBeforePlanning() {
            return true;
        }

        public bool ForceSymbolsUpdateEveryActionRequest() {
            return true;
        }

        public List<WorldstateSymbol> GetUpdatedSymbols() {
            List<WorldstateSymbol> updatedSymbols = new List<WorldstateSymbol> {
                new WorldstateSymbol(Properties.IsOutSide, _blackboard.Get(Human.IsOutSide), typeof (Boolean)),
                new WorldstateSymbol(Properties.IsOnExit, _blackboard.Get(Human.IsOnExit), typeof (Boolean)),
                new WorldstateSymbol(Properties.IsInExitArea, _blackboard.Get(Human.IsInExitArea), typeof (Boolean)),
                new WorldstateSymbol(Properties.HasTarget, _blackboard.Get(Human.HasTarget), typeof (Boolean)),
                new WorldstateSymbol(Properties.HasPath, _blackboard.Get(Human.HasPath), typeof (Boolean)),
                new WorldstateSymbol
                    (Properties.KnowsExitLocation, _blackboard.Get(Human.KnowsExitLocation), typeof (Boolean)),
                new WorldstateSymbol
                    (Properties.KnowsExitLocation, _blackboard.Get(Human.KnowsExitLocation), typeof (Boolean))
            };
            return updatedSymbols;
        }

        #endregion
    }

}