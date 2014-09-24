namespace ESCTest.Entities {
    using CommonTypes.DataTypes;
    using ESCTestLayer.Implementation;
    using ESCTestLayer.Interface;

    class TestAgent2D {
        private const int InformationType = 1;
        private readonly IESC _esc;
        private readonly int Id;
        private readonly Vector Dimension;
        private readonly Vector Direction;

        public TestAgent2D(int id, IESC esc) {
            _esc = esc;
            Id = id;
            Dimension = new Vector(1, 1);
            Direction = new Vector(0, 0);
            Register();
        }


        private void Register() {
            _esc.Add(Id, InformationType, true, Dimension);
        }


        public bool SetPosition(Vector position) {
            return position.Equals(_esc.SetPosition(Id, position, Direction).Position);
        }
    }
}