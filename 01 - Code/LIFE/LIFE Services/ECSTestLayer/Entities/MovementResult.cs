using System;
using System.Collections.Generic;

namespace ESCTestLayer.Entities
{
    public class MovementResult
    {
        public Vector3f Position { get; private set; }

        public Dictionary<String, Object> Information { get; private set; }

        public MovementResult(Vector3f position, Dictionary<String, Object> information)
        {
            this.Position = position;
            this.Information = information;
        }

        public MovementResult(Vector3f position) :
            this(position, new Dictionary<String, Object>())
        {
        }
    }
}
