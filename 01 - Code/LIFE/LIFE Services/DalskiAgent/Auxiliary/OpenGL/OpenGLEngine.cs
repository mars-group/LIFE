using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace DalskiAgent.Auxiliary.OpenGL {
  
  public class OpenGLEngine {

    private readonly GameWindow _window;
    private readonly Camera _camera; 
    private Matrix4 _cameraMatrix;
    private readonly TextWriter _writer;
    private bool _execFlag;

    public readonly List<IDrawable> Objects;
    

    /// <summary>
    ///   3D Engine constructor.
    ///   All logic is assigned directly through delegate functions.
    /// </summary>
    public OpenGLEngine() {

      _window = new GameWindow(640, 480, new GraphicsMode(), "3D Engine V2");
      _writer = new TextWriter(_window);
      _camera = new Camera(_window, _writer, 5, 5, 12, -89.9f);
      Objects = new List<IDrawable>();

      // OpenGL initialization.
      _window.Load += delegate {
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);                          // Set clearing color.
        GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest); // Highest quality desired.
        GL.ShadeModel(ShadingModel.Smooth); // Shading type 'smooth' (def.). Other option: 'flat'. 
        GL.Enable(EnableCap.DepthTest);     // Enable depth test (to avoid rendering hidden points).
        GL.DepthFunc(DepthFunction.Lequal); // Only render points with less or equal depth (def.: GL_LESS). 
        GL.ClearDepth(1.0f);                // Set depth buffer clearing value to maximum. 
        _window.VSync = VSyncMode.On;       // Enable screen V-sync.
        _cameraMatrix = Matrix4.Identity;   // Initialize camera with identity matrix.
      };

      // Update the OpenGL viewport and adjust the user perspective.
      _window.Resize += delegate {
        GL.Viewport(_window.ClientRectangle.X, _window.ClientRectangle.Y,
          _window.ClientRectangle.Width, _window.ClientRectangle.Height);
        var proj = Matrix4.CreatePerspectiveFieldOfView(
          60*0.01745f, // FOV (in radians). 
          (float) _window.Width/_window.Height, // Aspect ratio.
          1.0f, 2000f); // Near and far pane.
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadMatrix(ref proj);
      };

      _window.Visible = true;
      _execFlag = true;
    }


    private void UpdateFrame() {
      if (_window.Keyboard[Key.Escape]) _execFlag = false;
      if (_window.Keyboard[Key.Number1]) SetResolution(640, 480);
      if (_window.Keyboard[Key.Number2]) SetResolution(1024, 600);
      if (_window.Keyboard[Key.Number3]) SetResolution(-1, -1, true);
      _camera.Update(out _cameraMatrix);      
    }


    private void RenderFrame() {
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); // Clear current buffer.
      GL.MatrixMode(MatrixMode.Modelview);      // Change to model view.
      GL.LoadMatrix(ref _cameraMatrix);         // Load camera view.
      foreach (var obj in Objects) obj.Draw();  // Draw the world.
      _writer.Draw();                           // Write text to screen.
      _window.SwapBuffers();                    // Swap active and standby buffers.
    }


    public bool Run() {
      _window.ProcessEvents();
      UpdateFrame();
      RenderFrame();
      if (!_execFlag) {
        _window.Exit();
        _window.Visible = false;
      }
      return _execFlag;
    }


    /// <summary>
    ///   Set the window resolution.
    /// </summary>
    /// <param name="width">New window width.</param>
    /// <param name="height">New window height.</param>
    /// <param name="fullscreen">Fullscreen flag. If set, w/h are ignored.</param>
    public void SetResolution (int width, int height, bool fullscreen = false) {
      if (fullscreen) _window.WindowState = WindowState.Fullscreen;
      else {
        _window.WindowState = WindowState.Normal;
        _window.Width = width;
        _window.Height = height;
      }
    }
  }
}
