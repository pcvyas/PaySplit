// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace PaySplit.iOS
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        UIKit.UIButton Button { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton addItem { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imageVW { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView splitView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton viewItem { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (addItem != null) {
                addItem.Dispose ();
                addItem = null;
            }

            if (Button != null) {
                Button.Dispose ();
                Button = null;
            }

            if (imageVW != null) {
                imageVW.Dispose ();
                imageVW = null;
            }

            if (splitView != null) {
                splitView.Dispose ();
                splitView = null;
            }

            if (viewItem != null) {
                viewItem.Dispose ();
                viewItem = null;
            }
        }
    }
}