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

            

		    var json = "";
            using (TextReader txtReader = new StreamReader(@"./awrMini.json"))
            {
                json = txtReader.ReadToEnd();
            }
		    //var i = 1;
            //json = "{\"features\":[{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[23.0,56.0]},\"properties\":{\"test1\":\"value1\"}}],\"type\":\"FeatureCollection\",\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"name1\"}}}";
			json = json.Replace("\r\n", string.Empty);
			json = json.Replace (" ", String.Empty);
			var pol = new GeoJsonReader ().Read<FeatureCollection> (json);
            
			Console.ReadLine ();
		}
	}
}
