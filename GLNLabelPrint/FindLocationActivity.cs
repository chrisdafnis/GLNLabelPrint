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
using System.Reflection;
using GLNLabelPrint;

namespace DakotaIntegratedSolutions
{
    [Activity(Label = "@+string/SearchLocation", Theme = "@style/dialog_light")]
    public class FindLocationActivity : Activity
    {
        EditText searchLocationText;
        Button buttonSearch;
        string searchLocation;
        IFileUtil fileUtility;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(GLNLabelPrint.Resource.Layout.FindLocation);

            searchLocationText = FindViewById<EditText>(GLNLabelPrint.Resource.Id.editTextSearch);
            buttonSearch = FindViewById<Button>(GLNLabelPrint.Resource.Id.buttonSearch);
            buttonSearch.Click += ButtonSearch_Click;
            // set up file utility for saving/loading settings
            fileUtility = new FileUtilImplementation();
        }

        private void ButtonSearch_Click(object sender, EventArgs e)
        {
            searchLocation = searchLocationText.Text;
            Intent returnIntent = new Intent();
            returnIntent.PutExtra("location", searchLocation);
            SetResult(Result.Ok, returnIntent);
            Finish();
        }
    }
}