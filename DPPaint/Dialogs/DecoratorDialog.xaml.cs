using DPPaint.Decorators;
using System;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DPPaint.Dialogs
{
    /// <summary>
    /// Dialog for editing Dialog
    /// </summary>
    public sealed partial class DecoratorDialog : ContentDialog
    {
        public string Decoration
        {
            get => TextBox.Text;
            set => TextBox.Text = value;
        }

        public DecoratorAnchor Position
        {
            get
            {
                if (Enum.TryParse($"{PositionCombo.SelectedIndex}", out DecoratorAnchor anchor))
                {
                    return anchor;
                }

                return DecoratorAnchor.Top;
            }
            set => PositionCombo.SelectedIndex = (int) value;
        }

        public DecoratorDialog()
        {
            this.InitializeComponent();
        }

        public DecoratorDialog(string decoration, DecoratorAnchor anchor)
        {
            this.InitializeComponent();

            TextBox.Text = decoration;
            PositionCombo.SelectedIndex = (int) anchor;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
