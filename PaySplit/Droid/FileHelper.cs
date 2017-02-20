using System;
using System.IO;
using PaySplit.Droid;
using Xamarin.Forms;


[assembly: Dependency(typeof(FileHelper))]
namespace PaySplit.Droid
{
	public class FileHelper : IFileHelper
	{

		public string GetLocalFilePath(string filename)
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			return Path.Combine(path, filename);
		}
	}
}
