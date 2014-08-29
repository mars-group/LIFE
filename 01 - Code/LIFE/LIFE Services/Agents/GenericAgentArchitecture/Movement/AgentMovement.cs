using System.Dynamic;
using ESCTestLayer.Entities;
using ESCTestLayer.Interface;

namespace GenericAgentArchitecture.Movement {

  /// <summary>
  ///   This abstract class serves as a base for agent movement.
  ///   It interfaces the Environment Service Component (ESC).
  /// </summary>
  internal abstract class AgentMovement {
    
    private readonly IESC _esc;     // Environment Service Component interface for collision detection.
    private readonly int _agentId;  // Agent identifier, needed for ESC registration.

    public Vector Position  { get; private set; }   // The agent's center. 
    public Vector Direction { get; private set; }   // Direction (vectorial).
    public float  Pitch     { get; private set; }   // Direction (lateral axis).
    public float  Yaw       { get; private set; }   // Direction (vertical axis).


    /// <summary>
    ///   Instantiate a new base class. Only available for specializations.
    /// </summary>
    protected AgentMovement(IESC esc, int agentId, Vector startPos, Vector direction, Vector dimension) {
      _esc = esc;
      _agentId = agentId;
      esc.Add(_agentId, new Vector3f(0,0,0)); //TODO Dimension: Vector cast needed. Or common .dll for data types.
    }


    /// <summary>
    ///   When the position module is destroyed, the agent is no longer physically present. 
    ///   Remove it from ESC!
    /// </summary>
    ~AgentMovement() {
      _esc.Remove(_agentId);
    }


    protected void SetPosition(Vector newPosition) {
      Position = new Vector();
    }    
    
    protected void SetDirection() {
      Direction = new Vector();
    }

    public void Move() {
      _esc.SetPosition(_agentId, null, null);  //TODO Use the set values!
    }
  }
}