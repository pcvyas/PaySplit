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
    public class DateFilter : FilterDecorator
    {
        public DateFilter(List<Bill> bills, object parameter)
        {
            if (parameter == null)
            {
                this.AddRange(bills);
            } else
            {
                if (parameter is DateTime)
                {

                    DateTime date = (DateTime)parameter;
                    List<Bill> filteredBills = new List<Bill>();
                    foreach (Bill bill in bills)
                    {
                        if ((date.Month) == (bill.Date.Month) && (date.Year) == (bill.Date.Year))
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