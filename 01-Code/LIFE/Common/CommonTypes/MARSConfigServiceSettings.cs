namespace CommonTypes
{
	public static class MARSConfigServiceSettings
	{
		static string _address = "http://marsconfig:8080";
		public static string Address { 
			get { return _address;}  
			set { _address = value; } 
		}
	}
}

