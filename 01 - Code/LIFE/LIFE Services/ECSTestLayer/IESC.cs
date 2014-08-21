using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCTestLayer
{
    interface IESC
    {
        /// <summary>
        ///   registers the element with give dimension
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="dimension"></param>
        void Add(int elementId, Vector3f dimension);

        /// <summary>
        /// unregisters the element if existent
        /// </summary>
        /// <param name="elementId"></param>
        void Remove(int elementId);

        /// <summary> 
        /// updates the dimension of given element. if not existent it will be added.
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="dimension"></param>
        /// <returns>indicates whether or not the operation succeded</returns>
        Boolean Update(int elementId, Vector3f dimension);

        /// <summary>
        /// tries to set the element to given position
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <returns>indicates whether the operation succeded or a collision occured</returns>
        Boolean SetPosition(int elementId, Vector3f position, Vector3f direction);

        /// <summary>
        /// tries to set the element randomly in given area that is describes by it's corners min and max
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="min">Minimum value (xMin, yMin, zMin). May be set to 'null' for a positive-only system.</param>
        /// <param name="max">Maximum value (xMax, yMax, zMax). This position is excluded.</param>
        /// <param name="grid">Tells, whether only integer ('true') or decimal ('false') values shall be generated.</param>
        /// <returns>indicates whether the operation succeded or no random position could be found</returns>
        Boolean SetRandomPosition(int elementId, Vector3f min, Vector3f max, bool grid);

        /// <summary>
        /// get the distance between the both elemtents. takes the dimesion into consideration.
        /// </summary>
        /// <param name="anElementId"></param>
        /// <param name="anotherElementId"></param>
        /// <returns></returns>
        float GetDistance(int anElementId, int anotherElementId);

        /// <summary>
        /// finds a list of elements that would collide with given element if set at given position with given direction
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        IEnumerable<CollidabelElement> Explore(int elementId, Vector3f position, Vector3f direction);
    }
}
