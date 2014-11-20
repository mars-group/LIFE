using System;
using System.Windows;

namespace CSharpQuadTree
{
    public interface IQuadObject
    {
        Rect Bounds { get; set; } //IntersectsWith, X, Y, Width, Height
        event EventHandler BoundsChanged;
    }
}