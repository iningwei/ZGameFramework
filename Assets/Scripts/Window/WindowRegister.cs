using ZGame.Window;
public class WindowRegister
{
	public static void Register()
	{
		var winManager=WindowManager.Instance;
		winManager.RegisterWindowType(WindowNames.NetMaskWindow,typeof(NetMaskWindow).ToString(),"NetMaskWindow");
		winManager.RegisterWindowType(WindowNames.TipWindow,typeof(TipWindow).ToString(),"TipWindow");
	}
}