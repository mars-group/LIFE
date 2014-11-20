namespace ESCTest.Entities {
    using CommonTypes.TransportTypes;
    using ESCTestLayer.Interface;

    internal class TestDeprecatedAgent2D {
        private const int InformationType = 1;
        private readonly IDeprecatedESC _esc;
        private readonly int _id;
        private readonly TVector _dimension;
        private readonly TVector _direction;

        public TestDeprecatedAgent2D(int id, IDeprecatedESC esc) {
            _esc = esc;
            _id = id;
            _dimension = new TVector(1, 1);
            _direction = new TVector(0, 0);
            Register();
        }


        private void Register() {
            _esc.Add(_id, InformationType, true, _dimension);
        }


        public bool SetPosition(TVector position) {
            return position.Equals(_esc.SetPosition(_id, position, _direction).Position);
        }
    }
}