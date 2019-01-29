using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using DPPaint.Shapes;
using Newtonsoft.Json;

namespace DPPaint.Commands.UserAction
{
    public class OpenFileCommand : IUserActionCommand
    {
        public List<PaintBase> ShapeList { get; set; }

        private ICanvasPage _page;

        public OpenFileCommand(ICanvasPage page)
        {
            _page = page;
        }

        public void ExecuteUserAction()
        {
            ExecuteUserActionAsync().GetAwaiter().GetResult();
        }

        public async Task ExecuteUserActionAsync()
        {
            var openPicker = new FileOpenPicker();

            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".json");
            openPicker.ViewMode = PickerViewMode.List;

            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                string jsonString = await FileIO.ReadTextAsync(file);

                List<PaintBase> deserialized = JsonConvert.DeserializeObject<List<PaintBase>>(jsonString, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });


                // Clear undo, redo and master list
                _page.ClearMemory();
                // Add deserialized master list to main page
                ShapeList.AddRange(deserialized);

                // Send draw and update list commands to main page
                _page.Draw();
                _page.UpdateList();
            }
        }
    }
}
