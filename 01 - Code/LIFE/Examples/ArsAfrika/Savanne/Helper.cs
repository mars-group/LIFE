using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Savanne {

    
    public static class HelperUtil {

        /// <summary>
        /// Get the data from a coordinates txt file. The first line may include metadata and if it contains string  with "DD" it will be ignored.
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        public static List<Tuple<double, double>> ReadKoordinatesFromFile(string fileLocation) {
            List<Tuple<double, double>> coordinates = new List<Tuple<double, double>>();
            
            foreach ( string readLine in File.ReadLines(fileLocation)) {

                if (readLine.Contains("DD")) {
                    //first line contains csv column informations
                    continue;
                }

                string[] coordArray = readLine.Split(";".ToCharArray());
                if (coordArray.Count() != 2) {
                    throw new ArgumentException("Error during file parsing. Unexpected number of coordinates");
                }

                double x = Double.Parse(coordArray[0], CultureInfo.InvariantCulture);
                double y = Double.Parse(coordArray[1], CultureInfo.InvariantCulture);
                //The file contains x and y switched
                coordinates.Add(new Tuple<double, double>(y,x));
            }
            return coordinates;
        }
    }
}