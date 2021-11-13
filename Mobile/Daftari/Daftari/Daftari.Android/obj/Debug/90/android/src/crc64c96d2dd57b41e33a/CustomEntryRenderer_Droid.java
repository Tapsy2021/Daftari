package crc64c96d2dd57b41e33a;


public class CustomEntryRenderer_Droid
	extends crc643f46942d9dd1fff9.EntryRenderer
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Droid.Droid.Renderers.CustomEntryRenderer_Droid, Daftari.Android", CustomEntryRenderer_Droid.class, __md_methods);
	}


	public CustomEntryRenderer_Droid (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == CustomEntryRenderer_Droid.class)
			mono.android.TypeManager.Activate ("Droid.Droid.Renderers.CustomEntryRenderer_Droid, Daftari.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public CustomEntryRenderer_Droid (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == CustomEntryRenderer_Droid.class)
			mono.android.TypeManager.Activate ("Droid.Droid.Renderers.CustomEntryRenderer_Droid, Daftari.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public CustomEntryRenderer_Droid (android.content.Context p0)
	{
		super (p0);
		if (getClass () == CustomEntryRenderer_Droid.class)
			mono.android.TypeManager.Activate ("Droid.Droid.Renderers.CustomEntryRenderer_Droid, Daftari.Android", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}

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
