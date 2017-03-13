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
    [Register ("ViewBillsController")]
    partial class ViewBillsController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView billList { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton createBillButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISearchBar searchBills { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ViewBills { get; set; }

        [Action ("CreateBillButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CreateBillButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (billList != null) {
                billList.Dispose ();
                billList = null;
            }

            if (createBillButton != null) {
                createBillButton.Dispose ();
                createBillButton = null;
            }

            if (searchBills != null) {
                searchBills.Dispose ();
                searchBills = null;
            }

            if (ViewBills != null) {
                ViewBills.Dispose ();
                ViewBills = null;
            }
        }
    }
}