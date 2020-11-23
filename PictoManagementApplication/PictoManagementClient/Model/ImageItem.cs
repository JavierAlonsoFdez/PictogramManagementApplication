using System.Windows.Controls;

namespace PictoManagementClient.Model
{
    public class ImageItem
    {
        public string Title { get; set; }
        public string Image { get; set; }
        public CheckBox IsIncluded { get; set; }

        public ImageItem(string _title, string _image, bool _include)
        {
            Title = _title;
            Image = _image;
            this.IsIncluded = new CheckBox
            {
                IsChecked = _include,
                Content = "Incluir: " + _title
            };
        }
    }
}
