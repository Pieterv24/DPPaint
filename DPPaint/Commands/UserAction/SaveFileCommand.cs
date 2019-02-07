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
    public class SaveFileCommand : IUserActionCommand
    {
        public List<PaintBase> ShapeList { get; set; }
        public Stack<List<PaintBase>> UndoStack { get; set; }
        public Stack<List<PaintBase>> RedoStack { get; set; }

        public SaveFileCommand()
        {
        }

        public void ExecuteUserAction()
        {
            ExecuteUserActionAsync().GetAwaiter().GetResult();
        }

        public async Task ExecuteUserActionAsync()
        {
            JArray jsonArray = new JArray();
            ShapeList.ForEach(paintBase =>
            {
                paintBase.Accept(new WriteFileVisitor(jsonArray));
            });

            string shapeListJson = jsonArray.ToString();

            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            savePicker.FileTypeChoices.Add("JSON File", new List<string>(){".json"});

            StorageFile file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                await FileIO.WriteTextAsync(file, shapeListJson);

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
    }
}
