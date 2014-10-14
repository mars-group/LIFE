namespace LayerAPI.Interfaces {
    using CommonTypes.DataTypes;

    public interface ISpatialEntity  {

        IGeometry GetBounds();

    }
}