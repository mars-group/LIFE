﻿using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapCommon.Interfaces;
using GoapModelTest.Actions;
using GoapModelTest.Goals;
using GoapModelTest.Worldstates;

namespace GoapModelTest {

    /// <summary>
    ///     determines the configuration of agent 1
    /// </summary>
    public class AgentConfig1 : IGoapAgentConfig {
        #region IGoapAgentConfig Members

        public List<WorldstateSymbol> GetStartWorldstate() {
            List<WorldstateSymbol> symbols = new List<WorldstateSymbol> {
                new WorldstateSymbol(WorldProperties.Happy, false, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.HasMoney, true, typeof (Boolean)),
                new WorldstateSymbol(WorldProperties.Happy, false, typeof (Boolean))
            };

            return symbols;
        }

        public List<AbstractGoapAction> GetAllActions() {
            return new List<AbstractGoapAction> {new ActionClean(), new ActionGetToy(), new ActionPlay()};
        }

        public List<IGoapGoal> GetAllGoals() {
            return new List<IGoapGoal> {new GoalBeHappy(), new GoalGetRich()};
        }

        public int GetMaxGraphSearchDepth() {
            return 20;
        }

        #endregion
    }

}