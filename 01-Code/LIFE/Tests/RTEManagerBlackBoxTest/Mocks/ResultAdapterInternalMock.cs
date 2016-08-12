//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;
using LifeAPI.Results;
using ResultAdapter.Interface;

namespace RTEManagerBlackBoxTest.Mocks {
  class ResultAdapterInternalMock : IResultAdapter {
    
    public Guid SimulationId {
      get { return Guid.NewGuid(); }
      set { }
    }

    public void WriteResults(int currentTick) {

    }

      public void Register(ISimResult simObject, int executionGroup = 1)
      {

      }

      public void DeRegister(ISimResult simObject, int executionGroup = 1)
      {

      }

  }
}