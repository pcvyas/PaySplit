using System;
using System.Collections.Generic;
using System.IO;
//using Android.Graphics;
using SQLite;

namespace PaySplit.Droid
{
	public class DataHelper
	{
		
		public string DBPath = null;

		//Create a database
		public bool CreateDataBase(string dbName)
		{

			string folder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "DB");

			if (!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}
			DBPath = System.IO.Path.Combine(folder, dbName);


			return true;
		}

		/*public string SaveImage(string name, Bitmap imageStream)
		{
			string path = null;

			//Create Directory if not present
			string imageFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "images");

			if (!Directory.Exists(imageFolder))
			{
				Directory.CreateDirectory(imageFolder);
			}

			string imageFile = System.IO.Path.Combine(imageFolder, name);

			//Write image to folder
			try
			{
				File.WriteAllBytes(imageFile, imageStream.ToArray<byte>());
				path = imageFile;
			}
			catch
			{
				return null;
			}
			return path;
		}*/

	}
}
