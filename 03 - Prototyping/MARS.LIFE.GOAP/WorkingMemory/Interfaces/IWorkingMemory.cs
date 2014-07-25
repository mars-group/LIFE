namespace WorkingMemory.Interfaces
{
    interface IWorkingMemory {

        bool AddFact(IFact fact);




        bool HasFact(MemoryFactType factType);

        IFact GetFact(MemoryFactType factType);
    }
}
