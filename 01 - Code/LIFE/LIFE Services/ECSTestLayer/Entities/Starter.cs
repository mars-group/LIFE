using System;
using CommonTypes.DataTypes;
using ESCTestLayer.Implementation;

namespace ESCTestLayer.Entities
{
    static class Starter
    {
        public static void Main() {
            var esc = new ESC();
            var agent0 = new Agent2D(0, esc);
            var agent1 = new Agent2D(1, esc);
//            var agent2 = new Agent2D(2, esc);

            Console.WriteLine(agent0.SetPosition(new Vector(1, 1)));
            Console.WriteLine(agent1.SetPosition(new Vector(1, 1)));
            Console.WriteLine(agent1.SetPosition(new Vector(2.1f, 2.1f)));
            Console.WriteLine(agent1.SetPosition(new Vector(1, 1)));

            Console.WriteLine("Game Over");
            Console.ReadLine();
        }
    }

    internal class Agent2D
    {
        private readonly ESC _esc;
        private readonly int Id;
        private readonly Vector Dimension;
        private readonly Vector Direction;

        public Agent2D(int id, ESC esc)
        {
            _esc = esc;
            Id = id;
            Dimension = new Vector(1, 1);
            Direction = new Vector(0, 0);
            Register();
        }

        private void Register()
        {
            _esc.Add(Id, Dimension);
        }


        public bool SetPosition(Vector position)
        {
            return position.Equals(_esc.SetPosition(Id, position, Direction).Position);
        }


    }
}
