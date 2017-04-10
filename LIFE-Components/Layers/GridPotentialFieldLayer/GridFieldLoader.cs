using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LIFE.Components.GridPotentialFieldLayer
{
    public class GridFieldLoader : IFieldLoader<PotentialField>
    {
        public PotentialField LoadPotentialField(string filePath)
        {
            var potentialField = new PotentialField();

            using (var fileStream = File.OpenText(filePath))
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
            return potentialField;
        }
    }
}