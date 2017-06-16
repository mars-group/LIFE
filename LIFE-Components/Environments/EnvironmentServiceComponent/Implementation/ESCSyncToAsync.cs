using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvironmentServiceComponent.SpatialAPI.Entities.Movement;
using LIFE.API.Environment;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Movement;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Shape;

namespace EnvironmentServiceComponent.Implementation
{
    public class ESCSyncToAsync : IAsyncEnvironment
    {
        private List<ISpatialEntity> _activeAgents;

        private List<Tuple<Guid, Vector3, Direction, MovementDelegate>> _ActAdds;

        private List<Tuple<Guid, Vector3, Vector3, bool, MovementDelegate>> _ActRndAdds;

        private List<Tuple<Guid, Vector3, Direction, MovementDelegate>> _actMoves;
        private List<Tuple<Guid, Action<ISpatialEntity>>> _actRemoves;

        private List<Tuple<Guid, IShape, MovementDelegate>> _actResize;

        private List<Tuple<ISpatialObject, ExploreDelegate, Type>> _actAgentExplores;

        private List<Tuple<IShape, ExploreDelegate, Enum>> _actExplores;
        private List<ExploreDelegate> _actExploreAlls;

        private ConcurrentDictionary<Guid, ISpatialEntity> _currentAgents;
        private IESC m_ESC;

        public ESCSyncToAsync(IESC p_ESC)
        {
            m_ESC = p_ESC;
            _activeAgents =new List<ISpatialEntity>();
            _ActAdds = new List<Tuple<Guid, Vector3, Direction, MovementDelegate>>();
            _ActRndAdds = new List<Tuple<Guid, Vector3, Vector3, bool, MovementDelegate>>();
            _actMoves = new List<Tuple<Guid, Vector3, Direction, MovementDelegate>>();
            _actResize = new List<Tuple<Guid, IShape, MovementDelegate>>();
            _actRemoves =new List<Tuple<Guid, Action<ISpatialEntity>>>();
            _actExploreAlls = new List<ExploreDelegate>();
            _currentAgents = new ConcurrentDictionary<Guid, ISpatialEntity>();
            _actAgentExplores = new List<Tuple<ISpatialObject, ExploreDelegate, Type>>();
            _actExplores = new List<Tuple<IShape, ExploreDelegate, Enum>>();

        }

        public ISpatialEntity GetSpatialEntity(Guid agentID) {
            ISpatialEntity tmp;
            if (!_currentAgents.ContainsKey(agentID)) {
                Debug.WriteLine("tried to get nonexisting Agent :" + agentID);
                return null;
            }
            _currentAgents.TryGetValue(agentID,out tmp);
            return tmp;
        }

        public Vector3 MaxDimension
        {
            get { return m_ESC.MaxDimension; }
            set { m_ESC.MaxDimension = value; }
        }

        public bool IsGrid { get; set; }
        private object _sync = new object();
        public void Add(ISpatialEntity entity, MovementDelegate movementDelegate) {
            _currentAgents.TryAdd(entity.AgentGuid,entity);
            lock (_sync) {
                _activeAgents.Add(entity);
                _ActAdds.Add(new Tuple<Guid, Vector3, Direction, MovementDelegate>(entity.AgentGuid, entity.Shape.Position, entity.Shape.Rotation, movementDelegate));
            }
        }

        public void AddWithRandomPosition(ISpatialEntity entity, Vector3 min, Vector3 max, bool grid, MovementDelegate movementDelegate)
        {
            _currentAgents.TryAdd(entity.AgentGuid, entity);
            lock (_sync)
            {
                _activeAgents.Add(entity);
                _ActRndAdds.Add(new Tuple<Guid, Vector3, Vector3, bool, MovementDelegate>(entity.AgentGuid, min, max, grid, movementDelegate));

            }
        }

        public void Remove(Guid agentId, Action<ISpatialEntity> removeDelegate) {
            lock (_sync) {
                _actRemoves.Add(new Tuple<Guid, Action<ISpatialEntity>>(agentId,removeDelegate));
            }
        }

        public void Resize(Guid agentId, IShape shape, MovementDelegate movementDelegate) {

            lock (_sync) {
                _actResize.Add
                    (new Tuple<Guid, IShape, MovementDelegate>
                        ( agentId , shape, movementDelegate));
            }
        }

        public void Move(Guid agentId, Vector3 movementVector, Direction rotation, MovementDelegate movementDelegate, int maxResults = 100) {
            lock (_sync) {
                _actMoves.Add
                    (new Tuple<Guid, Vector3, Direction, MovementDelegate>(agentId, movementVector, rotation, movementDelegate));

            }
        }

