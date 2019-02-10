using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml.Controls;
using DPPaint.Shapes;
using DPPaint.Visitor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DPPaint.Commands.UserAction
{
    /// <summary>
    /// This class handles the saving of the current state into a file
    /// For this, JSON formatting is used
    /// </summary>
    public class SaveFileCommand : IUserActionCommand
    {
        #region Properties

        /// <inheritdoc />
        public List<PaintBase> ShapeList { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> UndoStack { get; set; }
        /// <inheritdoc />
        public Stack<List<PaintBase>> RedoStack { get; set; }

        #endregion

        public SaveFileCommand()
        {
        }

        #region Command pattern entry

        /// <inheritdoc />
        public void ExecuteUserAction()
        {
            ExecuteUserActionAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public async Task ExecuteUserActionAsync()
        {
            // Create a json array object
            JArray jsonArray = new JArray();
            
            // Use the visitor pattern to create a json array object
            ShapeList.ForEach(paintBase =>
            {
                paintBase.Accept(new WriteFileVisitor(jsonArray));
            });

            // Convert jsonarray object to string
            string shapeListJson = jsonArray.ToString();

            // Get save location from user
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            savePicker.FileTypeChoices.Add("JSON File", new List<string>(){".json"});

            StorageFile file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                // Try to write file to the disk
                CachedFileManager.DeferUpdates(file);
                await FileIO.WriteTextAsync(file, shapeListJson);

                // Give dialog dependent on success
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    ContentDialog dialog = new ContentDialog()
                    {
                        Title = "File save success",
                        Content = "The file was saved successfully",
                        CloseButtonText = "Ok"
                    };

                    await dialog.ShowAsync();
                }
                else
                {
                    ContentDialog dialog = new ContentDialog()
                    {
                        Title = "File save failed",
                        Content = "An error occured while trying to save the file",
                        CloseButtonText = "Close"
                    };

                    await dialog.ShowAsync();
                }
            }
            else
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "File save failed",
                    Content = "An error occured while trying to save the file",
                    CloseButtonText = "Close"
                };

                await dialog.ShowAsync();
            }
        }

        #endregion
    }
}
