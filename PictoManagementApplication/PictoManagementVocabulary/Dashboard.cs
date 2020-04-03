using System;
using System.Collections.Generic;
using System.Text;

namespace PictoManagementVocabulary
{
    [Serializable()]
    public class Dashboard
    {
        private string _title;
        private Image[] _images;

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public Image[] Images
        {
            get { return _images; }
            set { _images = value; }
        }

        public Dashboard (string NewTitle, Image[] NewImages)
        {
            _title = NewTitle;
            _images = NewImages;
        }
    }
}