        public void Explore(IShape shape, ExploreDelegate exploreDelegate, Type agentType = null, Enum collisionType = null, int maxResults = 100) {

            if (agentType == null) {
                lock (_sync) {
                    _actExplores.Add(new Tuple<IShape, ExploreDelegate, Enum>(shape, exploreDelegate, collisionType));
                }
            }
            else {
                var dummy = new DummySpatialEntity {Shape = shape};
                lock (_sync) {
                    _actAgentExplores.Add(new Tuple<ISpatialObject, ExploreDelegate, Type>(dummy, exploreDelegate, agentType));
                }
            }
        }



        public void ExploreAll(ExploreDelegate exploreDelegate) {
            lock (_sync) {
                _actExploreAlls.Add(exploreDelegate);
            }
        }



        public void Commit()
        {

            lock (_sync) {
                // Add 
                foreach (Tuple<Guid, Vector3, Direction, MovementDelegate> actAdd in _ActAdds)
                {
                    if (actAdd.Item1 == null) continue;

                    if (!_currentAgents.ContainsKey(actAdd.Item1)) continue;
                    var tmp = m_ESC.Add(_currentAgents[actAdd.Item1], actAdd.Item2, actAdd.Item3);
                    actAdd.Item4?.Invoke(!tmp ? new EnvironmentResult(new List<ISpatialEntity>(), EnvironmentResultCode.COLLISION) : new EnvironmentResult(), _currentAgents[actAdd.Item1]);
                }
                // AddRnd
                foreach (var actAdd in _ActRndAdds)
                {
                    if (actAdd.Item1 == null) continue;
                    
                    if (!_currentAgents.ContainsKey(actAdd.Item1)) continue;
                    
                    bool tmp = m_ESC.AddWithRandomPosition(_currentAgents[actAdd.Item1], actAdd.Item2, actAdd.Item3, actAdd.Item4);
                    actAdd.Item5?.Invoke(!tmp ? new EnvironmentResult(new List<ISpatialEntity>() , EnvironmentResultCode.COLLISION) : new EnvironmentResult(), _currentAgents[actAdd.Item1]);
                }
                // Move
                foreach (var actMove in _actMoves.ToList())
                {
                    if (actMove.Item1 == null) continue;
                    if (!_currentAgents.ContainsKey(actMove.Item1)) continue;

                    MovementResult tmp = m_ESC.Move(_currentAgents[actMove.Item1], actMove.Item2, actMove.Item3);
                    actMove.Item4?.Invoke(new EnvironmentResult(tmp.Collisions), _currentAgents[actMove.Item1]);
                }
                // Resize 
                foreach (var act in _actResize.ToList())
                {
                    var valid = false;
                    foreach (var ag in _activeAgents.ToList())
                    {
                        if (ag.AgentGuid == act.Item1)
                        {
                            valid = true;
                            break;
                        }
                    }
                    if (!valid) continue;
                    bool tmp = m_ESC.Resize(_currentAgents[act.Item1], act.Item2);
                    act.Item3?.Invoke(!tmp ? new EnvironmentResult(new List<ISpatialEntity>(), EnvironmentResultCode.COLLISION) : new EnvironmentResult(), _currentAgents[act.Item1]);
                }
                // Explore 
                foreach (var act in _actExplores.ToList())
                {
                    var tmp = m_ESC.Explore(act.Item1, act.Item3);
                    act.Item2?.Invoke(new EnvironmentResult(tmp, EnvironmentResultCode.OK));
                }
                
                foreach (var act in _actRemoves.ToList())
                {

                    try
                    {
                        if (!_currentAgents.ContainsKey(act.Item1))
                        {
                            Debug.WriteLine("tried to remove key which does not exist : " + act.Item1);
                            continue;
                        }
                    }
                    catch (System.Exception)
                    {
                        Debug.WriteLine("tried to remove key which does not exist : " + act.Item1);
                        continue;
                    }

                    act.Item2?.Invoke(_currentAgents[act.Item1]);
                    m_ESC.Remove(_currentAgents[act.Item1]);
                    _activeAgents.Remove(_currentAgents[act.Item1]);
                    ISpatialEntity tmp;
                    _currentAgents.TryRemove(act.Item1, out tmp);
                }

                // Explore Agent 
                foreach (var act in _actAgentExplores.ToList())
                {
                    var tmp = m_ESC.Explore(act.Item1, act.Item3);
                    act.Item2?.Invoke(new EnvironmentResult(tmp, EnvironmentResultCode.OK));
                }

                foreach (var act in _actExploreAlls.ToList())
                {
                    act?.Invoke(new EnvironmentResult(_activeAgents, EnvironmentResultCode.OK));
                }


                _ActAdds.Clear();
                _ActRndAdds.Clear();
                _actMoves.Clear();
                _actResize.Clear();
                _actExplores.Clear();
                _actAgentExplores.Clear();
                _actRemoves.Clear();
                _actExploreAlls.Clear();
            }
            
        }
    }
}
