namespace WorkingMemory.Interfaces
{
    public enum MemoryFactType {
        Visual, 
        Pain,
    }

    interface IFact {

        MemoryFactType GetType();

        bool SetValue();
    }
}
