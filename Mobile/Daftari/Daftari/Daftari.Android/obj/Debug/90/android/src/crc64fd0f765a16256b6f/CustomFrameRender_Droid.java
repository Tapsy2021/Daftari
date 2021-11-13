package crc64fd0f765a16256b6f;


public class CustomFrameRender_Droid
	extends crc64720bb2db43a66fe9.FrameRenderer
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Daftari.Droid.Renderers.CustomFrameRender_Droid, Daftari.Android", CustomFrameRender_Droid.class, __md_methods);
	}


	public CustomFrameRender_Droid (android.content.Context p0)
	{
		super (p0);
		if (getClass () == CustomFrameRender_Droid.class)
			mono.android.TypeManager.Activate ("Daftari.Droid.Renderers.CustomFrameRender_Droid, Daftari.Android", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public CustomFrameRender_Droid (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == CustomFrameRender_Droid.class)
			mono.android.TypeManager.Activate ("Daftari.Droid.Renderers.CustomFrameRender_Droid, Daftari.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public CustomFrameRender_Droid (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == CustomFrameRender_Droid.class)
			mono.android.TypeManager.Activate ("Daftari.Droid.Renderers.CustomFrameRender_Droid, Daftari.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
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
