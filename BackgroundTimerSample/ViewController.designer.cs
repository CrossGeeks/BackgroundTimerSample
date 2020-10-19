// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BackgroundTimerSample
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIButton toggleTimerButton { get; set; }

		[Action ("ToggleTimer:")]
		partial void ToggleTimer (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (toggleTimerButton != null) {
				toggleTimerButton.Dispose ();
				toggleTimerButton = null;
			}
		}
	}
}
