namespace ESCTest.Entities {
    using CommonTypes.DataTypes;
    using ESCTestLayer.Interface;

    class TestAgent2D {
        private const int InformationType = 1;
        private readonly IESC _esc;
        private readonly int _id;
        private readonly Vector _dimension;
        private readonly Vector _direction;

        public TestAgent2D(int id, IESC esc) {
            _esc = esc;
            _id = id;
            _dimension = new Vector(1, 1);
            _direction = new Vector(0, 0);
            Register();
        }


        private void Register() {
            _esc.Add(_id, InformationType, true, _dimension);
        }


        public bool SetPosition(Vector position) {
            return position.Equals(_esc.SetPosition(_id, position, _direction).Position);
        }
    }
}