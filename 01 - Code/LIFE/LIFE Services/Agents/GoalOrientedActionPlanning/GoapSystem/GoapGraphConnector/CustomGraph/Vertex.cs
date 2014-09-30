﻿using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.CustomGraph {
    public class Vertex : IGoapVertex, IEquatable<Vertex> {
        private readonly List<IGoapWorldstate> _worldstate;

        private readonly string _name;
        private readonly int _heuristic;


        public Vertex(List<IGoapWorldstate> worldstate, string name = "NotNamedVertex")
        {
            _worldstate = worldstate;
            _name = name;
        }

        public Vertex(List<IGoapWorldstate> worldstate, int heuristic, string name = "NotNamedVertex") {
            _worldstate = worldstate;
            _heuristic = heuristic;
            _name = name;
        }

        public int GetHeuristic(IGoapVertex target) {
            return _heuristic;
        }

        public string GetIdentifier()
        {
            return _name;
        }

        public List<IGoapWorldstate> Worldstate() {
            return _worldstate;
        }

        public override string ToString() {
            string states = _worldstate.Aggregate("", (current, state) => current + " " + state.ToString());

            return string.Format("<Knoten {0} {1}>", _name, states);
        }

        /// <summary>
        /// equality depens on the set of worldstate symbols a vertex represents
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Vertex other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ((_worldstate.Where(x => other._worldstate.Contains(x)).Count() == _worldstate.Count()) &&
                    (other._worldstate.Where(x => _worldstate.Contains(x)).Count() == other._worldstate.Count()));

        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Vertex) obj);
        }

        public override int GetHashCode() {
            return (_worldstate != null ? _worldstate.GetHashCode() : 0);
        }

        public static bool operator ==(Vertex left, Vertex right) {
            return Equals(left, right);
        }

        public static bool operator !=(Vertex left, Vertex right) {
            return !Equals(left, right);
        }
    }
}