using System;
using System.Windows;

namespace CSharpQuadTree
{
    public interface IQuadObject
    {
        Rect Bounds { get; }
        event EventHandler BoundsChanged;
    }
}