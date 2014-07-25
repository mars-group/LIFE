using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes.Interfaces;
using GoapCommon.Interfaces;

namespace GoapActionSystem.Implementation
{
    /// <summary>
    ///     nochmal überlegen welche Methoden auf den allgemeinen Objekten ausgeführt werden müssen
    ///     diese können allerdings auch per interface angeboten werden
    ///     für die abstrakte klasse ist allerdings interessant, welche methoden immer gleich sind für alle unterklassen und
    ///     wie den programmierern der actions somit arbeit abgenommen werden kann
    /// 
    ///     auch das graphensystem sollte nur die methoden der abtracten benutzen
    /// </summary>
    public abstract class AbstractGoapAction : IGoapAction
    {
        readonly List<IGoapWorldstate> _preConditions;
        readonly List<IGoapWorldstate> _effects;

        public List<IGoapWorldstate> PreConditions{
            get { return _preConditions; }
        }

        public List<IGoapWorldstate> Effects{
            get { return _effects; }
        }

        protected AbstractGoapAction(List<IGoapWorldstate> preconditionWorldstates, List<IGoapWorldstate> effectWorldstates)
        {
            _preConditions = preconditionWorldstates;
            _effects = effectWorldstates;
        }

        /// <summary>
        /// </summary>
        /// <param name="sourceWorldState"></param>
        /// <returns></returns>
        public bool IsExecutable(List<IGoapWorldstate> sourceWorldState)
        {
            return (_preConditions.Where(x => sourceWorldState.Contains(x)).Count() == _preConditions.Count());
        }

        /// <summary>
        ///     create a new list of cloned worldstates based on the sourceWorldstate
        ///     without the elements with same type in effect list and add effect list
        /// </summary>
        /// <param name="sourceWorldState"></param>
        /// <returns></returns>
        public List<IGoapWorldstate> GetResultingWorldstate(List<IGoapWorldstate> sourceWorldState)
        {
            List<IGoapWorldstate> resultingWorldStates = (from worldstate in _effects select worldstate.GetClone()).ToList();

            List<Type> typesInEffectList = (from worldstate in _effects select worldstate.GetType()).ToList();

            resultingWorldStates.AddRange(from worldState in sourceWorldState
                                          where !typesInEffectList.Contains(worldState.GetType())
                                          select worldState.GetClone());

            return resultingWorldStates;
        }

        public abstract bool ValidateContextPreconditions();

        public abstract bool ExecuteContextEffects();

        public abstract bool Execute();
    }
}
