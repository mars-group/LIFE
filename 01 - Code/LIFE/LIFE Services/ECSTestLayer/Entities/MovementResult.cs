using System;
using System.Collections.Generic;
using CommonTypes.DataTypes;

namespace ESCTestLayer.Entities
{
    public class MovementResult
    {
        public Vector Position { get; private set; }

        public Dictionary<String, Object> Information { get; private set; }

        public MovementResult(Vector position, Dictionary<String, Object> information)
        {
            this.Position = position;
            this.Information = information;
        }

        public MovementResult(Vector position) :
            this(position, new Dictionary<String, Object>())
        {
        }
    }
}
