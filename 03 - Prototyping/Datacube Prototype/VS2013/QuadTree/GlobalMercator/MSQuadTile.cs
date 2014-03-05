using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalMercatorLib
{
    public class MSQuadTile
    {
        public static int MAX_ZOOM_LEVEL = 21;

        public string QuadID { get; private set; }

        public int ZoomLevel
        {
            get
            {
                return QuadID.Length;
            }
        }

        public Rect LatLonBounds
        {
            get
            {
                return GlobalMercator.QuadTreeToLatLon(this.QuadID);
            }
        }

        public Point LatLonTopLeft
        {
            get
            {
                return GlobalMercator.QuadTreeToLatLon(this.QuadID).TopLeft;
            }
        }

        public Point LatLonBottomRight
        {
            get
            {
                return GlobalMercator.QuadTreeToLatLon(this.QuadID).BottomRight;
            }
        }

        public MSQuadTile Parent
        {
            get
            {
                if (ZoomLevel > 1)
                {
                    return new MSQuadTile(QuadID.Substring(0, ZoomLevel - 1));
                }
                else
                {
                    return null;
                }
            }
        }

        public List<MSQuadTile> Children
        {
            get
            {
                if (ZoomLevel < MAX_ZOOM_LEVEL)
                {
                    return QuadListFromIds(GlobalMercator.GetQuadTreeList(ZoomLevel + 1, this.LatLonTopLeft, this.LatLonBottomRight));
                }
                else
                {
                    return null;
                }
            }
        }

        public bool HasChildren
        {
            get
            {
                if (ZoomLevel < MAX_ZOOM_LEVEL)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public MSQuadTile(string quadId)
        {
            this.QuadID = quadId;
        }

        public static List<MSQuadTile> QuadListFromIds(List<string> quadIds)
        {
            return QuadListFromIds(quadIds.ToArray());
        }

        public static List<MSQuadTile> QuadListFromIds(string[] quadIds)
        {
            var result = new List<MSQuadTile>(quadIds.Length);
            result.AddRange(quadIds.Select(quadId => new MSQuadTile(quadId)));
            return result;
        }

        public static List<MSQuadTile> FindQuads(MSQuadTile quad, IGeometry geometricObject, int maxZoomLvl, double minPercCovered)
        {
            return new List<MSQuadTile>(FindQuadsConcurrent(quad, geometricObject, maxZoomLvl, minPercCovered));
        }

        private static ConcurrentBag<MSQuadTile> FindQuadsConcurrent(MSQuadTile quad, IGeometry geometricObject, int maxZoomLvl, double minPercCovered)
        {
            var result = new ConcurrentBag<MSQuadTile>();

            // Quadausmaße
            var quadBounds = quad.LatLonBounds;

            // Sichtbarkeit
            var visibility = DetermineVisibility(geometricObject, quadBounds, minPercCovered);

            switch (visibility)
            {
                case MSQuadTileVisibility.OUTSIDE:
                    return null;
                case MSQuadTileVisibility.INSIDE:
                case MSQuadTileVisibility.INTERSECT:
                    if (quad.ZoomLevel >= maxZoomLvl || !quad.HasChildren) break;
                    Parallel.ForEach(quad.Children, childQuad =>
                    {
                        var foundQuads = FindQuadsConcurrent(childQuad, geometricObject, maxZoomLvl, minPercCovered);
                        if (foundQuads == null || foundQuads.Count <= 0) return;
                        foreach (var foundQuad in foundQuads)
                        {
                            result.Add(foundQuad);
                        }
                    });
                    break;
                case MSQuadTileVisibility.WITHIN:
                    result.Add(quad);
                    break;
            }

            return result;
        }

        private static MSQuadTileVisibility DetermineVisibility(IGeometry geometricObject, Rect quadBounds, double minPercCovered)
        {
            var quadBoundsCoords = new List<Coordinate>
            {
                new Coordinate(quadBounds.Left, quadBounds.Top),
                new Coordinate(quadBounds.Right, quadBounds.Top),
                new Coordinate(quadBounds.Right, quadBounds.Bottom),
                new Coordinate(quadBounds.Left, quadBounds.Top)
            };
            var quadBoundsLinearRing = new LinearRing(quadBoundsCoords.ToArray());
            var quadEnvelope = quadBoundsLinearRing.Envelope;

            if (geometricObject.Contains(quadEnvelope))
            {
                return MSQuadTileVisibility.WITHIN;
            }

            if (quadEnvelope.Contains(geometricObject))
            {
                return MSQuadTileVisibility.INSIDE;
            }

            if (!quadEnvelope.Intersects(geometricObject)) return MSQuadTileVisibility.OUTSIDE;
            var percentageCovered = quadEnvelope.Difference(geometricObject).Area / quadEnvelope.Area;
            return percentageCovered > minPercCovered ? MSQuadTileVisibility.WITHIN : MSQuadTileVisibility.INTERSECT;
        }

        private enum MSQuadTileVisibility { OUTSIDE, INSIDE, INTERSECT, WITHIN };
    }
}
