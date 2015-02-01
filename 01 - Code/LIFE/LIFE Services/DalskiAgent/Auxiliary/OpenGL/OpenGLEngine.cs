using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace DalskiAgent.Auxiliary.OpenGL {

  /// <summary>
  ///   A simple OpenGL 3D engine to visualize the environment and its objects.
  /// </summary>
  public class OpenGLEngine {

    private readonly Camera _camera;          // The user's view.
    private readonly GameWindow _window;      // The main window.
    private TextWriter _writer;               // Text writer class. 
    public readonly List<IDrawable> Objects;  // The objects to draw.


    /// <summary>
    ///   3D Engine constructor.
    ///   All logic is assigned directly through delegate functions.
    /// </summary>
    public OpenGLEngine(int width, int height) {
      _window = new GameWindow(width, height, new GraphicsMode(), "3D Engine V2");
      _writer = new TextWriter(_window);
      _camera = new Camera(_window, _writer, 15, 10, 21, -90);
      Objects = new List<IDrawable>();

      // OpenGL initialization.
      GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);                          // Set clearing color.
      GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest); // Highest quality desired.
      GL.ShadeModel(ShadingModel.Smooth);   // Shading type 'smooth' (def.). Other option: 'flat'. 
      GL.Enable(EnableCap.DepthTest);       // Enable depth test (to avoid rendering hidden points).
      GL.DepthFunc(DepthFunction.Lequal);   // Only render points with less or equal depth (def.: GL_LESS). 
      GL.ClearDepth(1.0f);                  // Set depth buffer clearing value to maximum. 
      _window.VSync = VSyncMode.On;         // Enable screen V-sync.
      _window.Visible = true;               // Set window to visible.
      SetResolution(width, height);         // Initialize window with desired resolution.
    }


    /// <summary>
    ///   Engine execution function (triggered externally).
    /// </summary>
    /// <returns>Loop execution flag. Is 'false', if engine terminated.</returns>
    public bool Run() {        
      _window.ProcessEvents();
      if (!_window.Exists) return false;
      UpdateFrame();
      RenderFrame();
      return true;
    }


    /// <summary>
    ///   Set the window resolution.
    /// </summary>
    /// <param name="width">New window width.</param>
    /// <param name="height">New window height.</param>
    /// <param name="fullscreen">Fullscreen flag. If set, w/h are ignored.</param>
    private void SetResolution (int width, int height, bool fullscreen = false) { 
      if (fullscreen) _window.WindowState = WindowState.Fullscreen;
      else {
        _window.WindowState = WindowState.Normal;
        _window.Width = width + 16;
        _window.Height = height + 38;       
      }

      // Create new writer.
      //TODO Not so great! The resize function isn't either!
      //_writer = new TextWriter(_window);

      // Update the OpenGL viewport and adjust the user perspective.
      GL.Viewport(_window.ClientRectangle);
      var proj = Matrix4.CreatePerspectiveFieldOfView(
        60*0.01745f,                          // FOV (in radians). 
        (float) _window.Width/_window.Height, // Aspect ratio.
        1.0f, 2000f);                         // Near and far pane.
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadMatrix(ref proj);
    }


    /// <summary>
    ///   Input processing function.
    /// </summary>
    private void UpdateFrame() {
      if (_window.Keyboard[Key.Escape]) _window.Exit();
      if (_window.Keyboard[Key.Number1]) SetResolution(640, 480);
      if (_window.Keyboard[Key.Number2]) SetResolution(1024, 600);
      if (_window.Keyboard[Key.Number3]) SetResolution(-1, -1, true);
    }


    /// <summary>
    ///   OpenGL rendering function. 
    /// </summary>
    private void RenderFrame() {
      GL.Clear(ClearBufferMask.ColorBufferBit | // Clear current buffer. 
               ClearBufferMask.DepthBufferBit); // Clear depth buffer.
      GL.MatrixMode(MatrixMode.Modelview);      // Change to model view.
      GL.LoadIdentity();                        // Reset model matrix.
      _camera.Update();                         // Load camera view.
      foreach (var obj in Objects) obj.Draw();  // Draw the world.
      _writer.Draw();                           // Write text to screen.
      _window.SwapBuffers();                    // Swap active and standby buffers.       
    }
  }
}
