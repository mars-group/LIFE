using System.Threading;
using DotSpatial.Projections;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //double lat = 53.550514;
            //double lon = 9.992439;

            //Point latlon = new Point(lon, lat);
            //Console.WriteLine("Lon,Lat: " + latlon.ToString());

            //for (int zoom = 21; zoom >= 0; zoom--)
            //{
            //    string quad = GlobalMercator.LatLonToQuadTree(latlon, zoom);
            //    Console.WriteLine("Z:{0}\tQ:{1}", quad.Length, quad, GlobalMercator.QuadTreeToLatLon(quad).ToString());
            //}

            string polgyonString = string.Empty;
            using (TextReader txtReader = new StreamReader(@"coordsBerlin.txt"))
            {
                polgyonString = txtReader.ReadToEnd();
            }

            var matchCollection = Regex.Matches(polgyonString, @"\[([0-9]+\.[0-9]+),([0-9]+\.[0-9]+)\]");

            double[] z = new double[1];
            z[0] = 1;

            ProjectionInfo pStart = KnownCoordinateSystems.Projected.World.Mercatorworld;
            ProjectionInfo pEnd = KnownCoordinateSystems.Geographic.World.WGS1984;

            List<Coordinate> coords = new List<Coordinate>();
            foreach (Match match in matchCollection)
            {
                if (match.Success)
                {
                    //Point point = GlobalMercator.MetersToLatLon(new Point(Convert.ToDouble(match.Groups[1].Value.Replace('.', ',')), Convert.ToDouble(match.Groups[2].Value.Replace('.', ','))));
                    //polygons.Add(point);
                    double[] xy = new double[2];
                    xy[0] = Convert.ToDouble(match.Groups[1].Value.Replace('.', ','));
                    xy[1] = Convert.ToDouble(match.Groups[2].Value.Replace('.', ','));
                    Reproject.ReprojectPoints(xy, z, pStart, pEnd, 0, 1);
                    coords.Add(new Coordinate() { X = xy[0], Y = xy[1] });
                }
            }
            if (coords.Count > 0)
            {
                coords.Add(coords.First());
            }

            var linearRing = new LinearRing(coords.ToArray());
            Polygon pg = new Polygon(linearRing);

            var envelopeCoords = pg.Envelope.Coordinates;
            mars.rock.common.GIS.Rect envelopeRect = new mars.rock.common.GIS.Rect(new mars.rock.common.GIS.Point(envelopeCoords[0].X, envelopeCoords[0].Y), new mars.rock.common.GIS.Point(envelopeCoords[2].X, envelopeCoords[2].Y));

            var sw = System.Diagnostics.Stopwatch.StartNew();
            var maxZoomLevel = 17;
            var quadsWthinPolygon = mars.rock.common.GIS.MSQuadTile.FindQuads(envelopeRect, pg, maxZoomLevel, 1);
            Console.WriteLine("Took {0}ms to calculate {1} quads with max. zoom level of {2}.", sw.ElapsedMilliseconds, quadsWthinPolygon.Count, maxZoomLevel);

            SortedDictionary<int, int> quadsInLevel = new SortedDictionary<int, int>();
            foreach (var quad in quadsWthinPolygon)
            {
                if (!quadsInLevel.ContainsKey(quad.ZoomLevel))
                {
                    quadsInLevel.Add(quad.ZoomLevel, 1);
                }
                quadsInLevel[quad.ZoomLevel]++;
            }

            foreach (var level in quadsInLevel)
            {
                Console.WriteLine("Level {0}: {1} quads.", level.Key, level.Value);
            }

            Console.ReadLine();
        }

        //private static List<string> FittingQuads(Polygon pg, mars.rock.common.GIS.Rect envelopeRect, int zoomLvl)
        //{
        //    List<string> fittingQuads = new List<string>();
        //    FittingQuad(pg, fittingQuads, envelopeRect, zoomLvl);
        //    return fittingQuads;
        //}

        //private static void FittingQuad(Polygon pg, List<string> fittingQuads, mars.rock.common.GIS.Rect envelopeRect, int zoomLvl)
        //{
        //    bool oneDidNotFit = false;

        //    var quadsInEnvelope = mars.rock.common.GIS.GlobalMercator.GetQuadTreeList(zoomLvl, envelopeRect.TopLeft, envelopeRect.BottomRight);
        //    foreach (var quad in quadsInEnvelope)
        //    {
        //        var quadBounds = mars.rock.common.GIS.GlobalMercator.QuadTreeToLatLon(quad);
        //        List<Coordinate> quadBoundsCoords = new List<Coordinate>();
        //        quadBoundsCoords.Add(new Coordinate(quadBounds.Left, quadBounds.Top));
        //        quadBoundsCoords.Add(new Coordinate(quadBounds.Right, quadBounds.Top));
        //        quadBoundsCoords.Add(new Coordinate(quadBounds.Right, quadBounds.Bottom));
        //        quadBoundsCoords.Add(new Coordinate(quadBounds.Left, quadBounds.Top));

        //        var quadBoundsLinearRing = new LinearRing(quadBoundsCoords.ToArray());
        //        Polygon quadBoundsPolygon = new Polygon(quadBoundsLinearRing);

        //        if (pg.Contains(quadBoundsPolygon))
        //        {
        //            fittingQuads.Add(quad);
        //        }
        //        else
        //        {
        //            oneDidNotFit = true;
        //        }
        //    }
        //}
    }
}
