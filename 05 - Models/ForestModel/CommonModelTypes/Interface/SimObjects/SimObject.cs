using System.Drawing;
using QuadTreeLib;

namespace CommonModelTypes.Interface.SimObjects
{
    public abstract class SimObject
    {
        protected SimObject(int id)
        {
            ID = id;
        }

        public int ID { get; private set; }
  



    }
}
