using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace DalskiAgent.Auxiliary.OpenGL {
  
  internal class TextWriter {

    private readonly GameWindow _window;
    private readonly Font _textFont = new Font(FontFamily.GenericSansSerif, 8);
    private readonly Bitmap _textBitmap;
    private readonly List<PointF> _positions;
    private readonly List<string> _lines;
    private readonly List<Brush> _colours;
    private readonly int _textureId;


    public TextWriter(GameWindow window) {
      _window = window;
      _positions = new List<PointF>();
      _lines = new List<string>();
      _colours = new List<Brush>();
      _textBitmap = new Bitmap(_window.Width, _window.Height);
      _textureId = CreateTexture();
    }


    public void Update(int ind, string newText) {
      if (ind < _lines.Count) {
        _lines[ind] = newText;
        UpdateText();
      }
    }


    private int CreateTexture() {
      int textureId;
      GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float) TextureEnvMode.Replace);
      //Important, or wrong color on some computers
      Bitmap bitmap = _textBitmap;
      GL.GenTextures(1, out textureId);
      GL.BindTexture(TextureTarget.Texture2D, textureId);

      BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
        PixelFormat.Format32bppArgb);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
        OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
      //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
      //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
      GL.Finish();
      bitmap.UnlockBits(data);
      return textureId;
    }


    public void Dispose() {
      if (_textureId > 0)
        GL.DeleteTexture(_textureId);
    }


    public void Clear() {
      _lines.Clear();
      _positions.Clear();
      _colours.Clear();
    }


    public void AddLine(string s, PointF pos, Brush col) {
      _lines.Add(s);
      _positions.Add(pos);
      _colours.Add(col);
      UpdateText();
    }


    public void UpdateText() {
      if (_lines.Count > 0) {
        using (Graphics gfx = Graphics.FromImage(_textBitmap)) {
          gfx.Clear(Color.Black);
          gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
          for (int i = 0; i < _lines.Count; i++) {
            gfx.DrawString(_lines[i], _textFont, _colours[i], _positions[i]);
          }
        }

        BitmapData data = _textBitmap.LockBits(new Rectangle(0, 0, _textBitmap.Width, _textBitmap.Height),
          ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, _textBitmap.Width, _textBitmap.Height,
          OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
        _textBitmap.UnlockBits(data);
      }
    }


    public void Draw() {
      GL.PushMatrix();
      GL.LoadIdentity();

      var orthoProjection = Matrix4.CreateOrthographicOffCenter(0, _window.Width, _window.Height, 0, -1, 1);
      GL.MatrixMode(MatrixMode.Projection);

      GL.PushMatrix(); //
      GL.LoadMatrix(ref orthoProjection);

      GL.Enable(EnableCap.Blend);
      GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.DstColor);
      GL.Enable(EnableCap.Texture2D);
      GL.BindTexture(TextureTarget.Texture2D, _textureId);


      GL.Begin(PrimitiveType.Quads);
      GL.TexCoord2(0, 0);
      GL.Vertex2(0, 0);
      GL.TexCoord2(1, 0);
      GL.Vertex2(_textBitmap.Width, 0);
      GL.TexCoord2(1, 1);
      GL.Vertex2(_textBitmap.Width, _textBitmap.Height);
      GL.TexCoord2(0, 1);
      GL.Vertex2(0, _textBitmap.Height);
      GL.End();
      GL.PopMatrix();

      GL.Disable(EnableCap.Blend);
      GL.Disable(EnableCap.Texture2D);

      GL.MatrixMode(MatrixMode.Modelview);
      GL.PopMatrix();
    }
  }
}