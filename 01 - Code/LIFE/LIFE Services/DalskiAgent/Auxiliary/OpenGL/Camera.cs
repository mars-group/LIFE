using System;
using System.Drawing;
using OpenTK;
using OpenTK.Input;

namespace DalskiAgent.Auxiliary.OpenGL {

/* Axis definition in OpenGL:
  x-Axis: (-) ← left  |  right → (+)
  y-Axis: (-) ↓ down  |     up ↑ (+)
  z-Axis: (-)  farer  |  nearer  (+)
  [right-handed coordinate system]
  // not to be confused with pixels on screen [(0,0) = (top,left)]
*/

  class Camera {

    private readonly GameWindow _window;     // Pointer to the window.
    private readonly TextWriter _textWriter; // Text writer reference for data output.
    private Vector3 _position;               // Coordinates of the camera position.
    private float _pitch, _yaw;              // Pitch (up/down) and yaw (turn around).
    private Point _mousePrev;                // Coordinates of mouse position for locking. 
    private readonly MouseButton _rotButton; // Button used for camera rotation.

    private const float SpeedMovement = 0.05f;  // Movement (displacement) factor.
    private const float SpeedZoom     = 0.20f;  // Zooming factor (mousewheel adjustment).
    private const float SpeedPitch    = 0.30f;  // Pitch adjustment factor.
    private const float SpeedYaw      = 0.45f;  // Turning (yaw) factor.

    private const float DegToRad = 0.0174532925f;   // Degree-to-radians conversion ratio.

    private bool _kbdOn;    // Enabled flag for camera input from keyboard.
    private bool _mouseOn;  // Enabled flag for mouse input.
    private bool _refresh;  // Refresh flag for mouse re-adjustment.
    private bool _rotation; // Set to 'true' when rotation mode is enabled.


    /// <summary>
    ///   Camera constructor call.
    /// </summary>
    /// <param name="win">Main window reference.</param>
    /// <param name="tw">Text writer reference for data output.</param>
    /// <param name="x">X coordinate of initial position (def.: 0).</param>
    /// <param name="y">Y coordinate of initial position (def.: 0).</param>
    /// <param name="z">Z coordinate of initial position (def.: 0).</param>
    /// <param name="pitch">Default pitch value (def.: 0).</param>
    /// <param name="yaw">Default yaw value.</param>
    public Camera (GameWindow win, TextWriter tw, float x = 0f, float y = 0f, float z = 0f, float pitch = 0f, float yaw = 0.0f) {
      _window = win;
      _textWriter = tw;
      _position = new Vector3(x, y, z);
      _pitch = pitch;
      _yaw = yaw;
      _rotButton = MouseButton.Right;
      _kbdOn = true;
      _mouseOn = true;
      
      // Set mouse cursor into center.
      _mousePrev = _window.PointToScreen(new Point(_window.Width/2, _window.Height/2));
      Mouse.SetPosition(_mousePrev.X, _mousePrev.Y);
      
      // Reflect window focus changes to camera.
      _window.FocusedChanged += (sender, e) => {
        _kbdOn = _window.Focused;
        _mouseOn = _window.Focused;
      };


      // Set event handlers for event handlers. Yep, it sounds weird ...
      _window.MouseEnter += OnMouseEnter;
      _window.MouseLeave += OnMouseLeave;
    }


    /// <summary>
    ///   Keyboard evaluation handler.
    /// </summary>
    private void KeyboardHandler() {
      if (!_kbdOn) return;
      if (_window.Keyboard[Key.Plus]  || _window.Keyboard[Key.KeypadPlus])  MoveCamera(0, 0, 1);
      if (_window.Keyboard[Key.Minus] || _window.Keyboard[Key.KeypadMinus]) MoveCamera(0, 0,-1);
      if (_window.Keyboard[Key.Up]    || _window.Keyboard[Key.W])           MoveCamera( 0, 3, 0);
      if (_window.Keyboard[Key.Left]  || _window.Keyboard[Key.A])           MoveCamera(-3, 0, 0);
      if (_window.Keyboard[Key.Down]  || _window.Keyboard[Key.S])           MoveCamera( 0,-3, 0);
      if (_window.Keyboard[Key.Right] || _window.Keyboard[Key.D])           MoveCamera( 3, 0, 0);
    }


    /// <summary>
    ///   The mouse move event. It is used for camera movement and rotation. 
    /// </summary>
    /// <param name="sender">Sender instance of thrown event.</param>
    /// <param name="e">The event argument with mouse position and deltas.</param>
    private void OnMouseMove(object sender, MouseMoveEventArgs e) {
      if (!_mouseOn) return;

      if (_refresh) {  // Re-adjust mouse location comparison buffer.
        _mousePrev = _window.PointToScreen(new Point(e.X, e.Y));
        _refresh = false;
      }

      // Calculate offset values.
      int dx = Mouse.GetCursorState().X - _mousePrev.X;
      int dy = Mouse.GetCursorState().Y - _mousePrev.Y;

      // Mouse wheel is pressed: Rotation mode activated.
      if (_window.Mouse[_rotButton]) {
        _pitch -= dy*SpeedPitch;
        _yaw += dx*SpeedYaw;

        // Check calculated values and adjust, if out of valid range.
        if (_pitch > 45f) _pitch = 45f;
        if (_pitch <= -90f) _pitch = -89.99f; //TODO Zeichenfehler bei -90°. Warum???
        if (_yaw < 0f) _yaw += 360f;
        if (_yaw >= 360f) _yaw -= 360f;

        // Reset mouse pointer to locked position.
        Mouse.SetPosition(_mousePrev.X, _mousePrev.Y);
      }

      // Camera movement.
      else {
        MoveCamera(dx, -dy, 0);
        _mousePrev.X += dx;
        _mousePrev.Y += dy;

        //TODO Idee: Rahmenbereiche schaffen. 
      }
    }


