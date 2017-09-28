using System;
using Android.App;

using AndroidHUD;

namespace XHUD
{
	public enum MaskType
	{
//		None = 1,
		Clear,
		Black,
//		Gradient
	}

	public static class HUD
	{
		public static Activity MyActivity;

		public static void Show(string message, int progress = -1, MaskType maskType = MaskType.Black)
		{
            AndroidHUD.AndroidHUD.Shared.Show(HUD.MyActivity, message, progress,(AndroidHUD.MaskType)maskType);
		}

		public static void Dismiss()
		{
            AndroidHUD.AndroidHUD.Shared.Dismiss(HUD.MyActivity);
		}

		public static void ShowToast(string message, bool showToastCentered = true, double timeoutMs = 1000)
		{
            AndroidHUD.AndroidHUD.Shared.ShowToast(HUD.MyActivity, message, (AndroidHUD.MaskType)MaskType.Black, TimeSpan.FromSeconds(timeoutMs / 1000), showToastCentered);
		}

		public static void ShowToast(string message, MaskType maskType, bool showToastCentered = true, double timeoutMs = 1000)
		{
            AndroidHUD.AndroidHUD.Shared.ShowToast(HUD.MyActivity, message, (AndroidHUD.MaskType)maskType, TimeSpan.FromSeconds(timeoutMs / 1000), showToastCentered);
		}
	}
}

