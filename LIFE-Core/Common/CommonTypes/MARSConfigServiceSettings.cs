namespace CommonTypes
{
	public static class MARSConfigServiceSettings
	{
		static string _address = "http://config-svc";
		public static string Address { 
			get { return _address;}  
			set { _address = value; } 
		}
	}
}

