// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using LayerAPI.Interfaces;

namespace ExampleLayer {
    internal class AgentSmith : IAgent, IEquatable<AgentSmith> {
        public Position2D MyPosition { get; private set; }

        public Guid AgentID { get; private set; }
        public bool Dead { get; set; }
        private readonly _2DEnvironment _2DEnvironment;
        private readonly UnregisterAgent _unregisterAgentHandle;
        private readonly ExampleLayer _thislayer;

        public AgentSmith(_2DEnvironment _2DEnvironment, UnregisterAgent unregisterAgentHandle, ExampleLayer thislayer) {
            this._2DEnvironment = _2DEnvironment;
            _unregisterAgentHandle = unregisterAgentHandle;
            _thislayer = thislayer;
            AgentID = Guid.NewGuid();
            Dead = false;
            MyPosition = _2DEnvironment.SetAgentPosition(this);
        }

        #region IAgent Members

        public void Tick() {
            if (!Dead) {
                IEnumerable<AgentSmith> enemies = _2DEnvironment.ExploreRadius(this);
                AgentSmith enemy = enemies.FirstOrDefault();
                if (enemy != null && !enemy.Dead) {
                    enemy.Kill();
                    Console.WriteLine("Killed someone!");
                }
                else {
                    //Console.WriteLine("No one left :(");
                }
            }
            else {
                //Console.WriteLine("I'm dead.");
                _unregisterAgentHandle(_thislayer, this);
            }
            //Console.WriteLine("Tell me, Mr. Anderson, what good is a phone call when you are unable to speak?");
        }

        #endregion

        #region IEquatable<AgentSmith> Members

        public bool Equals(AgentSmith other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return AgentID.Equals(other.AgentID);
        }

        #endregion

        public void Kill() {
            if (!Dead) {
                Dead = true;
                _2DEnvironment.ReleaseMyPosition(this);
            }
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AgentSmith) obj);
        }

        public override int GetHashCode() {
            return AgentID.GetHashCode();
        }

        public static bool operator ==(AgentSmith left, AgentSmith right) {
            return Equals(left, right);
        }

        public static bool operator !=(AgentSmith left, AgentSmith right) {
            return !Equals(left, right);
        }
    }
}