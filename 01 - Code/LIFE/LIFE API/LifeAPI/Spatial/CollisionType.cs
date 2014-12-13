namespace LifeAPI.Spatial {

    /// <summary>
    ///     The collision type defines different groups for that a collision behaviour is defined. Therefore see the CollisionMatrix.
    /// </summary>
    public enum CollisionType
    {
        Ghost,
        SelfCollision,
        StaticEnvironment,
        MassiveAgent,
    }

}