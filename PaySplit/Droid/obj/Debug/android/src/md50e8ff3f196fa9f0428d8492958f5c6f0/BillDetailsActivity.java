package md50e8ff3f196fa9f0428d8492958f5c6f0;


public class BillDetailsActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("PaySplit.Droid.BillDetailsActivity, PaySplit.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", BillDetailsActivity.class, __md_methods);
	}


	public BillDetailsActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == BillDetailsActivity.class)
			mono.android.TypeManager.Activate ("PaySplit.Droid.BillDetailsActivity, PaySplit.Droid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}