using System;
using System.Collections.Generic;
using System.IO;
//using Android.Graphics;
using SQLite;

namespace PaySplit.iOS
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

		/*
		//Need to be tested later
		public string SaveImage(string name, byte[] imageStream)
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
				File.WriteAllBytes(imageFile, imageStream);
				path = imageFile;
			}
			catch
			{
				return null;
			}
			return path;
		}
		*/

	}
}
