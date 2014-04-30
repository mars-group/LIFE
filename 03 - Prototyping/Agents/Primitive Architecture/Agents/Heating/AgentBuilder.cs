namespace Primitive_Architecture.Agents.Heating {

  /// <summary>
  /// This builder provides a convenient creation of agents.
  /// </summary>
  class AgentBuilder {

    private HeaterAgent _heater;

    public TempAgent BuildTempAgent() {

      if (_heater == null) return null;

      var agent = new TempAgent(null, null, null);
      return agent;
    }

  }
}
