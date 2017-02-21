using System;
using UIKit;

namespace PaySplit.iOS
{
	public class CameraService
	{
		UIImagePickerController iPicker;
		UIImageView imageVW;
		UIViewController viewController;
		public UIImage Image { get; set; }

		public CameraService(UIImageView imageView, UIViewController viewController)
		{
			this.imageVW = imageView;
			this.viewController = viewController;
			this.iPicker = new UIImagePickerController();
			Image = null;

			//Handle user picks
			iPicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
			iPicker.Canceled += Handle_Canceled;
		}

		//Start Camera
		public void StartCamera()
		{
			if (UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera))
			{
				iPicker.SourceType = UIImagePickerControllerSourceType.Camera;
			}
			else
			{
				iPicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			}
			viewController.PresentViewController(iPicker, true, null);
		}

		//Photo Capture Cancelled
		public void Handle_Canceled(object sender, EventArgs e)
		{
			iPicker.DismissModalViewController(true);
		}

		//Photo taken
		public void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
		{

			bool isImage = false;
			switch (e.Info[UIImagePickerController.MediaType].ToString())
			{
				case "public.image":
					isImage = true;
					break;
			}

			// if it was an image, get the other image info
			if (isImage)
			{
				// get the original image
				Image = e.Info[UIImagePickerController.OriginalImage] as UIImage;
				if (Image != null)
				{
					imageVW.Image = Image; // display
				}
			}

			// dismiss the picker
			iPicker.DismissModalViewController(true);
		}
	}
}
