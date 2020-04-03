using System;

namespace PictoManagementVocabulary
{
    public class Image
    {
        private string _title;
        private string _path;

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public Image (string NewTitle, string NewPath)
        {
            _title = NewTitle;
            _path = NewPath;
        }
    }
}
