using System;
namespace PaySplit.Droid
{
	public static class Debugger
	{

		public static void Log(string message)
		{
			System.Diagnostics.Debug.WriteLine(message);
		}
	}
}
