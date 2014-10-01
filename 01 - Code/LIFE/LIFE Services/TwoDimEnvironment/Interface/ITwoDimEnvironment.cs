using System;
using System.Windows;
using System.Collections.Generic;

namespace TwoDimEnvironment
{
	public interface ITwoDimEnvironment<T> where T : class, ISimObject2D
	{
		/// <summary>
		/// Add the specified item into the Environment
		/// </summary>
		/// <param name="item">the item to add.</param>
		void Add(T item);

		/// <summary>
		/// Move the specified item to targetPosition.
		/// The items' center point is set to targetPosition
		/// </summary>
		/// <param name="item">The item to move</param>
		/// <param name="targetPosition">The target position to move to</param>
		void Move(T item, Position targetPosition);

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
	}
}

