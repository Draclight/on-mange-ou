using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnMangeOu.Lib.Models
{
    public class Note
    {
        public Note(string qui, string avis)
        {
            Qui = qui;
            Avis = avis;
        }
        public String Qui { get; set; }
        public String Avis { get; set; }
    }
}
