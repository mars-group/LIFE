using System;
using NetTopologySuite.IO;
using NetTopologySuite.Features;

namespace AbdoulayeTestV1
{
    using System.IO;
    using System.Text;

    class MainClass
	{
		public static void Main (string[] args)
		{
			//var json = new WebClient().DownloadString ("http://gis.3ten.de/GROUND/resrc/GROUND/data/togo/awrTreesComplete.json");
		    var txt = new StringBuilder();
            using (FileStream fs = File.Open("./awrTreesComplete.json", FileMode.Open))
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);
                

                while (fs.Read(b, 0, b.Length) > 0) {
                    txt.Append(temp.GetString(b));
                }
            }
		    var json = txt.ToString();
			json = json.Replace("\r\n", string.Empty);
			json = json.Replace (" ", String.Empty);
			var pol = new GeoJsonReader ().Read<FeatureCollection> (json);
            
			Console.ReadLine ();
		}
	}
}
