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
    public class CategoryFilter : FilterDecorator
    {
        public CategoryFilter(List<Bill> bills, object parameter) {
            if (parameter == null || parameter.Equals(""))
            {
                this.AddRange(bills);
            } else
            {
                if (parameter is String)
                {
                    List<Bill> filteredBills = new List<Bill>();
                    foreach (Bill bill in bills)
                    {
                        if (parameter.Equals(bill.Category))
                        {
                            filteredBills.Add(bill);
                        }
                    }
                    this.AddRange(filteredBills);
                    
                }
            }
        }
    }
}