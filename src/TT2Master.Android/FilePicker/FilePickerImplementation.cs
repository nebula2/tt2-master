using Android.Content;
using Android.Runtime;
using System;
using System.Threading;
using System.Threading.Tasks;
using TT2Master.Droid.FilePicker;
using TT2Master.Interfaces;
using TT2Master.Model.FilePicker;
using Xamarin.Forms;

[assembly: Dependency(typeof(FilePickerImplementation))]
namespace TT2Master.Droid.FilePicker
{
    /// <summary>
    /// Implementation for file picking on Android
    /// </summary>
    [Preserve(AllMembers = true)]
    public class FilePickerImplementation : IFilePicker
    {
        /// <summary>
        /// Android context to use for picking
        /// </summary>
        private readonly Context _context;

        /// <summary>
        /// Request ID for current picking call
        /// </summary>
        private int _requestId;

        /// <summary>
        /// Task completion source for task when finished picking
        /// </summary>
        private TaskCompletionSource<FileData> _completionSource;

        /// <summary>
        /// Creates a new file picker implementation
        /// </summary>
        public FilePickerImplementation() => _context = Android.App.Application.Context;

        /// <summary>
        /// Implementation for picking a file on Android.
        /// </summary>
        /// <param name="allowedTypes">
        /// Specifies one or multiple allowed types. When null, all file types
        /// can be selected while picking.
        /// On Android you can specify one or more MIME types, e.g.
        /// "image/png"; also wild card characters can be used, e.g. "image/*".
        /// </param>
        /// <returns>
        /// File data object, or null when user cancelled picking file
        /// </returns>
        public async Task<FileData> PickFile(string[] allowedTypes)
        {
            var fileData = await PickFileAsync(allowedTypes);

            return fileData;
        }

        /// <summary>
        /// File picking implementation
        /// </summary>
        /// <param name="allowedTypes">list of allowed types; may be null</param>
        /// <param name="action">Android intent action to use; unused</param>
        /// <returns>picked file data, or null when picking was cancelled</returns>
        private Task<FileData> PickFileAsync(string[] allowedTypes)
        {
            int id = GetRequestId();

            var ntcs = new TaskCompletionSource<FileData>(id);

            var previousTcs = Interlocked.Exchange(ref _completionSource, ntcs);
            if (previousTcs != null)
            {
                previousTcs.TrySetResult(null);
            }

            try
            {
                var pickerIntent = new Intent(_context, typeof(FilePickerActivity));
                pickerIntent.SetFlags(ActivityFlags.NewTask);

                pickerIntent.PutExtra(FilePickerActivity.ExtraAllowedTypes, allowedTypes);

                _context.StartActivity(pickerIntent);

                EventHandler<FilePickerEventArgs> handler = null;
                EventHandler<FilePickerCancelledEventArgs> cancelledHandler = null;

                handler = (s, e) =>
                {
                    var tcs = Interlocked.Exchange(ref _completionSource, null);

                    FilePickerActivity.FilePickCancelled -= cancelledHandler;
                    FilePickerActivity.FilePicked -= handler;

                    tcs?.SetResult(new FileData(
                        e.FilePath,
                        e.FileName,
                        () =>
                        {
                            if (IOUtil.IsMediaStore(e.FilePath))
                            {
                                var contentUri = Android.Net.Uri.Parse(e.FilePath);
                                return Android.App.Application.Context.ContentResolver.OpenInputStream(contentUri);
                            }
                            else
                            {
                                return System.IO.File.OpenRead(e.FilePath);
                            }
                        }));
                };

                cancelledHandler = (s, e) =>
                {
                    var tcs = Interlocked.Exchange(ref _completionSource, null);

                    FilePickerActivity.FilePickCancelled -= cancelledHandler;
                    FilePickerActivity.FilePicked -= handler;

                    if (e?.Exception != null)
                    {
                        tcs?.SetException(e.Exception);
                    }
                    else
                    {
                        tcs?.SetResult(null);
                    }
                };

                FilePickerActivity.FilePickCancelled += cancelledHandler;
                FilePickerActivity.FilePicked += handler;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
                System.Diagnostics.Debug.Write(ex.Data);
                _completionSource.SetException(ex);
            }

            return _completionSource.Task;
        }

        /// <summary>
        /// Returns a new request ID for a new call to PickFile()
        /// </summary>
        /// <returns>new request ID</returns>
        private int GetRequestId()
        {
            int id = _requestId;

            if (_requestId == int.MaxValue)
            {
                _requestId = 0;
            }
            else
            {
                _requestId++;
            }

            return id;
        }
    }
}