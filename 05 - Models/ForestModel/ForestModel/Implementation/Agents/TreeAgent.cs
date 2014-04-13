using System;
using CommonModelTypes.Interface.AgentObjects;
using CommonModelTypes.Interface.SimObjects;
using ConfigurationAdapter.Interface;
using ForestModel.Interface.Configuration;

namespace ForestModel.Implementation.Agents
{
    public class TreeAgent : AbstractAgent
    {
        #region fields
        private float _diameter;
        private float _height;
        private float _fitness;
        private float _growthCoefficient;
        private float _maxDiameter;
        private float _maxHeight;
        private int _age;
        private TreeAgentConfiguration _config;
        #endregion

        #region constructors
        public TreeAgent(int id,  Vector3D position, float diameter, float height) : base(id, position)
        {
            _config = Configuration.Load<TreeAgentConfiguration>();

            _diameter = diameter;
            _height = height;
            _fitness = 100;
            _growthCoefficient = _config.GrowthCoefficient;
            _maxDiameter = _config.MaxDiameter;
            _maxHeight = _config.MaxHeight;

        }
        #endregion

        #region public methods



        public override void Tick()
        {
            tickDiameter();
            tickHeight();
        }


        /// <summary>
        /// Take a swing from another agent. Returns the amount of wood that was chucked.
        /// </summary>
        /// <param name="damage">dmg dealt to the tree</param>
        /// <returns></returns>
        public int chuck(int damage)
        {
            //TODO berechne holzwert aus größe und durchmesser.
            _fitness = _fitness - damage;
            return (_fitness > 0) ? 0 : 700;
        }

        public void SeedDispearsal()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region private methods
        private void tickDiameter()
        {
            _diameter = _diameter + _growthCoefficient * (_maxDiameter - _diameter);
        }

        private void tickHeight()
        {
            _height = _height + _growthCoefficient * (_maxHeight - _height);
        }
        #endregion
        

    }
}
