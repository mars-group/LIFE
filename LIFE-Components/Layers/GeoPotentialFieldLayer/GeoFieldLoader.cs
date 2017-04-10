using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using LIFE.Components.GridPotentialFieldLayer;

namespace LIFE.Components.GeoPotentialFieldLayer
{
    public class GeoFieldLoader : IFieldLoader<GeoPotentialField>
    {
        public GeoPotentialField LoadPotentialField(string filePath)
        {
            GeoPotentialField potentialField;
            using (var fileStream = File.OpenText(filePath))
            {
                potentialField = ParseFileHeader(fileStream);
                ParseValues(fileStream, potentialField);
            }
            return potentialField;
        }

        private static void ParseValues(StreamReader fileStream, GeoPotentialField potentialField)
        {
            var firstLine = true;
            var rows = 0;
            var potentialFieldValues = new List<int>();

            while (!fileStream.EndOfStream)
            {
                rows++;
                var splittedLine = fileStream.ReadLine().Split(';');
                if (firstLine)
                {
                    potentialField.NumberOfGridCellsX = splittedLine.Length;
                    firstLine = false;
                }
                var list = new List<string>(splittedLine).Select(int.Parse).ToList();
                potentialFieldValues.AddRange(list);
            }
            potentialField.NumberOfGridCellsY = rows;
            potentialField.PotentialFieldData = potentialFieldValues.ToArray();
        }

        private GeoPotentialField ParseFileHeader(StreamReader streamReader)
        {
            double topLat = 0;
            double leftLon = 0;
            double bottomLat = 0;
            double rightLon = 0;

            var headerRow = 0;

            while (!streamReader.EndOfStream && (headerRow < 5))
            {
                headerRow++;
                var line = streamReader.ReadLine();
                if (headerRow == 1)
                {
                    var split = line.Split('=');
                    topLat = double.Parse(split[1], CultureInfo.InvariantCulture);
                }
                else if (headerRow == 2)
                {
                    var split2 = line.Split('=');
                    leftLon = double.Parse(split2[1], CultureInfo.InvariantCulture);
                }
                else if (headerRow == 3)
                {
                    var split3 = line.Split('=');
                    bottomLat = double.Parse(split3[1], CultureInfo.InvariantCulture);
                }
                else if (headerRow == 4)
                {
                    var split4 = line.Split('=');
                    rightLon = double.Parse(split4[1], CultureInfo.InvariantCulture);
                }
                else if (headerRow == 5)
                {
                    var split5 = line.Split('=');
                    int.Parse(split5[1], CultureInfo.InvariantCulture);
                }
            }
            var geoPotentialField = new GeoPotentialField(topLat, leftLon, bottomLat, rightLon);
            return geoPotentialField;
        }
    }
}