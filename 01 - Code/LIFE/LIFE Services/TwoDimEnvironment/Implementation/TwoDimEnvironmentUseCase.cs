using System;
using CSharpQuadTree;
using System.Windows;
using System.Collections.Generic;

namespace TwoDimEnvironment
{
	public class TwoDimEnvironmentUseCase<T> : ITwoDimEnvironment<T> where T : class, IQuadObject
	{

		private QuadTree<T> _quadTree;

		public TwoDimEnvironmentUseCase ()
		{
			_quadTree = new QuadTree<T> (new Size (25, 25), 1);
		}

		public TwoDimEnvironmentUseCase ()
		{
			_quadTree = new QuadTree<T> (new Size (25, 25), 1);
		}

		#region ITwoDimEnvironment implementation

		public void Add (T item)
		{
			_quadTree.Insert (item);
		}

		public T Move (T item, int X, int Y)
		{
			_quadTree.Remove (item);

			var oldBounds = item.Bounds;

			item.Bounds = new Rect (
				X - oldBounds.Width / 2,
				Y + oldBounds.Height / 2,
				oldBounds.Width,
				oldBounds.Height
			);

			_quadTree.Insert (item);
			return item;
		}

		public List<T> Find (Rect area)
		{
			return _quadTree.Query(area);
		}

		public List<T> Find (T centerItem, int distance)
		{
			return _quadTree.Query(
				new Rect(
					centerItem.Bounds.X,
					centerItem.Bounds.Y,
					centerItem.Bounds.Width + distance,
					centerItem.Bounds.Height + distance
				)
			);
		}


		public void Update (T item)
		{
			_quadTree.Remove (item);
			_quadTree.Insert (item);
		}

		public List<T> GetAll ()
		{
			return _quadTree.Query(_quadTree.Root.Bounds);
		}
		#endregion
	}
}

