using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimPanViewer
{
    public static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
           // Application.Run(new SimPanForm());
        }

        public static Form StartForm(int cellCountHorizontal, int cellCountVertical, int cellSideLength, Dictionary<int, object[]> cellData)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form form = new SimPanForm(cellCountHorizontal, cellCountVertical, cellSideLength, cellData);
            Application.Run(form);
            return form;
        }

        
        
    }
}
