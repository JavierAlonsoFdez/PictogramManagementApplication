using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictoManagementGUI.Model
{
    public class Image
    {
        public int ID { get; set; }
        [Display(Name = "Título")]
        public string title { get; set; }

    }
}
