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

		public static bool getUserCreated(Context context)
		{
			ISharedPreferences prefs = context.GetSharedPreferences(PREFS_NAME, FileCreationMode.Private);
			if (prefs != null)
			{
				return prefs.GetBoolean(context.GetString(Resource.String.pref_user_created), false);
			}
			return false;
		}

		public static void setUserCreated(Context context, bool created)
		{
			ISharedPreferencesEditor editor = context.GetSharedPreferences(PREFS_NAME, FileCreationMode.Private).Edit();
			editor.PutBoolean(context.GetString(Resource.String.pref_user_created), created);
			editor.Commit();
		}

		public static void SetDefaultName(Context context, string name)
		{
			ISharedPreferencesEditor editor = context.GetSharedPreferences(PREFS_NAME, FileCreationMode.Private).Edit();
			editor.PutString(context.GetString(Resource.String.pref_update_name), name);
			editor.Commit();
		}

		public static void SetDefaultEmail(Context context, string email)
		{
			ISharedPreferencesEditor editor = context.GetSharedPreferences(PREFS_NAME, FileCreationMode.Private).Edit();
			editor.PutString(context.GetString(Resource.String.pref_update_email), email);
			editor.Commit();
		}

	}
}
