//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
namespace SimulationManagerTest.ModelContainerTestClasses
{
    /* These are test classes that only exist to check the dependency injection. See the TestSimulationManagerPositive Test. */

    class Zero
    {
    }

    class One
    {
    }

    class Two
    {
    }

    class Three
    {
        public Three(One one, Two two, Three three) {}
    }

    class Four
    {
        public Four(Three three) {}
    }

    class Five
    {
        public Five(Three three, Six six) {}
    }

    class Six
    {
        public Six(Four four) {}
    }

    class Seven
    {
        public Seven(Seven seven) {}
    }
}
