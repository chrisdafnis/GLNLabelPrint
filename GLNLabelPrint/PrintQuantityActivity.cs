using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;

namespace DakotaIntegratedSolutions
{
    [Activity(Label = "@+string/PrintQuantity", Theme = "@style/dialog_light")]
    class PrintQuantityActivity : Activity
    {
        Spinner spinQty;
        int quantity = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(GLNLabelPrint.Resource.Layout.PrintQuantity);
            var btnOK = ((Button)FindViewById<Button>(GLNLabelPrint.Resource.Id.buttonOK));
            spinQty = FindViewById<Spinner>(GLNLabelPrint.Resource.Id.spinnerQty); ;
            spinQty.ItemSelected += SpinQty_ItemSelected;
            var adapter = ArrayAdapter.CreateFromResource(
                this, GLNLabelPrint.Resource.Array.quantity_array, Android.Resource.Layout.SimpleSpinnerItem);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinQty.Adapter = adapter;
            btnOK.Click += BtnOK_Click;
        }

        void SpinQty_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = (Spinner)sender;
            quantity = Convert.ToInt32(spinQty.GetItemAtPosition(e.Position).ToString());
        }

        void BtnOK_Click(object sender, EventArgs e)
        {
            var returnIntent = new Intent();
            returnIntent.PutExtra("quantity", quantity);
            SetResult(Result.Ok, returnIntent);
            Finish();
        }
    }
}