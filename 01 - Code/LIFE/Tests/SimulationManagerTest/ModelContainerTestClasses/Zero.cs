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
