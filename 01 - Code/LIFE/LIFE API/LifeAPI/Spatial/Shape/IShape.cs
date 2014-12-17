namespace LifeAPI.Spatial.Shape
{
    public interface IShape
    {
        TVector GetPosition();
        Direction GetRotation();
        Quad GetBounds();
        bool CollidesWith(IShape shape);
        IShape Transform(TVector movement, Direction rotation);
    }
}
