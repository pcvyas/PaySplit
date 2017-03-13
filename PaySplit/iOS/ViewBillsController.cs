using Foundation;
using System;
using UIKit;

namespace PaySplit.iOS
{
    public partial class ViewBillsController : UIViewController
    {
		public ViewBillsController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

		}
      
        partial void CreateBillButton_TouchUpInside(UIButton sender)
		{
			UIStoryboard board = UIStoryboard.FromName("Main", null);
			UIViewController ctrl = (UIViewController)board.InstantiateViewController("CreateBill");
			this.PresentViewController(ctrl, true, null);
		}
	}
}