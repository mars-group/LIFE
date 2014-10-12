using System;
using System.Collections.Generic;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Environments;

namespace AgentTester {
  
  /// <summary>
  ///   This class offers a simple agent/environment view in a console window.
  ///   It is suitable for grid-based 2D environments and agents based on the GAA. 
  /// </summary>
  class ConsoleView {
    
    private ConsoleInitData _cid;   // Initialization data object.
    private IEnvironment _env;      // The environmental reference.
    private int _width, _height;    // Current console window dimensions.
    const int EnvTop = 2;           // Row of environment map beginning (top).

    private List<Pos> _agentPos;         // List of currently occupied positions.
    private List<SpatialAgent> _agents;  // List of all agents that are printed.
    private int _agentsLeft;             // Console column of agent output start.
    private int _agtCountLast;           // Number of agents during last display.
    private readonly int _agtListMin;    // Minimum length of list (at least MapY+1).
    private readonly int _agtListMax;    // Maximum length of list (at least = min).

    private const string AgtOverflow = "Einträge wurden abgeschnitten.";
    private const string MsgTitle = "NACHRICHTEN";

    private int _messageTop;                    // Console row of message frame. 
    private string _msgHead, _msgBox, _msgTail; // Formatted lines for message box.
    private static bool _msgChanged;            // Flag for message queue change.
    private static int _msgLines;               // Lines of messages to display.
    private static List<string> _msgTexts;      // Message text list.
    private static List<int> _msgColors;        // Message color code list.


    /// <summary>
    ///   Creates a new console view by initializing internal lists,
    ///   resizing the console accordingly and building the window frames. 
    /// </summary>
    /// <param name="cid">The console initialization data object.</param>
    /// <param name="env">The environment to display.</param>
    public ConsoleView(ConsoleInitData cid, IEnvironment env) {     
      _msgTexts = new List<string>();
      _msgColors = new List<int>();
      _agentPos= new List<Pos>();
      _agents = new List<SpatialAgent>();
      _msgLines = cid.MessageLines;
      _cid = cid;
      _env = env;
      _agtListMin = _cid.AgtListMin;
      _agtListMax = _cid.AgtListMax;
      if (_agtListMin < _cid.MapY+1) _agtListMin = _cid.MapY+1;
      if (_agtListMax < _agtListMin) _agtListMax = _agtListMin;
      _agtCountLast = _agtListMin;
      BuildFrames();
    }


    /// <summary>
    ///   Fills the map by fetching the agent list from the environment and
    ///   printing the agents with their color and symbol to the right position.
    /// </summary>
    private void PrintEnvironment() {
    
      // First, remove all current units from the map.
      for (int i = 0; i < _agentPos.Count; i++) {
        Console.SetCursorPosition(_agentPos[i].X, _agentPos[i].Y);
        Console.Write(" ");
      }
      _agentPos.Clear();

      // Then fetch new list and iterate over all entries to print them.
      _agents = _env.GetAllAgents();    
      for (int i = 0; i < _agents.Count; i++) {
        int x = (int) _agents[i].GetPosition().X  + 1;  // Offsets: +1: skip left border,
        int y = (int) _agents[i].GetPosition().Y  + 3;  // +3: skip two headlines + border.
        _agentPos.Add(new Pos{X = x, Y = y});
        Console.SetCursorPosition(x, y);     
        if (_cid.GetColor  != null) Console.ForegroundColor = _cid.GetColor(_agents[i]);
        else                        Console.ForegroundColor = ConsoleColor.Gray;      
        if (_cid.GetSymbol != null) Console.Write(_cid.GetSymbol(_agents[i]));       
        else                        Console.Write('█');
      }
      Console.ForegroundColor = ConsoleColor.Gray;
    }


