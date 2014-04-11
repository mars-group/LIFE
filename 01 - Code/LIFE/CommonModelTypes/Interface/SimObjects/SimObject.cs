namespace CommonModelTypes.Interface.SimObjects
{
    public abstract class SimObject
    {
        public int ID { get; private set; }
        public Vector3D Position { get; private set; }

        public SimObject(int id, Vector3D position) {
            ID = id;
            Position = position;
        }

    }
}
