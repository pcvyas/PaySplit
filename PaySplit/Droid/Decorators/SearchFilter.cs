using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PaySplit.Droid.Decorators
{
    public class SearchFilter : FilterDecorator
    {
        public SearchFilter(List<Bill> bills, object parameter)
        {
            if (parameter == null || parameter.Equals(""))
            {
                this.AddRange(bills);
            } else
            {
                if (parameter is String)
                {
                    // Compare constraint to all names uppercased. 
                    // It they are contained they are added to results.
                    this.AddRange(
                        bills.Where(
                            bill => bill.Name.ToUpper().Contains(parameter.ToString().ToUpper())));
                }
            }
        }
    }
}