    //TODO
    private void PrintInfo () {}
 
    
    /// <summary>
    ///   Print the agent list to screen. This function also handles 
    ///   the resizement, based on minimal and maximal list size.
    /// </summary>
    private void PrintAgents() {

      // Check length of agent list to print and restrict it to min/max values. 
      int agtCountCur = (_agents.Count > _agtListMax) ? _agtListMax : _agents.Count;
      if (agtCountCur < _agtListMin) agtCountCur = _agtListMin;

      // Resizing needed. Calculate offset, adjust window and start positions. Also clear lines below map!
      if (agtCountCur != _agtCountLast) {
        int offset = agtCountCur - _agtCountLast;        
        AddMessage("Last: "+_agtCountLast+", Cur: "+agtCountCur+": We have to resize window by "+offset, ConsoleColor.Yellow);
        ResizeConsole(0, offset); 
        for (int i = _cid.MapY+4; i < _height-1; i ++) PrintStringToLine(null, 0, i);
          
        //for (int i = _cid.MapY+4; i < _height-1; i ++) PrintStringToLine(null, 0, i);
        _messageTop += offset;
        PrintMessageBox();
      }

      // Print agent information.
      for (int i = 0; i < _agents.Count; i++) {
        if (i < _agtListMax) PrintStringToLine(_agents[i].ToString(), _agentsLeft, 3+i);    
        else {   // Agent list gets too long. Print cut-off string and break.
          string str = " --- " + (_agents.Count - (_agtListMax - 1)) + " " + AgtOverflow + " ---";
          PrintStringToLine(str, _agentsLeft, 2+i);
          break;          
        }
      }

      // Delete remaining old lines (if necessary).
      for (int i = _agents.Count; i <= agtCountCur; i ++) PrintStringToLine(null, _agentsLeft, 3+i);
      _agtCountLast = agtCountCur;
    }


    /// <summary>
    ///   The main printing function. It is triggered externally 
    ///   and calls subroutines to refresh the map and the lists.
    /// </summary>
    public void Print() {
      PrintEnvironment();
      if (_cid.AgentsHeader != null) PrintAgents();
      PrintInfo();
      if (_msgChanged) PrintMessages(); 
      Console.SetCursorPosition(0, 0);
    }


    /// <summary>
    ///   Add a new message to the message display queue. This function is 
    ///   'static', so it can be called in the agent cycle without reference.
    /// <param name="message">The message to display.</param>
    /// <param name="color">The message color (default: gray).</param>
    /// </summary>
    public static void AddMessage(string message, ConsoleColor color = ConsoleColor.Gray) {
      if (_msgLines == 0) return;
      if (_msgTexts.Count == _msgLines) {
        _msgTexts.RemoveAt(0);    // If full, remove oldest
        _msgColors.RemoveAt(0);   // element before inserting new.
      }
      _msgTexts.Add(message);
      _msgColors.Add((int) color);
      _msgChanged = true;
    }


    /// <summary>
    ///   Builds the command line visualization by printing environmental frame,
    ///   agent detail list and message box (optional) in a fitting window.
    /// </summary>
    private void BuildFrames() {

      // Reset width and height, set initial size.
      _width = 0; _height = 0;
      ResizeConsole(_cid.MapX+2, _cid.MapY+3 + EnvTop); // 3: 2x borders, bottom cursor row. 


      // Print scenario title.
      Console.SetCursorPosition(0, 0);
      Console.ForegroundColor = ConsoleColor.Red;
      Console.Write(" » "+_cid.Scenario);
      Console.ForegroundColor = ConsoleColor.Gray;
      

      // Build environment frame.
      Console.SetCursorPosition(0, EnvTop);
      Console.Write("┌");
      for (int i = 0; i < _cid.MapX; i ++) Console.Write("─");
      Console.Write("┐");   
      for (int y = 0; y < _cid.MapY; y++) {
        Console.SetCursorPosition(0, EnvTop+y+1); 
        Console.Write("│");
        for (int x = 0; x < _cid.MapX; x ++) Console.Write(" ");
        Console.Write("│");
      }
      Console.SetCursorPosition(0, EnvTop+_cid.MapY+1);
      Console.Write("└");
      for (int i = 0; i < _cid.MapX; i ++) Console.Write("─");
      Console.Write("┘");


      // Build agent header (if supplied).
      int agtListOffset = _agtListMin - (_cid.MapY + 1);
      if (_cid.AgentsHeader != null) {
        string h1 = _cid.AgentsHeader[0];
        string h2 = "";
        for (int hx = 0; hx < _cid.AgentsHeader[0].Length; hx++) h2 += "─";
        for (int i = 1; i < _cid.AgentsHeader.Length; i++) {
          h1 += "|" + _cid.AgentsHeader[i];
          h2 += "┼";
          for (int hx = 0; hx < _cid.AgentsHeader[i].Length; hx ++) h2 += "─";
        }
        // Resize console to fit to minimum size of display list.
        ResizeConsole(h1.Length+3, agtListOffset);  // Offset: 2 left, 1 right
        _agentsLeft = _cid.MapX + 2 + 2;
        Console.SetCursorPosition(_agentsLeft, 1);  
        Console.Write(h1);
        Console.SetCursorPosition(_agentsLeft, 2);  
        Console.Write(h2);
      }


      // Build message box.
      if (_msgLines > 0) {
        _messageTop = EnvTop + _cid.MapY + agtListOffset + 3; // 2x borders and area, 1x spacing.
        
        int msgLineM = _width - MsgTitle.Length - 2;
        int msgLineL = msgLineM/2;
        int msgLineR = msgLineM/2  + msgLineM%2;        
        _msgHead = "┌";       for (int mx = 0; mx < msgLineL; mx++) _msgHead += "─";
        _msgHead += MsgTitle; for (int mx = 0; mx < msgLineR; mx++) _msgHead += "─"; _msgHead += "┐";  
        _msgBox  = "│";       for (int mx = 0; mx < _width-2; mx++) _msgBox  += " "; _msgBox  += "│";
        _msgTail = "└";       for (int mx = 0; mx < _width-2; mx++) _msgTail += "─"; _msgTail += "┘";
            
        ResizeConsole(0, _msgLines+3); 
        PrintMessageBox();
      }
    }


