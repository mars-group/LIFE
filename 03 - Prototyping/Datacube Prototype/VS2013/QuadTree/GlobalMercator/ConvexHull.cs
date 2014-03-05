using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GlobalMercatorLib
{
    public class ConvexHull : ArrayList
    {
        private int status;
        private int start, stop; //tangents for iterative convex hull
        private int xmin, xmax, ymin, ymax; //position of hull
        private int yxmax; //y coord of xmax
        private GeomPoint rectp;
        private int recth, rectw;
        private bool changed;

        /* largest rectangles with corners on AC, BD, ABC, ABD, ACD, BCD */
        private ArrayList RectList;

        /* fixed aspect ratio */
        private bool isFixed;
        private int fixedX, fixedY;

        public ConvexHull()
        {
            this.isFixed = false;
            this.fixedX = 1;
            this.fixedY = 1;
            this.fixedX = 1;
            this.fixedY = 1;
            RectList = new ArrayList();
        }

        /* position of point w.r.t. hull edge
     * sign of twice the area of triangle abc
     */

        private bool onLeft(GeomPoint a, GeomPoint b, GeomPoint c)
        {
            int area = (b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y);
            return (area < 0);
        }

        /* check if point is outside
     * true is point is on right of all vertices
     * finds tangents if point is outside
     */

        private bool pointOutside(GeomPoint p)
        {
            //, int start, int stop){

            bool ptIn = true, currIn, prevIn = true;

            GeomPoint a = (GeomPoint)this[0];
            GeomPoint b;

            for (int i = 0; i < this.Count; i++)
            {

                b = (GeomPoint)this[(i + 1) % this.Count];
                currIn = onLeft(a, b, p);
                ptIn = ptIn && currIn;
                a = b;

                if (prevIn && !currIn)
                {
                    start = i;
                } /* next point outside, 1st tangent found */
                if (!prevIn && currIn)
                {
                    stop = i;
                } /* 2nd tangent */
                prevIn = currIn;

            }
            return !ptIn;
        }

        /* check if point is outside, insert it, maintaining general position */

        private bool addPointToHull(GeomPoint p)
        {

            /* index of tangents */
            start = 0;
            stop = 0;

            if (!pointOutside(p))
            {
                return false;
            }

            /* insert point */
            int numRemove;

            if (stop > start)
            {
                numRemove = stop - start - 1;
                if (numRemove > 0)
                {
                    this.RemoveRange(start + 1, stop);
                }
                this.Insert(start + 1, p); //insertElmentAt(p, start+1);
            }
            else
            {
                numRemove = stop + this.Count - start - 1;
                if (numRemove > 0)
                {
                    if (start + 1 < this.Count)
                    {
                        this.RemoveRange(start + 1, this.Count);
                    }
                    if (stop - 1 >= 0)
                    {
                        this.RemoveRange(0, stop);
                    }
                }
                this.Add(p);

            }
            Console.WriteLine("changing");
            this.changed = true;
            return true;
        } //addPointToHull

        /* compute edge list
     * set xmin, xmax
     * used to find largest rectangle by scanning horizontally
     */

        private ArrayList computeEdgeList()
        {
            ArrayList l = new ArrayList();
            GeomPoint a, b;
            GeomEdge e;
            a = (GeomPoint)this[this.Count - 1];
            for (int i = 0; i < this.Count; i++)
            {
                b = (GeomPoint)this[i];
                //b = (GeomPoint)this.IndexOf(i+1);

                if (i == 0)
                {
                    this.xmin = a.x;
                    this.xmax = a.x;
                    this.ymin = a.y;
                    this.ymax = a.y;
                }
                else
                {
                    if (a.x < this.xmin)
                    {
                        this.xmin = a.x;
                    }
                    if (a.x > this.xmax)
                    {
                        this.xmax = a.x;
                        this.yxmax = a.y;
                    }
                    if (a.y < this.ymin)
                    {
                        this.ymin = a.y;
                    }
                    if (a.y > this.ymax)
                    {
                        this.ymax = a.y;
                    }
                }
                e = new GeomEdge(a, b);
                l.Add(e);
                a = b;
            } //for
            // b = (GeomPoint)this.IndexOf(this.Count-1);
            // a = (GeomPoint)this.IndexOf(0);
            // e = new GeomEdge(b,a);
            // l.Add(e);
            return l;
        }

        /* compute y intersection with an edge
     * first pixel completely inside
     * ceil function if edge is on top, floor otherwise
     * (+y is down)
     */

        private int yIntersect(int xi, GeomEdge e)
        {

            int y;
            double yfirst = (e.m) * (xi - 0.5) + e.b;
            double ylast = (e.m) * (xi + 0.5) + e.b;

            if (!e.isTop)
            {
                y = (int)Math.Floor(Math.Min(yfirst, ylast));
            }
            else
            {
                y = (int)Math.Ceiling(Math.Max(yfirst, ylast));
            }
            return y;
        }

        /* find largest pixel completely inside
     * look through all edges for intersection
     */

        private int xIntersect(int y, ArrayList l)
        {
            int x = 0;
            double x0 = 0, x1 = 0;
            for (int i = 0; i < this.Count; i++)
            {
                GeomEdge e = (GeomEdge)l[i];
                if (e.isRight && e.ymin <= y && e.ymax >= y)
                {
                    x0 = (double)(y + 0.5 - e.b) / e.m;
                    x1 = (double)(y - 0.5 - e.b) / e.m;
                }
            }
            x = (int)Math.Floor(Math.Min(x0, x1));
            //System.out.println("xIntersect, x is " + x);
            return x;
        }

        private GeomEdge findEdge(int x, bool isTop, ArrayList l)
        {
            GeomEdge e, emax = (GeomEdge) l[0];
            //int count = 0;
            for (int i = 0; i < this.Count; i++)
            {
                e = (GeomEdge)l[i];
                if (e.xmin == x)
                {
                    //count++;
                    //if (count == 1){
                    //    emax = e;
                    //}
                    //else{
                    if (e.xmax != e.xmin)
                    {
                        if ((e.isTop && isTop) || (!e.isTop && !isTop))
                        {
                            emax = e;
                        }
                    }
                }

            }
            return emax;
        }

        /* compute 3 top and bottom 3 corner rectangle for each xi
     * find largest 2 corner rectangle
     */

        private int computeLargestRectangle()
        {

            this.changed = false;
            ArrayList edgeList = computeEdgeList();
            this.RectList = new ArrayList();

            GeomEdge top, bottom;
            int ymax, ymin, xright, xlo, xhi;
            int area, maxArea = 0;
            int maxAreaAC = 0, maxAreaBD = 0, maxAreaABC = 0, maxAreaABD = 0, maxAreaACD = 0, maxAreaBCD = 0;
            int width, height, maxh = 0, maxw = 0;

            /* all 2-corner and 3-corner largest rectangles */
            int aAC = 0, aBD = 0, aABC = 0, aABD = 0, aACD = 0, aBCD = 0;
            GeomPoint pAC, pBD, pABC, pABD, pACD, pBCD;
            int hAC = 0,
                wAC = 0,
                hBD = 0,
                wBD = 0,
                hABC = 0,
                wABC = 0,
                hABD = 0,
                wABD = 0,
                hACD = 0,
                wACD = 0,
                hBCD = 0,
                wBCD = 0;
            bool onA, onB, onC, onD;

            GeomPoint maxp = new GeomPoint(0, 0);
            pAC = maxp;
            pBD = maxp;
            pABC = maxp;
            pABD = maxp;
            pACD = maxp;
            pBCD = maxp;

            ArrayList xint = new ArrayList();

            for (int i = 0; i < this.ymax; i++)
            {
                int x = xIntersect(i, edgeList);
                GeomPoint px = new GeomPoint(x, i);
                xint.Add(px);
            }
            //find first top and bottom edges
            top = findEdge(this.xmin, true, edgeList);
            bottom = findEdge(this.xmin, false, edgeList);

            //scan for rectangle left position
            for (int xi = this.xmin; xi < this.xmax; xi++)
            {

                ymin = yIntersect(xi, top);
                ymax = yIntersect(xi, bottom);

                for (int ylo = ymax; ylo >= ymin; ylo--)
                {
                    //ylo from to to bottom

                    for (int yhi = ymin; yhi <= ymax; yhi++)
                    {

                        if (yhi > ylo)
                        {

                            onA = (yhi == ymax && !bottom.isRight);
                            onD = (ylo == ymin && !top.isRight);

                            xlo = (int)((GeomPoint)xint[ylo]).x; //xIntersect(ylo,edgeList);
                            xhi = (int)((GeomPoint)xint[yhi]).x; //xIntersect(yhi,edgeList);

                            xright = maxp.min(xlo, xhi);
                            onC = (xright == xlo && this.yxmax >= ylo);
                            onB = (xright == xhi && this.yxmax <= yhi);

                            height = yhi - ylo;
                            width = xright - xi;

                            if (!this.
                            isFixed)
                            {
                            } //!fixed
                            else
                            {
                                int fixedWidth =
                                    (int)Math.Ceiling(((double)height * this.fixedX) / ((double)this.fixedY));
                                if (fixedWidth <= width)
                                {
                                    width = fixedWidth;
                                }
                                else
                                {
                                    width = 0;
                                }
                            }
                            area = width * height;
                            //AC 
                            if (onA && onC && !onB && !onD)
                            {
                                if (area > aAC)
                                {
                                    aAC = area;
                                    pAC = new GeomPoint(xi, ylo);
                                    hAC = height;
                                    wAC = width;
                                }
                            }
                            //BD
                            if (onB && onD && !onA && !onC)
                            {
                                if (area > aBD)
                                {
                                    aBD = area;
                                    pBD = new GeomPoint(xi, ylo);
                                    hBD = height;
                                    wBD = width;
                                }
                            }
                            //ABC
                            if (onA && onB && onC)
                            {
                                if (area > aABC)
                                {
                                    aABC = area;
                                    pABC = new GeomPoint(xi, ylo);
                                    hABC = height;
                                    wABC = width;
                                }
                            }
                            //ABD
                            if (onA && onB && onD)
                            {
                                if (area > aABD)
                                {
                                    aABD = area;
                                    pABD = new GeomPoint(xi, ylo);
                                    hABD = height;
                                    wABD = width;
                                }
                            }
                            //ACD
                            if (onA && onC && onD)
                            {
                                if (area > aACD)
                                {
                                    aACD = area;
                                    pACD = new GeomPoint(xi, ylo);
                                    hACD = height;
                                    wACD = width;
                                }
                            }
                            //BCD
                            if (onB && onC && onD)
                            {
                                if (area > aBCD)
                                {
                                    aBCD = area;
                                    pBCD = new GeomPoint(xi, ylo);
                                    hBCD = height;
                                    wBCD = width;
                                }
                            }

                            if (area > maxArea)
                            {
                                maxArea = area;
                                maxp = new GeomPoint(xi, ylo);
                                maxw = width;
                                maxh = height;
                                // System.out.println(onA + " " + onB + " " + onC + " " + onD);
                            }
                        } //yhi > ylo
                    } //for yhi
                } //for ylo
                if (xi == top.xmax)
                {
                    top = findEdge(xi, true, edgeList);
                }
                if (xi == bottom.xmax)
                {
                    bottom = findEdge(xi, false, edgeList);
                }
            } //xi
            this.rectp = maxp;
            this.recth = maxh;
            this.rectw = maxw;

            this.RectList.Add(new Rectangle(pAC.x, pAC.y, wAC, hAC));
            this.RectList.Add(new Rectangle(pBD.x, pBD.y, wBD, hBD));
            this.RectList.Add(new Rectangle(pABC.x, pABC.y, wABC, hABC));
            this.RectList.Add(new Rectangle(pABD.x, pABD.y, wABD, hABD));
            this.RectList.Add(new Rectangle(pACD.x, pACD.y, wACD, hACD));
            this.RectList.Add(new Rectangle(pBCD.x, pBCD.y, wBCD, hBCD));
            this.RectList.Add(new Rectangle(maxp.x, maxp.y, maxw, maxh));
            return 0;

        }
    }

    public class GeomPoint
    {
        public int x { get; set; }
        public int y { get; set; }

        public GeomPoint(int ptx, int pty)
        {
            this.x = ptx;
            this.y = pty;
        }

        public int min(int a, int b)
        {
            if (a <= b) return a;
            else return b;
        }

        public int max(int a, int b)
        {
            if (a >= b) return a;
            else return b;
        }
    }


    public class GeomEdge
    {

        public int xmin, xmax; /* horiz, +x is right */
        public int ymin, ymax; /* vertical, +y is down */
        public double m, b; /* y = mx + b */
        public bool isTop, isRight; /* position of edge w.r.t. hull */

        public GeomEdge(GeomPoint p, GeomPoint q)
        {
            this.xmin = p.min(p.x, q.x);
            this.xmax = p.max(p.x, q.x);
            this.ymin = p.min(p.y, q.y);
            this.ymax = p.max(p.y, q.y);
            this.m = ((double)(q.y - p.y)) / ((double)(q.x - p.x));
            this.b = p.y - m * (p.x);
            this.isTop = p.x > q.x; //edge from right to left (ccw)
            this.isRight = p.y > q.y; //edge from bottom to top (ccw)
        }
    }
}
