// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using System.Collections.Generic;
using System.Windows;
using CSharpQuadTree;

namespace TwoDimEnvironment {
    public class TwoDimEnvironmentUseCase<T> : ITwoDimEnvironment<T> where T : class, IQuadObject {
        private readonly QuadTree<T> _quadTree;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TwoDimEnvironment.TwoDimEnvironmentUseCase`1" /> class.
        ///     This implementation will guarantuee the order of objects. Every query will return the objects in the same
        ///     order you inserted them.
        /// </summary>
        public TwoDimEnvironmentUseCase() {
            _quadTree = new QuadTree<T>(new Size(25, 25), 1, true);
        }

        #region ITwoDimEnvironment implementation

        public void Add(T item) {
            _quadTree.Insert(item);
        }

        public Rect Move(T item, double X, double Y) {
            _quadTree.Remove(item);

            Rect oldBounds = item.Bounds;

            item.Bounds = new Rect
                (
                X - oldBounds.Width/2,
                Y + oldBounds.Height/2,
                oldBounds.Width,
                oldBounds.Height
                );

            _quadTree.Insert(item);
            return item.Bounds;
        }

        public List<T> Find(Rect area) {
            return _quadTree.Query(area);
        }

        public List<T> Find(T centerItem, int distance) {
            return _quadTree.Query
                (
                    new Rect
                        (
                        centerItem.Bounds.X,
                        centerItem.Bounds.Y,
                        centerItem.Bounds.Width + distance,
                        centerItem.Bounds.Height + distance
                        )
                );
        }


        public void Update(T item) {
            _quadTree.Remove(item);
            _quadTree.Insert(item);
        }

        public List<T> GetAll() {
            return _quadTree.Query(_quadTree.Root.Bounds);
        }

        public Rect GetBounds() {
            return _quadTree.Root.Bounds;
        }

        #endregion
    }
}