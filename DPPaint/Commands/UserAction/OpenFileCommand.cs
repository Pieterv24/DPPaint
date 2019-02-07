using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using DPPaint.Shapes;
using DPPaint.Strategy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DPPaint.Commands.UserAction
{
    public class OpenFileCommand : IUserActionCommand
    {
        public List<PaintBase> ShapeList { get; set; }
        public Stack<List<PaintBase>> UndoStack { get; set; }
        public Stack<List<PaintBase>> RedoStack { get; set; }

        private readonly ICanvasPage _page;

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

                //List<PaintBase> deserialized = JsonConvert.DeserializeObject<List<PaintBase>>(jsonString, new JsonSerializerSettings()
                //{
                //    TypeNameHandling = TypeNameHandling.Auto
                //});
                List<PaintBase> newShapeList = DeserializeJsonSave(jsonString);

                // Clear undo, redo and master list
                UndoStack.Clear();
                RedoStack.Clear();
                ShapeList.Clear();
                // Add deserialized master list to main page
                ShapeList.AddRange(newShapeList);

                // Send draw and update list commands to main page
                _page.Draw();
                _page.UpdateList();
            }
        }

        private List<PaintBase> DeserializeJsonSave(string jsonSaveString)
        {
            List<PaintBase> newShapeList = new List<PaintBase>();

            JArray jArray = JArray.Parse(jsonSaveString);
            if (jArray != null)
            {
                foreach (JToken jToken in jArray)
                {
                    if (jToken is JObject jObject)
                    {
                        if (Enum.TryParse(jObject.GetValue("type").ToString(), out PaintType type))
                        {
                            PaintBaseProperties deserializedProperties = GetBaseProperties(jObject);

                            if (deserializedProperties != null)
                            {
                                if (type == PaintType.Shape)
                                {
                                    PaintShape shape = GetPaintShape(jObject, deserializedProperties);
                                    if (shape != null)
                                    {
                                        newShapeList.Add(shape);
                                    }
                                }
                                else if (type == PaintType.Group)
                                {
                                    PaintGroup group = GetPaintGroup(jObject, deserializedProperties);
                                    if (group != null)
                                    {
                                        newShapeList.Add(group);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return newShapeList;
        }

        private PaintBaseProperties GetBaseProperties(JObject jObject)
        {
            bool overalCompletion = true;

            DecoratorAnchor anchor = DecoratorAnchor.Top;
            double width = 0;
            double height = 0;
            double x = 0;
            double y = 0;

            overalCompletion = overalCompletion && Enum.TryParse(jObject.GetValue("anchor").ToString(), out anchor);
            string decoration = jObject.GetValue("decoration").ToString();
            overalCompletion = overalCompletion && double.TryParse(jObject.GetValue("width").ToString(), out width);
            overalCompletion = overalCompletion && double.TryParse(jObject.GetValue("height").ToString(), out height);
            overalCompletion = overalCompletion && double.TryParse(jObject.GetValue("x").ToString(), out x);
            overalCompletion = overalCompletion && double.TryParse(jObject.GetValue("y").ToString(), out y);

            if (overalCompletion)
            {
                return new PaintBaseProperties
                {
                    Anchor = anchor,
                    Decoration = decoration,
                    Width = width,
                    Height = height,
                    X = x,
                    Y = y
                };
            }

            return null;
        }

        private PaintShape GetPaintShape(JObject jObject, PaintBaseProperties baseProps)
        {
            if (Enum.TryParse(jObject.GetValue("shapeType").ToString(), out ShapeType type))
            {
                IShapeBase shape = null;
                if (type == ShapeType.Circle) shape = CircleShape.Instance;
                if (type == ShapeType.Rectangle) shape = RectangleShape.Instance;

                return new PaintShape(shape)
                {
                    Anchor = baseProps.Anchor,
                    Decoration = baseProps.Decoration,
                    Height = baseProps.Height,
                    Width = baseProps.Width,
                    X = baseProps.X,
                    Y = baseProps.Y
                };
            }

            return null;
        }

        private PaintGroup GetPaintGroup(JObject jObject, PaintBaseProperties baseProps)
        {
            PaintGroup group = new PaintGroup
            {
                Anchor = baseProps.Anchor,
                Decoration = baseProps.Decoration,
                Height = baseProps.Height,
                Width = baseProps.Width,
                X = baseProps.X,
                Y = baseProps.Y
            };

            if (jObject.GetValue("children") is JArray children)
            {
                foreach (JToken child in children)
                {
                    if (child is JObject jChild &&
                        Enum.TryParse(jChild.GetValue("type").ToString(), out PaintType type))
                    {
                        PaintBaseProperties deserBase = GetBaseProperties(jChild);

                        if (deserBase != null)
                        {
                            if (type == PaintType.Shape)
                            {
                                PaintShape shape = GetPaintShape(jChild, deserBase);
                                if (shape != null)
                                {
                                    group.Add(shape);
                                }
                            }
                            else if (type == PaintType.Group)
                            {
                                PaintGroup innerGroup = GetPaintGroup(jChild, deserBase);
                                if (innerGroup != null)
                                {
                                    group.Add(innerGroup);
                                }
                            }
                        }
                    }
                }
            }

            return group;
        }
    }
}
