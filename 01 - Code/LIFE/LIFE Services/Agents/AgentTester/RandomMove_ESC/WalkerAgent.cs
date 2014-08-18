using ESCTestLayer;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Dummies;
using GenericAgentArchitecture.Interfaces;
using GenericAgentArchitecture.Perception;

namespace AgentTester.RandomMove_ESC {
  
  internal class WalkerAgent : Agent, IAgentLogic {

    private ESC _esc;

    public WalkerAgent(string id, ESC esc) : base(id) {
      _esc = esc;  // Set ESC reference.
      //PerceptionUnit.AddSensor(new AgentSensor());
    } 


    /// <summary>
    ///   The agent's main reasoning method. Consists of two different logics. 
    /// </summary>
    /// <returns>The interaction to execute.</returns>
    public IInteraction Reason() {
      return Id.Equals("WA-2") ? 
        ChaseNearestAgent() : 
        RandomMove2D (new Float3(10, 10, 0));
    }




/*  WIE BEWEGT DER AGENT SICH?
  (und wer hält die ESC-Referenz?)

1) Richtung ändern, Geschwindigkeit ändern.
 * - geschieht in der Planungslogik
 * - relative Änderung der Werte aus Position
 * - man kann auch zwei Planungslogiken machen: random moving / chasing
 * 
2) Position berechnen
   - DirectedMoveInteraction (Position agentPos)



*/




    /// <summary>
    /// 
    /// </summary>
    /// <param name="boundary">Maximum position (top-right). </param>
    private IInteraction RandomMove2D (Float3 boundary) {
      return null;
    }

    private IInteraction ChaseNearestAgent() {
      return null;
    }
  }
}