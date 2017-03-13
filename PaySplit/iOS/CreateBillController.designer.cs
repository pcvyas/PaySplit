// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace PaySplit.iOS
{
    [Register ("CreateBillController")]
    partial class CreateBillController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField amount { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel amountLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel billName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel categoryLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView categoryPicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView CreateBill { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView createBillScrollView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel dateLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIDatePicker datePicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField description { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel descriptionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField name { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField people { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView picture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton takePicture { get; set; }

        [Action ("TakePicture_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void TakePicture_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (amount != null) {
                amount.Dispose ();
                amount = null;
            }

            if (amountLabel != null) {
                amountLabel.Dispose ();
                amountLabel = null;
            }

            if (billName != null) {
                billName.Dispose ();
                billName = null;
            }

            if (categoryLabel != null) {
                categoryLabel.Dispose ();
                categoryLabel = null;
            }

            if (categoryPicker != null) {
                categoryPicker.Dispose ();
                categoryPicker = null;
            }

            if (CreateBill != null) {
                CreateBill.Dispose ();
                CreateBill = null;
            }

            if (createBillScrollView != null) {
                createBillScrollView.Dispose ();
                createBillScrollView = null;
            }

            if (dateLabel != null) {
                dateLabel.Dispose ();
                dateLabel = null;
            }

            if (datePicker != null) {
                datePicker.Dispose ();
                datePicker = null;
            }

            if (description != null) {
                description.Dispose ();
                description = null;
            }

            if (descriptionLabel != null) {
                descriptionLabel.Dispose ();
                descriptionLabel = null;
            }

            if (name != null) {
                name.Dispose ();
                name = null;
            }

            if (people != null) {
                people.Dispose ();
                people = null;
            }

            if (picture != null) {
                picture.Dispose ();
                picture = null;
            }

            if (takePicture != null) {
                takePicture.Dispose ();
                takePicture = null;
            }
        }
    }
}