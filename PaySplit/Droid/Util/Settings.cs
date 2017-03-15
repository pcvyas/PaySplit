using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PaySplit.Droid
{
	public static class Settings
	{

		private const string PREFS_NAME = "paysplit_prefs";
		private const string SETTING_USER_CREATED = "setting_user_created";

		public static bool getUserCreated(Context context)
		{
			ISharedPreferences prefs = context.GetSharedPreferences(PREFS_NAME, FileCreationMode.Private);
			if (prefs != null)
			{
				return prefs.GetBoolean(SETTING_USER_CREATED, false);
			}
			return false;
		}

		public static void setUserCreated(Context context, bool created)
		{
			ISharedPreferencesEditor editor = context.GetSharedPreferences(PREFS_NAME, FileCreationMode.Private).Edit();
			editor.PutBoolean(SETTING_USER_CREATED, created);
			editor.Commit();
		}

	}
}
