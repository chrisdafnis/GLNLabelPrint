using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Zebra.Sdk.Comm;
using static Android.Widget.AdapterView;

namespace DakotaIntegratedSolutions
{
    [Activity(Label = "@+string/AppName", MainLauncher = true, Icon = "@drawable/dakota_healthcare_icon", Theme = "@android:style/Theme.Holo.Light", ConfigurationChanges = (Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize))]
    public class MainActivity : Activity
    {
        IZebraPrinter zebraPrinter;
        IFileUtil fileUtility;
        ListView locationsView;
        ObservableCollection<IGLNLocation> locationList = new ObservableCollection<IGLNLocation>();
        int printQuantity = 1;
        string locationsFile;
        // int currentSelected = 0;
        public enum ActivityCode { FindPrinters = 0, PrintQuantity, LoadLocations, LocationSearch, About/*, LocationInfo, */ };

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(GLNLabelPrint.Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            var printButton = FindViewById<Button>(GLNLabelPrint.Resource.Id.PrintButton);
            printButton.Click += PrintButton_Click;
            // disable print button until we have a printer connected and something selected to print
            printButton.Enabled = false;
            var selectedPrinter = (TextView)FindViewById<TextView>(GLNLabelPrint.Resource.Id.selectedPrinterTxt);
            selectedPrinter.Text = "";

            locationsView = ((ListView)FindViewById<ListView>(GLNLabelPrint.Resource.Id.locationsView));
            locationList = new System.Collections.ObjectModel.ObservableCollection<IGLNLocation>();
            locationsView.Adapter = new CheckListCustomArrayAdapter(Android.App.Application.Context, Android.Resource.Layout.SimpleListItemChecked, locationList);
            locationsView.ItemClick += LocationsView_ItemClick;
            locationsView.ItemSelected += LocationsView_ItemSelected;

            // set up file utility for saving/loading settings
            fileUtility = new FileUtilImplementation();
            zebraPrinter = (IZebraPrinter)fileUtility.LoadXMLSettings();
            try
            {
                ((TextView)FindViewById<TextView>(GLNLabelPrint.Resource.Id.selectedPrinterTxt)).Text = zebraPrinter.FriendlyName;
                printButton.Enabled = true;
            }
            catch (Exception ex)
            {
                ((TextView)FindViewById<TextView>(GLNLabelPrint.Resource.Id.selectedPrinterTxt)).SetText(GLNLabelPrint.Resource.String.NoPrinter);

                // call LogFile method and pass argument as Exception message, event name, control name, error line number, current form name
                fileUtility.LogFile(ex.Message, ex.ToString(), MethodBase.GetCurrentMethod().Name, ExceptionHelper.LineNumber(ex), Class.SimpleName);
            }

            // #if DEBUG
            // #else
            if (!AntiPiracyCheck())
            {
                var dialogBuilder = new AlertDialog.Builder(this);

                dialogBuilder.SetTitle("Error");
                dialogBuilder.SetMessage("This software is not permitted for use on this device.\n\rPlease contact your IT department.");
                dialogBuilder.SetIcon(Android.Resource.Drawable.IcDialogAlert);
                dialogBuilder.SetPositiveButton(Android.Resource.String.Ok, delegate
                    {
                        Finish();
                    });
                dialogBuilder.Show();
            }
            else
            {
                // load list of locations files
                var findFilesPage = new Android.Content.Intent(this, typeof(FindFilesActivity));
                StartActivityForResult(findFilesPage, (int)ActivityCode.LoadLocations);
            }
            // #endif

            // var findFilesPage = new Android.Content.Intent(this, typeof(FindFilesActivity));
            // StartActivityForResult(findFilesPage, (int)ActivityCode.LoadLocations);
        }

