using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DakotaIntegratedSolutions
{
    [Activity(Label = "@+string/FindFiles", Theme = "@style/dialog_light")]
    public class FindFilesActivity : Activity
    {
        ListView fileListView;
        IEnumerable<string> fileList;
        string selectedFile;
        IFileUtil fileUtility;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(GLNLabelPrint.Resource.Layout.FindFiles);

            fileListView = FindViewById<ListView>(GLNLabelPrint.Resource.Id.fileListView);
            fileListView.ItemClick += FileListView_ItemClick; ;
            // set up file utility for saving/loading settings
            fileUtility = new FileUtilImplementation();
            SearchForFiles();
        }

        void SearchForFiles()
        {
            try
            {
                fileList = fileUtility.GetFileList();
                if (fileList.Count<string>() > 0)
                {
                    string[] files = new string[fileList.Count<string>()];
                    for (int i = 0; i < fileList.Count<string>(); i++)
                        files[i] = fileList.ElementAt<string>(i);


                    try
                    {
                        fileListView.Adapter = new ListAlternateRowAdapter(Android.App.Application.Context, Android.Resource.Layout.SimpleListItem1, files);
                    }
                    catch (Exception ex1)
                    {
                        // call LogFile method and pass argument as Exception message, event name, control name, error line number, current form name
                        // fileUtility.LogFile(ex1.Message, ex1.ToString(), MethodBase.GetCurrentMethod().Name, ExceptionHelper.LineNumber(ex1), Class.SimpleName);
                    }
                }
                else
                {
                    var returnIntent = new Intent();
                    returnIntent.PutExtra("filename", "");
                    SetResult(Result.Canceled, returnIntent);
                    Finish();
                }
            }
            catch (Exception ex2)
            {
                // call LogFile method and pass argument as Exception message, event name, control name, error line number, current form name
                // fileUtility.LogFile(ex2.Message, ex2.ToString(), MethodBase.GetCurrentMethod().Name, ExceptionHelper.LineNumber(ex2), Class.SimpleName);
            }
        }

        void FileListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                selectedFile = fileList.ElementAt<string>(e.Position);
            }
            catch (Exception ex)
            {
                // call LogFile method and pass argument as Exception message, event name, control name, error line number, current form name
                // fileUtility.LogFile(ex.Message, ex.ToString(), MethodBase.GetCurrentMethod().Name, ExceptionHelper.LineNumber(ex), Class.SimpleName);
            }

            var returnIntent = new Intent();
            returnIntent.PutExtra("filename", selectedFile);
            SetResult(Result.Ok, returnIntent);
            Finish();
        }
    }
}