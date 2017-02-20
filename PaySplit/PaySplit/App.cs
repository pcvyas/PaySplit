using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;

//[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace PaySplit
{
	public class App : Application
	{
		static SplitDatabase database;

		public App()
		{
			Resources = new ResourceDictionary();
			Resources.Add("primaryGreen", Color.FromHex("91CA47"));
			Resources.Add("primaryDarkGreen", Color.FromHex("6FA22E"));

		}

		public static SplitDatabase Database
		{
			get
			{
				if (database == null)
				{
					database = new SplitDatabase(DependencyService.Get<IFileHelper>().GetLocalFilePath("SplitSQLite.db3"));
				}
				return database;
			}
		}

		public int ResumeAtTodoId { get; set; }

		protected override void OnStart()
		{

		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}

