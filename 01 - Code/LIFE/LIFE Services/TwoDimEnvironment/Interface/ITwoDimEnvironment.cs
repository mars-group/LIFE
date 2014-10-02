using System;
using System.Windows;
using System.Collections.Generic;
using CSharpQuadTree;

namespace TwoDimEnvironment
{
	public interface ITwoDimEnvironment<T> where T : class, IQuadObject
	{
		/// <summary>
		/// Add the specified item into the Environment
		/// </summary>
		/// <param name="item">the item to add.</param>
		void Add(T item);

		/// <summary>
		/// Move the specified item to X,Y.
		/// The items' center point is set to X,Y
		/// </summary>
		/// <param name="item">The item to move</param>
		/// <param name="X">The target position's X coordinate</param>
		/// <param name="Y">The target position's Y coordinate</param>
		/// <returns>The updated item with ne Bounds and Position</returns>
		Rect Move(T item, double X, double Y);

		/// <summary>
		/// Update the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		void Update(T item);

		/// <summary>
		/// Find objects which are covered by the specified area.
		/// </summary>
		/// <param name="area">Area.</param>
		/// <returns>>A list of found objects, an empty list if no objects where found</returns>
		List<T> Find(Rect area);

		/// <summary>
		/// Find objects which are covered by the area spanned by
		/// the specified centerItem and distance.
		/// </summary>
		/// <param name="centerItem">The item whose center point is taken as the middle of a circle</param>
		/// <param name="distance">Distance to look at</param>
		/// <returns>>A list of found objects, an empty list if no objects where found</returns>
		List<T> Find(T centerItem, int distance);

		/// <summary>
		/// Gets all items in the environment
		/// </summary>
		/// <returns>A list with all object or an empty list if there are none.</returns>
		List<T> GetAll();
	}
}

