using System;

namespace PedestrianModel.Util.Math {
    internal class Distribution {

        public static double NormalGaussian(double meanValue, double standardDeviation) {
            Random random = new Random();
            double u1 = random.NextDouble();
            double u2 = random.NextDouble();
            double randomStandardNormal = System.Math.Sqrt(-2.0*System.Math.Log(u1))
                                          *System.Math.Sin(2.0*System.Math.PI*u2);
            double randomNormal = meanValue + standardDeviation*randomStandardNormal;
            return randomNormal;
        }

    }
}