        void LocationsView_ItemSelected(object sender, ItemSelectedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void LocationsView_ItemClick(object sender, EventArgs e)
        {
            // loc.Selected = check;
            // ((CheckListCustomArrayAdapter)locationsView.Adapter).SetChecked(pos, check);
        }

        bool AntiPiracyCheck()
        {
            IList<string> validDeviceSerialNumbers = Resources.GetStringArray(GLNLabelPrint.Resource.Array.valid_devices);
            // new string[] { "FHP5CM00227", "FHP5CM00269", "FHP5CM00232", "FHP5CM00144", "FHP5CM00013", "FHP4AM00107", "FHP52M00438", "FHP52M00075", "FHP52M00242" };
            var isValid = false;

            if (validDeviceSerialNumbers.Contains(Build.Serial))
                isValid = true;

            return isValid;
        }

        public override void OnBackPressed()
        {
            var dialogBuilder = new AlertDialog.Builder(this);

            dialogBuilder.SetTitle(GLNLabelPrint.Resource.String.QuitApplication);
            dialogBuilder.SetMessage(GLNLabelPrint.Resource.String.QuitPrompt);
            dialogBuilder.SetIcon(Android.Resource.Drawable.IcDialogAlert);
            dialogBuilder.SetNegativeButton(Android.Resource.String.No, delegate { });
            dialogBuilder.SetPositiveButton(Android.Resource.String.Yes, delegate
            {
                Finish();
            });
            dialogBuilder.Show();
        }

        void TogglePrintButtonEnabled()
        {
            var printButton = FindViewById<Button>(GLNLabelPrint.Resource.Id.PrintButton);
            printButton.Enabled = !printButton.Enabled;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(GLNLabelPrint.Resource.Menu.popup_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case GLNLabelPrint.Resource.Id.FindPrinters:
                    var findPrintersPage = new Android.Content.Intent(this, typeof(FindPrintersActivity));
                    StartActivityForResult(findPrintersPage, (int)ActivityCode.FindPrinters);
                    return true;
                case GLNLabelPrint.Resource.Id.LoadDataFile:
                    var findFilesPage = new Android.Content.Intent(this, typeof(FindFilesActivity));
                    StartActivityForResult(findFilesPage, (int)ActivityCode.LoadLocations);
                    return true;
                case GLNLabelPrint.Resource.Id.SelectAllLocations:
                    CheckAllLocations(true);
                    return true;
                case GLNLabelPrint.Resource.Id.SelectNoLocations:
                    CheckAllLocations(false);
                    return true;
                //case GLNLabelPrint.Resource.Id.SearchLocation:
                //    var searchLocationPage = new Android.Content.Intent(this, typeof(FindLocationActivity));
                //    StartActivityForResult(searchLocationPage, (int)ActivityCode.LocationSearch);
                //    return true;
                case GLNLabelPrint.Resource.Id.About:
                    var aboutPage = new Android.Content.Intent(this, typeof(AboutButtonActivity));
                    StartActivityForResult(aboutPage, (int)ActivityCode.About);
                    return true;
                case GLNLabelPrint.Resource.Id.Quit:
                    var dialogBuilder = new AlertDialog.Builder(this);

                    dialogBuilder.SetTitle(GLNLabelPrint.Resource.String.QuitApplication);
                    dialogBuilder.SetMessage(GLNLabelPrint.Resource.String.QuitPrompt);
                    dialogBuilder.SetIcon(Android.Resource.Drawable.IcDialogAlert);
                    dialogBuilder.SetNegativeButton(Android.Resource.String.No, delegate { });
                    dialogBuilder.SetPositiveButton(Android.Resource.String.Yes, delegate
                    {
                        Finish();
                    });
                    dialogBuilder.Show();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        void CheckAllLocations(bool check)
        {
            var pos = 0;
            foreach (IGLNLocation loc in locationList)
            {
                loc.Selected = check;
                ((CheckListCustomArrayAdapter)locationsView.Adapter).SetChecked(pos, check);
                pos++;
            }

            ((CheckListCustomArrayAdapter)locationsView.Adapter).NotifyDataSetInvalidated();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            switch ((ActivityCode)requestCode)
            {
                case ActivityCode.FindPrinters:
                    {
                        if (resultCode == Result.Ok)
                        {
                            var result = data.GetStringExtra("result");
                            LoadXMLSettings();
                        }

                        if (resultCode == Result.Canceled)
                        {
                            // Write your code if there's no result
                        }
                    }

                    break;
                case ActivityCode.PrintQuantity:
                    {
                        if (resultCode == Result.Ok)
                        {
                            printQuantity = data.GetIntExtra("quantity", 1);
                            new Task(new Action(() =>
                            {
                                SendZplOverBluetooth();
                            })).Start();
                            try
                            {
                            }
                            catch (Exception ex)
                            {
                                // call LogFile method and pass argument as Exception message, event name, control name, error line number, current form name
                                fileUtility.LogFile(ex.Message, ex.ToString(), MethodBase.GetCurrentMethod().Name, ExceptionHelper.LineNumber(ex), Class.SimpleName);
                            }
                        }

                        if (resultCode == Result.Canceled)
                        {
                            // Write your code if there's no result
                        }
                    }

                    break;
                case ActivityCode.LoadLocations:
                    {
                        if (resultCode == Result.Ok)
                        {
                            locationsFile = data.GetStringExtra("filename");
                            LoadFile(locationsFile);
                        }
                    }

                    break;
                case ActivityCode.LocationSearch:
                    {
                        if (resultCode == Result.Ok)
                        {
                            var searchLocation = data.GetStringExtra("location");
                            var adapter = (ListCustomArrayAdapter)locationsView.Adapter;
                            var i = 0;
                            var found = false;
                            foreach (IGLNLocation location in locationList)
                            {
                                if (location.Code == searchLocation)
                                {
                                    found = true;
                                    adapter.SetSelectedIndex(i);
                                    // currentSelected = ((ListCustomArrayAdapter)locationsView.Adapter).GetSelectedIndex();
                                    locationsView.SmoothScrollToPosition(i);
                                    break;
                                }

                                i++;
                            }

                            if (!found)
                            {
                                var dialogBuilder = new AlertDialog.Builder(this);

                                dialogBuilder.SetTitle("Code Not Found");
                                dialogBuilder.SetMessage("The room code '" + searchLocation + "' does not exist in the current database");
                                dialogBuilder.SetIcon(Android.Resource.Drawable.IcDialogAlert);
                                dialogBuilder.SetPositiveButton(Android.Resource.String.Ok, delegate { });
                                dialogBuilder.Show();
                            }
                        }

                        if (resultCode == Result.Canceled)
                        {
                            // Write your code if there's no result
                        }
                    }

                    break;
            }
        }

        public async void SaveFile(string filename, IGLNLocation location)
        {
            try
            {
                AndHUD.Shared.Show(this, "Updating...", -1, MaskType.Black);
                await Task.Factory.StartNew(() =>
                fileUtility.SaveLocation(filename, location));
                AndHUD.Shared.Dismiss(this);
            }
            catch (Exception ex)
            {
                // call LogFile method and pass argument as Exception message, event name, control name, error line number, current form name
                fileUtility.LogFile(ex.Message, ex.ToString(), MethodBase.GetCurrentMethod().Name, ExceptionHelper.LineNumber(ex), GetType().Name);
            }
        }
        public async void LoadFile(string filename)
        {
            var xmlString = string.Empty;
            object res = null;
            try
            {
                AndHUD.Shared.Show(this, "Loading...", -1, MaskType.Black);
                Func<string> function = new Func<string>(() => LoadLocations(filename));
                res = await Task.Factory.StartNew<string>(function);
                AndHUD.Shared.Dismiss(this);
                res = LoadLocations(filename);

                var xDoc = XDocument.Parse((string)res);
                if (xDoc != null)
                {
                    var xName = XName.Get("GLNLocation");

                    foreach (XElement xElem in xDoc.Descendants("GLNLocation"))
                    {
                        IGLNLocation location = new GLNLocation()
                        {
                            Region = xElem.Element("Region").Value,
                            Site = xElem.Element("Site").Value,
                            Building = xElem.Element("Building").Value,
                            Floor = xElem.Element("Floor").Value,
                            Room = xElem.Element("Room").Value,
                            Code = xElem.Element("Code").Value,
                            GLN = xElem.Element("GLN").Value,
                            Date = Convert.ToDateTime(xElem.Element("GLNCreationDate").Value),
                            Printed = Convert.ToBoolean(xElem.Element("Printed").Value),
                            ToPrint = false
                        };
                        if (!locationList.Contains(location))
                        {
                            locationList.Add(location);
                        }
                    }

                    try
                    {
                        locationsView.Adapter = new CheckListCustomArrayAdapter(Android.App.Application.Context, Android.Resource.Layout.SimpleListItemChecked, locationList);
                        if (((CheckListCustomArrayAdapter)locationsView.Adapter).CheckForItemsToPrint())
                        {
                            FindViewById<Button>(GLNLabelPrint.Resource.Id.PrintButton).Enabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        // call LogFile method and pass argument as Exception message, event name, control name, error line number, current form name
                        fileUtility.LogFile(ex.Message, ex.ToString(), MethodBase.GetCurrentMethod().Name, ExceptionHelper.LineNumber(ex), Class.SimpleName);
                    }
                }
                else
                {
                    var dialogBuilder = new AlertDialog.Builder(this);

                    dialogBuilder.SetTitle("File Error");
                    dialogBuilder.SetMessage("There was a problem loading this file. Please choose another file, or correct the error and try again.");
                    dialogBuilder.SetIcon(Android.Resource.Drawable.IcDialogAlert);
                    dialogBuilder.SetPositiveButton(Android.Resource.String.Ok, delegate { });
                    dialogBuilder.Show();
                }
            }
            catch (Exception ex)
            {//call LogFile method and pass argument as Exception message, event name, control name, error line number, current form name
                fileUtility.LogFile(ex.Message, ex.ToString(), MethodBase.GetCurrentMethod().Name, ExceptionHelper.LineNumber(ex), GetType().Name);
            }
        }

        string LoadLocations(string filename)
        {
            var xDoc = (XDocument)fileUtility.LoadGLNFile(filename);
            var xmlDocString = xDoc.ToString();
            var xmlHead = @"<?xml version=""1.0"" encoding=""utf-8"" ?>";
            // string returnValue = String.Concat(@"<?xml version=""1.0"" encoding=""utf-8"" ?>", xDoc.ToString());
            var returnValue = xmlHead + xmlDocString;
            return returnValue;
        }

        void SaveLocations(string filename, IGLNLocation location)
        {
            fileUtility.SaveLocation(filename, location);
        }

        void LoadXMLSettings()
        {
            zebraPrinter = (IZebraPrinter)fileUtility.LoadXMLSettings();
            try
            {
                ((TextView)FindViewById<TextView>(GLNLabelPrint.Resource.Id.selectedPrinterTxt)).Text = zebraPrinter.FriendlyName;
                // ((Button)FindViewById<Button>(GLNLabelPrint.Resource.Id.PrintButton)).Enabled = true;
            }
            catch (Exception ex)
            {
                // call LogFile method and pass argument as Exception message, event name, control name, error line number, current form name
                fileUtility.LogFile(ex.Message, ex.ToString(), MethodBase.GetCurrentMethod().Name, ExceptionHelper.LineNumber(ex), Class.SimpleName);
            }
        }

        void PrintButton_Click(object sender, EventArgs e)
        {
            if (CheckPrinter())
            {
                locationsView = ((ListView)FindViewById<ListView>(GLNLabelPrint.Resource.Id.locationsView));

                if (locationList != null)
                {
                    if (((CheckListCustomArrayAdapter)locationsView.Adapter).CheckForItemsToPrint())
                    {
                        var printQuantityPage = new Android.Content.Intent(this, typeof(PrintQuantityActivity));
                        StartActivityForResult(printQuantityPage, (int)ActivityCode.PrintQuantity);
                    }
                }
            }
        }

        bool SendZplOverBluetooth()
        {
            var success = false;
            try
            {
                var connection = ConnectionBuilder.Build("BT:" + zebraPrinter.MACAddress);
                // Open the connection - physical connection is established here.
                connection.Open();

                /* New change, print multiple labels from those selected */
                var pos = 0;
                foreach (IGLNLocation loc in locationList)
                {
                    if (loc.ToPrint)
                    {
                        // Actual Label
                        var zplData = GetZplGLNLabel(loc, pos);
                        fileUtility.LogFile("ZPL Output Debug", zplData, "MainActivity", 440, "SendZplOverBluetooth");

                        // Send the data to printer as a byte array.
                        byte[] response = connection.SendAndWaitForResponse(GetBytes(zplData), 3000, 1000, "\"");
                        locationList[pos].Printed = true;
                        locationList[pos].ToPrint = false;
                        SaveFile(locationsFile, locationList[pos]);

                        RunOnUiThread(() => ((CheckListCustomArrayAdapter)locationsView.Adapter).NotifyDataSetChanged());
                        ((CheckListCustomArrayAdapter)locationsView.Adapter).SetPrintedIndex(pos);
                        ((CheckListCustomArrayAdapter)locationsView.Adapter).SetRowPrinted(((CheckListCustomArrayAdapter)locationsView.Adapter).GetView(pos, null, null) as CheckedTextView, pos);
                    }

                    pos++;
                }

                connection.Close();
                success = true;
            }
            catch (Zebra.Sdk.Comm.ConnectionException ex)
            {
                // call LogFile method and pass argument as Exception message, event name, control name, error line number, current form name
                fileUtility.LogFile(ex.Message, ex.ToString(), MethodBase.GetCurrentMethod().Name, ExceptionHelper.LineNumber(ex), Class.SimpleName);
            }

            return success;
        }

        byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length];
            bytes = Encoding.UTF8.GetBytes(str);
            return bytes;
        }

        string GetZplGLNLabel(IGLNLocation location, int position)
        {
            var zpl =
                @"^XA" + "\r\n" +
                @"^MMT" + "\r\n" +
                @"^PW601" + "\r\n" +
                @"^LL0406" + "\r\n" +
                @"^LS0" + "\r\n";
            if (location == null)
            {
                zpl +=
                    @"^BY3,3,230^FT508,109^BCI,,N,N^FD>;>8414" + "1234567890123" + "^FS" + "\r\n" +
                    @"^FT441,71^A0I,34,33^FB276,1,0,C^FH\^FD(414)" + "1234567890123" + "\r\n";
            }
            else
            {
                if (locationList[position].Region == "ROYAL CORNWALL HOSPITALS NHS TRUST")
                {
                    // Royal Cornwall want the room code above the barcode
                    zpl +=
                        @"^FT591,340^A0I,54,52^FD" + "Room Number:" + locationList[position].Code + "^FS" + "\r\n" +
                        @"^BY3,3,230^FT508,89^BCI,,N,N^FD>;>8414" + locationList[position].GLN + "^FS" + "\r\n" +
                        @"^FT441,41^A0I,34,33^FB276,1,0,C^FH\^FD(414)" + locationList[position].GLN + "\r\n";
                    // @"^FT591,360^A0I,40,39^FD" + "Room Number:" + locationList[currentSelected].Code + "^FS" + "\r\n" +
                    // @"^BY3,3,230^FT508,109^BCI,,N,N^FD>;>8414" + locationList[currentSelected].GLN + "^FS" + "\r\n" +
                    // @"^FT441,71^A0I,34,33^FB276,1,0,C^FH\^FD(414)" + locationList[currentSelected].GLN + "\r\n";
                }
                else
                {
                    zpl +=
                        @"^BY3,3,230^FT508,109^BCI,,N,N^FD>;>8414" + locationList[position].GLN + "^FS" + "\r\n" +
                        @"^FT441,71^A0I,34,33^FB276,1,0,C^FH\^FD(414)" + locationList[position].GLN + "\r\n";
                }
            }

            zpl += @"^PQ" + printQuantity + ",0,1,Y^XZ" + "\r\n";
            return zpl;
        }

        bool CheckPrinter()
        {
            if (null == zebraPrinter)
                return false;
            else
                return true;
        }
    }
}