using System;
using ESCTestLayer.Implementation;

namespace ESCTestLayer.Entities
{
    internal class Starter
    {
        public static void Main() {
            var esc = new ESC();
            var agent0 = new Agent2D(0, esc);
            var agent1 = new Agent2D(1, esc);
            var agent2 = new Agent2D(2, esc);

            Console.WriteLine(agent0.SetPosition(new Vector2f(1, 1)));
            Console.WriteLine(agent1.SetPosition(new Vector2f(1, 1)));
            Console.WriteLine(agent1.SetPosition(new Vector2f(2.1f, 2.1f)));
            Console.WriteLine(agent1.SetPosition(new Vector2f(1, 1)));

            Console.WriteLine("Game Over");
            Console.ReadLine();
        }
    }

    internal class Agent2D
    {
        private readonly ESC _esc;
        public int id { get; set; }
        public Vector2f dimension { get; set; }
        public Vector2f direction { get; set; }
        public Vector2f positio { get; set; }

        public Agent2D(int id, ESC esc)
        {
            _esc = esc;
            this.id = id;
            this.dimension = new Vector2f(1, 1);
            this.direction = new Vector2f(0, 0);
            Register();
        }

        private void Register()
        {
            _esc.Add(id, dimension);
        }


        public bool SetPosition(Vector2f position)
        {
            return _esc.SetPosition(id, position, direction);
        }


    }
}
