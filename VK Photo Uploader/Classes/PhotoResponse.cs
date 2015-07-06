using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VK_Photo_Uploader.Classes
{
    public class PhotoResponse
    {
        public long Server { get; set; }
        public string Photo { get; set; }
        public string Hash { get; set; }
    }
}
