using Foundation;
using System;
using UIKit;

namespace PaySplit.iOS
{
    public partial class CreateBillController : UIViewController
    {
		CameraService cs;
        public CreateBillController (IntPtr handle) : base (handle)
        {
			
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			categoryPicker.Model = new CategoryPickerViewModel();
			createBillScrollView.ContentSize = new CoreGraphics.CGSize(CreateBill.Bounds.Width, 1000);
			cs = new CameraService(picture, this);
			datePicker.Mode = UIDatePickerMode.Date;

			backCreateBill.Clicked += BackCreateBill_Clicked;;

		
			//picture.Hidden = true;
		}

		void BackCreateBill_Clicked(object sender, EventArgs e)
		{
			UIStoryboard board = UIStoryboard.FromName("Main", null);
			UIViewController ctrl = (UIViewController)board.InstantiateViewController("ViewBills");
			this.PresentViewController(ctrl, true, null);
		}

		public class CategoryPickerViewModel : UIPickerViewModel
		{
			public override nint GetComponentCount(UIPickerView picker)
			{
				return 1;
			}

			public override nint GetRowsInComponent(UIPickerView picker, nint component)
			{
				return 5;
			}

			public override string GetTitle(UIPickerView picker, nint row, nint component)
			{

				return "Component " + row.ToString();
			}
		}


		partial void TakePicture_TouchUpInside(UIButton sender)
		{
			cs.StartCamera();
		}
	}
}