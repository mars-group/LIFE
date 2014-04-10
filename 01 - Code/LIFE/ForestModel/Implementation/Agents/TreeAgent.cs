﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModelTypes.Interface;
using ConfigurationAdapter.Interface;
using ForestModel.Interface.Configuration;

namespace ForestModel.Implementation.Agents
{
    class TreeAgent : AbstractAgent
    {
        #region fields
        private float _diameter;
        private float _height;
        private float _fitness;
        private float _growthCoefficient;
        private float _maxDiameter;
        private float _maxHeight;
        private int _age;
        private Configuration<TreeAgentConfiguration> _config;
        #endregion

        #region constructors
        public TreeAgent(int id,  Vector3D position, float diameter, float height) : base(id, position)
        {
            _config = new Configuration<TreeAgentConfiguration>(new TreeAgentConfiguration());

            _diameter = diameter;
            _height = height;
            _fitness = 100;
            _growthCoefficient = _config.Content.GrowthCoefficient;
            _maxDiameter = _config.Content.MaxDiameter;
            _maxHeight = _config.Content.MaxHeight;

        }
        #endregion

        #region public methods



        public override void tick()
        {
            tickDiameter();
            tickHeight();
        }

        public void chuck(int damage)
        {
            _fitness = _fitness - damage;
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

        private void AmIDead() {
            throw new NotImplementedException();
        }

    }
}
