using System;
using System.Windows;
using SpatialCommon.Shape;

namespace OctreeFlo.Implementation
{
    public interface IQuadObjectFlo
    {
        BoundingBox Bounds { get; set; } //IntersectsWith, X, Y, Width, Height
        event EventHandler BoundsChanged;
    }
}