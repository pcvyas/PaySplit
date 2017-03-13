using System;
using System.Collections.Generic;
using System.IO;
//using Android.Graphics;
using SQLite;

namespace PaySplit.Droid
{
	public class DataHelper
	{

		private static DataHelper sInstance = null;
		public static GenDataService sGenDataService;

		protected DataHelper()
		{
			string folder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "DB");
			if (!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}
			sGenDataService = new GenDataService(System.IO.Path.Combine(folder, Constants.PAYSPLIT_DB_NAME));
		}	

		public static DataHelper getInstance()
		{
			if (sInstance == null)
			{
				sInstance = new DataHelper();
			}
			return sInstance;
		}

		public GenDataService getGenDataService()
		{
			return sGenDataService;
		}
	}
}
