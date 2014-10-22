using System;
using System.Collections.Generic;
using GenericAgentArchitecture.Agents;

namespace GenericAgentArchitecture.Auxiliary {

  /// <summary>
  ///   This class provides really simple functions for channel-based communication.
  ///   Messages are only for one tick audible - after that, they will fade away. So the agent won't
  ///   notice any messages when it does not listen (sensor off) neither will it see messages prior 
  ///   to listening start. For example a message sent in tick 100 is just receivable in tick 101.  
  /// </summary>
  public static class MessageServer {

    /* These channel-to-messages mappings store the newly received (send by agents in execution phase) 
     * and the available (to agents in perception phase) messages. The mappings are refreshed every tick. */
    private static  Dictionary<int, List<Message>> _available = new Dictionary<int, List<Message>>();  
    private static  Dictionary<int, List<Message>> _received  = new Dictionary<int, List<Message>>();  


    /// <summary>
    ///   Receive a new message. 
    /// </summary>
    /// <param name="message">The message to queue.</param>
    public static void AddMessage(Message message) {
      if (!_received.ContainsKey(message.Channel))
        _received.Add(message.Channel, new List<Message>());
      _received[message.Channel].Add(message); 
    }


    /// <summary>
    ///   Get all available messages on a channel.  
    /// </summary>
    /// <param name="channel">The channel number to query.</param>
    /// <returns>A list of messages</returns>
    public static List<Message> GetMessages(int channel) {
      if (_available.ContainsKey(channel)) return _available[channel];
      return new List<Message>(); 
    }


    /// <summary>
    ///   Move the received messages to available and reset the received list.   
    /// </summary>
    public static void Tick() {
      _available = _received;
      _received = new Dictionary<int, List<Message>>();
    }
  }



  /// <summary>
  ///   A message container. Holds some meta data about channel, sender and 
  ///   broadcasting time and an arbitrary payload.
  /// </summary>
  public class Message {
    
    public int Channel   { get; private set; }  // Channel this message is sent on. 
    public Agent Sender  { get; private set; }  // The sending agent.
    public long SendTime { get; private set; }  // Tick of creation.
    public Object Data   { get; private set; }  // Message data.


    /// <summary>
    ///   Create a new message.
    /// </summary>
    /// <param name="channel">Channel this message shall be send on. </param>
    /// <param name="sender">Reference to the sending agent.</param>
    /// <param name="sendtime">The tick in that the message is sent.</param>
    /// <param name="data">Message payload.</param>
    public Message(int channel, Agent sender, long sendtime, Object data) {
      Channel = channel;
      Sender = sender;
      SendTime = sendtime;
      Data = data;
    }
  }
}