    /// <summary>
    ///   Print the message box frame. This function is separated because
    ///   of replacement needs when resizing the agent list.
    /// </summary>
    private void PrintMessageBox() {  
      Console.SetCursorPosition(0, _messageTop);
      Console.Write(_msgHead);
      for (int i = 1; i <= _msgLines; i++) {
        Console.SetCursorPosition(0, _messageTop+i);
        Console.Write(_msgBox);
      }
      Console.SetCursorPosition(0, _messageTop+_msgLines+1);
      Console.Write(_msgTail);
    }

    
    /// <summary>
    ///   Print the messages to the box. Called whenever changes in the
    ///   message list occur. 
    /// </summary>
    private void PrintMessages() {
      for (int i = 0; i < _msgTexts.Count; i++) {
        Console.SetCursorPosition(0, _messageTop+i+1);
        Console.Write("│ ");
        Console.ForegroundColor = (ConsoleColor) _msgColors[i];
        for (int j = 2, c = 0; j < _width - 1; j++, c++) {
          if (c < _msgTexts[i].Length) Console.Write(_msgTexts[i][c]);
          else                         Console.Write(" ");
        }
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("│");
      }
    }


    /// <summary>
    ///   Prints a string into a command line row with respect to the line width.
    ///   The output is non-wrapping, it is cut off instead. 
    /// </summary>
    /// <param name="str">The string to print. If null, the row is filled with whitespaces.</param>
    /// <param name="curX">Cursor position (x).</param>
    /// <param name="curY">Cursor position (y).</param>
    /// <param name="offsetX">Padding to the right border (default: 0).</param>
    private void PrintStringToLine(string str, int curX, int curY, int offsetX = 0) {
      Console.SetCursorPosition(curX, curY);
      if (str != null) {
        for (int wx = 0; wx < _width - curX - offsetX; wx++) {
          if (wx < str.Length) Console.Write(str[wx]);
          else Console.Write(" ");
        }
      }
      else for (int wx = 0; wx < _width - curX - offsetX; wx++) Console.Write(" ");
    }


    /// <summary>
    ///   Resizes the console window (and buffer as well) in respect to the
    ///   supplied offset values for width and height. 
    /// </summary>
    /// <param name="x">Width offset.</param>
    /// <param name="y">Height offset.</param>
    private void ResizeConsole(int x, int y) {
      _width += x;
      _height += y;
      if (_width > 190) _width = 190;
      Console.SetWindowSize(_width, _height);
      Console.SetBufferSize(_width, _height);     
    }


    /// <summary>
    ///   Structure for saving currently occupied positions.
    /// </summary>
    private struct Pos {
      public int X, Y;
    }
  
  }


  /// <summary>
  ///   Returns the agent's color that is used to print it [delegate]. 
  /// </summary>
  /// <param name="agent">The agent reference.</param>
  /// <returns>The command line foreground color that shall be used.</returns>
  public delegate ConsoleColor GetColor(SpatialAgent agent);
  

  /// <summary>
  ///   Returns the character to print to visualize an agent [delegate].
  /// </summary>
  /// <param name="agent">The agent reference.</param>
  /// <returns>A character that symbolizes this agent type.</returns>
  public delegate char GetSymbol(SpatialAgent agent);
  

  /// <summary>
  ///   The CID container holds initialization data like header strings,
  ///   environmental extents and configuration parameters.  
  /// </summary>
  struct ConsoleInitData {  
    public string Scenario;
    public string [] AgentsHeader;
    public int MessageLines;
    public int MapX, MapY;
    public int AgtListMin, AgtListMax;

    // Delegates (function pointers) to retrieve agent color and symbol.
    public GetColor GetColor;    
    public GetSymbol GetSymbol;
  }
}