    /// <summary>
    ///   Moves the camera around.
    /// </summary>
    /// <param name="xIn">X delta input (mouse x-axis, -: left, +: right).</param>
    /// <param name="yIn">Y delta input (mouse y-axis, -: down, +: up).</param>
    /// <param name="zIn">Z delta input (zoom level,   -: out,  +: in).</param>
    private void MoveCamera(int xIn, int yIn, int zIn) {
      
      //TODO Positionsanpassung anhand der zulässigen Grenzwerte!

      // Horizontal displacement.
      if (xIn != 0 || yIn != 0) {
        
        /* Calculation of position with orientation (yaw) taken into account:
         * 
         *       0   90  180  270                      ↓↓↓                                      0    90   180   270 
         * xPos  x    y   -x   -y   → cos(x) + sin(y)      xDelta*cos(yaw) + yDelta*sin(yaw)   1,0  0,1  -1,0  0,-1
         * yPos  y   -x   -y    x   → sin(x) + cos(y)  (-) xDelta*sin(yaw) + yDelta*cos(yaw)   0,1  1,0  0,-1  -1,0      
         *
         * Attention: yDelta of event is inverse (up=negative values)!        
         */

        float yawRad = _yaw*DegToRad;
        _position.X += (float) (SpeedMovement*( xIn*Math.Cos(yawRad) + yIn*Math.Sin(yawRad)));
        _position.Y += (float) (SpeedMovement*(-xIn*Math.Sin(yawRad) + yIn*Math.Cos(yawRad)));
      }

      // Zooming mode.
      if (zIn != 0) {
        _position.Z += zIn*SpeedZoom;
      }
    }


    // Redraw event handlers, if cursor is out-of-window.
    private void OnMouseLeave(object sender, EventArgs e) {    
      Console.WriteLine("lost");
      _window.MouseMove -= OnMouseMove;
      _window.MouseWheel -= OnMouseWheel;
      _window.Mouse.ButtonDown -= OnMouseButtonDown;
      _window.Mouse.ButtonUp -= OnMouseButtonUp;   
    }

    // Activate mouse event handlers on active cursor.
    private void OnMouseEnter(object sender,EventArgs e) {
      Console.WriteLine("gained");
      _refresh = true;
      _window.MouseMove += OnMouseMove;
      _window.MouseWheel += OnMouseWheel;
      _window.Mouse.ButtonDown += OnMouseButtonDown;
      _window.Mouse.ButtonUp += OnMouseButtonUp;     
    }


    private void OnMouseWheel(object sender, MouseWheelEventArgs e) {
      MoveCamera(0, 0, e.Delta);
    }


    // Rotation mode enabled. Save current position and hide pointer.
    private void OnMouseButtonDown(object sender, MouseButtonEventArgs e) {     
      if (_window.Mouse[_rotButton]) {
        if (!_rotation) {
          Console.WriteLine("turn off");
          _window.MouseLeave -= OnMouseLeave;
          _window.MouseEnter -= OnMouseEnter;
          _window.CursorVisible = false; 
          _rotation = true;  
        }               
      }
    }


    // Rotation mode disabled. Make it visible again!
    private void OnMouseButtonUp(object sender, MouseButtonEventArgs e) {      
      if (!_window.Mouse[_rotButton] && _rotation) { 
        Console.WriteLine("turn on");
        _window.CursorVisible = true;
        _window.MouseLeave += OnMouseLeave;
        _window.MouseEnter += OnMouseEnter;
        _refresh = true;
        _rotation = false;
      }
    }


    /// <summary>
    ///   Camera update function.
    /// </summary>
    /// <param name="cameraMatrix">Camera matrix to set.</param>
    public void Update (out Matrix4 cameraMatrix) {
      KeyboardHandler();
      
      // Calculate target view point of position and rotation.
      var pitchRad = _pitch*DegToRad;
      var yawRad = _yaw*DegToRad;
      
      var x = (float)(Math.Cos(pitchRad)*Math.Sin(yawRad));
      var y = (float)(Math.Cos(pitchRad)*Math.Cos(yawRad));
      var z = (float)(Math.Sin(pitchRad));    
 
      var target = new Vector3(_position.X+x, _position.Y+y, _position.Z+z);
      cameraMatrix = Matrix4.LookAt(_position, target, Vector3.UnitZ);
     
      // Write positioning data also directly to screen.
      var l1 = "Position: ("+(int)_position.X+","+(int)_position.Y+","+(int)_position.Z+")";
      var l2 = "Drehung: "+(int)_yaw+"°";
      var l3 = "Steigung: "+(int)_pitch+"°";
      var l4 = "Auflösung: " + _window.Width + " x " + _window.Height;//+" @ "+FPSCounter.GetFps()+" FPS";
      _textWriter.Clear();
      _textWriter.AddLine(l1,new PointF(5,5),  Brushes.White);
      _textWriter.AddLine(l2,new PointF(5,20), Brushes.White);
      _textWriter.AddLine(l3,new PointF(5,35), Brushes.White);
      _textWriter.AddLine(l4,new PointF(5,50), Brushes.White);
    }
  }
}
