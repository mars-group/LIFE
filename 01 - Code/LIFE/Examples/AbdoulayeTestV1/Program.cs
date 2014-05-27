using System;
using System.Net;
using NetTopologySuite.IO;
using NetTopologySuite.Features;

namespace AbdoulayeTestV1
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var json = new WebClient().DownloadString ("http://gis.3ten.de/GROUND/resrc/GROUND/data/togo/awrTreesComplete.json");
			json = json.Replace("\r\n", string.Empty);
			json = json.Replace (" ", String.Empty);
			var pol = new GeoJsonReader ().Read<FeatureCollection> (json);

			Console.ReadLine ();
		}
	}
}
