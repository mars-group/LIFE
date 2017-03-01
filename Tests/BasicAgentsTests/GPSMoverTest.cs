using System;
using System.Globalization;
using LIFE.Components.Agents.BasicAgents.Movement;
using LIFE.Components.Agents.BasicAgents.Perception;
using LIFE.Components.ESC.Implementation;
using LIFE.Components.ESC.SpatialAPI.Entities.Movement;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Shape;
using NUnit.Framework;

namespace BasicAgentsTests {

 /* [TestFixture]
  public class GPSMoverTest {

    private IEnvironment _env;      // The default Environment Service Component.
    private AgentEntity _entity;    // A spatial object to use for probing.
    private MovementSensor _sensor; // Sensor for movement success evaluation.
    private GPSMover _mover;        // Mover instance.
    private Direction _north, _east, _west, _special, _wp; // Probing directions. Movement distance is set to 10m.
    private AgentMover _agentMover;

    [OneTimeSetUp]
    public void Init() {
      _env = new DistributedESC();
      _entity = new AgentEntity {
        CollisionType = CollisionType.Ghost,
        Shape = new Cuboid(new Vector3(1.0, 1.0, 1.0), new Vector3(), new Direction())
      };
      _agentMover = new AgentMover(_env, _entity, new SensorArray());
      _sensor = new MovementSensor();
      _mover = new GPSMover(_env, _entity, _sensor);
      _north = new Direction();
      _east = new Direction();
      _east.SetYaw(90);
      _west = new Direction();
      _west.SetYaw(270);
      _special = new Direction();
      _special.SetYaw(274.289738927);
      _wp = _agentMover.Continuous.CalculateDirectionToTarget(new Vector3(-25.17995, 0, 31.31209));
    }


    private void PlaceAndMove(Vector3 pos, Direction dir, double spd) {
      Assert.True(_env.Add(_entity, pos));
      for (var i = 0; i < 200; i++) _mover.Move(spd, dir).Execute();
      Assert.True(((MovementResult) _sensor.Sense()).Success);
      Console.WriteLine(spd*200 + ":" + _entity.Position.X.ToString(CultureInfo.InvariantCulture) + ", " +
                        _entity.Position.Z.ToString(CultureInfo.InvariantCulture));
      _env.Remove(_entity);
    }


    [Test]
    public void MoverTest1() {
      // ReSharper disable UnusedVariable
      var svalbard = new Vector3(78.7546944, 0, 16.3090938);
      var antarctica = new Vector3(-78.0806747, 0, 58.6696986);
      var colombia = new Vector3(0.1921127, 0, -75.8506319);
      var southafrica = new Vector3(-29.4396220, 0, 23.6243350);
      var czechia = new Vector3(48.8553982, 0, 17.4176795);
      var frPolynesia = new Vector3(-17.5686263, 0, -150.069726);
      var togo = new Vector3(8.485408599, 0, 0.838479995);
      // ReSharper restore UnusedVariable

      PlaceAndMove(southafrica, _north, 5);
      PlaceAndMove(southafrica, _east, 5);
      PlaceAndMove(southafrica, _west, 5);
      PlaceAndMove(southafrica, _special, 5);

      Console.WriteLine("Towards WP:");
      PlaceAndMove(southafrica, _wp, 5);
      PlaceAndMove(southafrica, _wp, 50);
      PlaceAndMove(southafrica, _wp, 500);
      PlaceAndMove(southafrica, _wp, 4450);
    }
  }*/
}