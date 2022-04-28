using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OnMangeOu.Lib.Models
{
    public class Restaurant : IComparable<Restaurant>
    {
        public Restaurant() { }

        public Restaurant(String date, string nom, string parLaFauteDe, string lieu, string type, IList<Note> notes, string moyenne, string commentaire)
        {
            Date = date;
            Nom = nom;
            ParLaFauteDe = parLaFauteDe;
            Lieu = lieu;
            Type = type;
            Notes = notes;
            Moyenne = moyenne;
            Commentaire = commentaire;
            Nb = 0;
        }

        public String Date { get; set; }
        public String Nom { get; set; }
        [Display(Name = "Proposé par")]
        public String ParLaFauteDe { get; set; }
        public String Lieu { get; set; }
        public String Type { get; set; }
        public IList<Note> Notes { get; set; }
        public String Moyenne { get; set; }
        public String Commentaire { get; set; }
        [Display(Name = "Sélection")]
        public int Nb { get; set; }
        public String Pourcentage { get; set; }

        public void CalculPourcentage(int iterations)
        {
            var value = (double)(Nb * 100) / iterations;
            var percentage = Math.Round(value, 2);
            Pourcentage = $"{percentage}%";
        }

        public int CompareTo(Restaurant obj)
        {
            return Nb.CompareTo(obj.Nb);
        }
    }
}
