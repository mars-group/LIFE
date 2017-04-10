//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

// ReSharper disable UnusedParameter.Local

namespace ModelContainerTests
{
    /* These are test classes that only exist to check the dependency 
     * injection. See the TestSimulationManagerPositive Test. */

    internal class Zero
    {
    }

    internal class One
    {
    }

    internal class Two
    {
    }

    internal class Three
    {
        public Three(One one, Two two, Three three)
        {
        }
    }

    internal class Four
    {
        public Four(Three three)
        {
        }
    }

    internal class Five
    {
        public Five(Three three, Six six)
        {
        }
    }

    internal class Six
    {
        public Six(Four four)
        {
        }
    }

    internal class Seven
    {
        public Seven(Seven seven)
        {
        }
    }
}