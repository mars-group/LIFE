using System.Collections.Generic;
using CommonTypes.DataTypes;
using ESCTestLayer.Entities;
using LayerAPI.Interfaces;

namespace ESCTestLayer.Interface
{
    using CommonTypes.TransportTypes;

    public interface IDeprecatedESC : IGenericDataSource {
        /// <summary>
        ///   Registers the element with given dimension.
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="informationType"></param>
        /// <param name="collidable"></param>
        /// <param name="dimension"></param>
        void Add(int elementId, int informationType, bool collidable, TVector dimension);

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
        /// <returns>the result holds the current position and possibly additional information; null if update did not succeed</returns>
        MovementResult Update(int elementId, TVector dimension);

        /// <summary>
        /// tries to set the element to given position
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <returns>the result holds the current position as well as additional information like collision</returns>
        MovementResult SetPosition(int elementId, TVector position, TVector direction);

        /// <summary>
        /// tries to set the element randomly in given area that is describes by it's corners min and max
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="min">Minimum value (xMin, yMin, zMin). May be set to 'null' for a positive-only system.</param>
        /// <param name="max">Maximum value (xMax, yMax, zMax). This position is excluded.</param>
        /// <param name="grid">Tells, whether only integer ('true') or decimal ('false') values shall be generated.</param>
        /// <returns>the result holds the current position and possibly additional information</returns>
        MovementResult SetRandomPosition(int elementId, TVector min, TVector max, bool grid);

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
        IEnumerable<CollidableElement> Explore(int elementId, TVector position, TVector direction);
    }
}
