using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LIFE.Components.ObstacleLayer
{
    public class ObstacleMapLoader
    {
        public ObstacleMap LoadObstacleMap(string filePath)
        {
            ObstacleMap obstacleMap;
            using (var fileStream = File.OpenText(filePath))
            {
                obstacleMap = ParseFileHeader(fileStream);
                ParseValues(fileStream, obstacleMap);
            }
            return obstacleMap;
        }

        private static void ParseValues(StreamReader fileStream, ObstacleMap obstacleMap)
        {
            var firstLine = true;
            var obstacleMapValues = new List<int>();
            var index = 0;

            while (!fileStream.EndOfStream)
            {
                var splittedLine = fileStream.ReadLine().Split(';');
                if (firstLine)
                    firstLine = false;
                var list = new List<string>(splittedLine).Select(int.Parse).ToList();
                obstacleMapValues.AddRange(list);
            }
            foreach (var potentialFieldValue in obstacleMapValues)
            {
                obstacleMap.Grid.TryAdd(index, potentialFieldValue);
                index++;
            }
        }

        private ObstacleMap ParseFileHeader(StreamReader streamReader)
        {
            double topLat = 0;
            double leftLon = 0;
            double bottomLat = 0;
            double rightLon = 0;
            var cellSize = 0;

            var headerRow = 0;

            while (!streamReader.EndOfStream && (headerRow < 5))
            {
                headerRow++;
                var line = streamReader.ReadLine();
                if (headerRow == 1)
                {
                    var split = line.Split('=');
                    topLat = double.Parse(split[1]);
                }
                else if (headerRow == 2)
                {
                    var split2 = line.Split('=');
                    leftLon = double.Parse(split2[1]);
                }
                else if (headerRow == 3)
                {
                    var split3 = line.Split('=');
                    bottomLat = double.Parse(split3[1]);
                }
                else if (headerRow == 4)
                {
                    var split4 = line.Split('=');
                    rightLon = double.Parse(split4[1]);
                }
                else if (headerRow == 5)
                {
                    var split5 = line.Split('=');
                    cellSize = int.Parse(split5[1]);
                }
            }
            var obstacleMap = new ObstacleMap( topLat, bottomLat, leftLon, rightLon, cellSize);
            return obstacleMap;
        }
    }
}