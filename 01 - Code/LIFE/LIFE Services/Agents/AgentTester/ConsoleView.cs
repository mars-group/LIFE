using System;
using System.Collections.Generic;
using CommonTypes.DataTypes;
using GenericAgentArchitecture.Agents;

namespace AgentTester {
  
  class ConsoleView {

    private static List<string> _messages;
    private ConsoleInitData _cid;
    private int _width, _height;   // Console window dimensions.

    private Dictionary<SpatialAgent, Vector> _agents; 

    public ConsoleView() {
      _messages = new List<string>();

      _width = 78;

      BuildHeaderStrings();
      _messages.Add("Dies ist ein Test!");
      _messages.Add("Noch etwas!");
      _messages.Add("Hier kommt nun etwas gaaaaaaanz langes. Und immer ist noch nicht schluß. Aber nun.");

      PrintMessages();
      Console.ReadLine();
    }





    public void Print() {
      if (_cid.MessageSize > 0) PrintMessages(); 
    }


    private void PrintEnvironment () { }
    private void PrintAgents () { }
    private void PrintInfo () { }


    private int _messageStart = 0;

    private void PrintMessages() {
      Console.SetCursorPosition(0, _messageStart);
      Console.WriteLine(_messageHeader);
      for (int i = 0; i < _messages.Count; i++) { 
        Console.Write("│ ");
        for (int j = 2, c = 0; j < _width - 1; j++, c++) {
          if (c < _messages[i].Length) Console.Write(_messages[i][c]);
          else                         Console.Write(" ");
        }
        Console.WriteLine("│");
      }
      Console.WriteLine(_messageTail);
    }



    private string _messageHeader;
    private string _messageTail;

    private void BuildHeaderStrings() {

      // ------------ Build message header and tail line. ------------
      const string msgTitle = "NACHRICHTEN";
      int msgLineMissing = _width - msgTitle.Length - 2;
      int msgLineLeft = msgLineMissing/2;
      int msgLineRight = msgLineMissing/2  + msgLineMissing%2;

      _messageHeader = "┌";
      for (int i = 0; i < msgLineLeft; i ++) _messageHeader += "─";
      _messageHeader += msgTitle;
      for (int i = 0; i < msgLineRight; i ++) _messageHeader += "─";
      _messageHeader += "┐";

      _messageTail = "└";
      for (int i = 0; i < _width-2; i ++) _messageTail += "─";
      _messageTail += "┘";
    }



    private void ResizeConsole(int x, int y) {
      _width += x;
      _height += y;
      if (_width > 190) _width = 190;
      Console.SetWindowSize(_width, _height);
      Console.SetBufferSize(_width, _height);
    }
  }


  struct ConsoleInitData {
    public string AgentsHeader;
    

    public int MessageSize;
    public int MapX, MapY;
  }
}
