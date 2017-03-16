
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;

#pragma warning disable CS0618 // Type or member is obsolete
namespace PaySplit.Droid
{
	[Activity(Label = "Settings")]
	public class SettingsActivity : PreferenceActivity, ISharedPreferencesOnSharedPreferenceChangeListener
	{

		private GenDataService mDBS;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.AddPreferencesFromResource(Resource.Xml.preferences_settings);
			PreferenceManager.SetDefaultValues(this, Resource.Xml.preferences_settings, true);

			mDBS = DataHelper.getInstance().getGenDataService();

			try
			{
				Contact c = mDBS.getUserContactInformation();
				Settings.SetDefaultName(this, c.FullName);
				FindPreference(GetString(Resource.String.pref_update_name)).Summary = c.FullName;

				Settings.SetDefaultName(this, c.Email);
				FindPreference(GetString(Resource.String.pref_update_email)).Summary = c.Email;
			}
			catch (Exception)
			{
				Toast.MakeText(this, "Unable to get contact", ToastLength.Short).Show();
				// Unable to fetch contact and update fields
			}
		}

		public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
		{
			if (key == null)
			{
				return;
			}

			Preference pref = FindPreference(key);
			if (key.Equals(GetString(Resource.String.pref_update_name)))
			{
				EditTextPreference etp = (EditTextPreference)pref;
				pref.Summary = etp.Text;

				Contact c = mDBS.getUserContactInformation();
				c.FullName = etp.Text;

				mDBS.UpdateUserContactInformation(c);
			}
			else if (key.Equals(GetString(Resource.String.pref_update_email)))
			{
				EditTextPreference etp = (EditTextPreference)pref;
				pref.Summary = etp.Text;

				Contact c = mDBS.getUserContactInformation();
				c.Email = etp.Text;

				mDBS.UpdateUserContactInformation(c);
			}
			else
			{
				Toast.MakeText(this, key, ToastLength.Short).Show();
			}
		}

		protected override void OnPause()
		{
			base.OnPause();
			if (this.PreferenceManager != null)
			{
				this.PreferenceManager.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);
			}
		}

		protected override void OnResume()
		{
			base.OnResume();
			if (this.PreferenceManager != null)
			{
				this.PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
			}
		}

	}
}
#pragma warning disable CS0618 // Type or member is obsolete