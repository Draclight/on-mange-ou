using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnMangeOu.Lib;
using OnMangeOu.Lib.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace OnMangeOu.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string ApplicationName = "on-mange-ou";
        private static readonly string SpreadsheetId = "16NyeLErGgH1F_PV9fSlBDkpjYfLIXPQ33CUlMLq37Tw";
        private static readonly string sheet = "sheet";
        private static SheetsService service;
        private static int iteration = 0;
        GoogleCredential credential;

        [BindProperty]
        [Display(Name = "Nombre d'itérations :")]
        public int Iteration { get => iteration; }

        [BindProperty]
        public IList<Restaurant> Restaurants { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;

            using (var stream = new FileStream("on-mange-ou-331619-451bdabb145b.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }

            service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
         
        }

        public void OnGet(IList<Restaurant> restaurants = null)
        {
            if (restaurants.Equals(null))
                Restaurants = new List<Restaurant>();
            else
                Restaurants = restaurants;

        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage();
            }

            Restaurants = GetRestaurants();

            iteration = Restaurants.Count * 1000;

            if (Restaurants.Count > 0)
            {
                for (int i = 0; i < iteration; i++)
                {
                    ChoixRestaurant(Restaurants);
                }

                Restaurants = Restaurants.OrderByDescending(r => r.Nb).ToList();
            }

            return Page();
        }

        private void ChoixRestaurant(IList<Restaurant> restaurants)
        {
            Random rdn = new Random();
            Restaurant rest = restaurants.ElementAt(rdn.Next(0, restaurants.Count));
            rest.Nb++;
            rest.CalculPourcentage(iteration);
        }

        private IList<IList<object>> GetValuesByRange(string range)
        {
            IList<IList<object>> ret = null;
            try
            {
                var request = service.Spreadsheets.Values.Get(SpreadsheetId, range);
                var response = request.Execute();
                if (response.Values != null && response.Values.Count > 0)
                {
                    ret = response.Values;
                }
                else
                {
                    new Exception("No data found");
                }
            }
            catch (Exception)
            {
                throw;
            }

            return ret;
        }

        public static string GetSheetUrl(string range)
        {
            string ret = null;
            try
            {
                var request = service.Spreadsheets.Get(SpreadsheetId);
                request.Ranges = range;
                request.IncludeGridData = false;

                // To execute asynchronously in an async method, replace `request.Execute()` as shown:
                var response = request.Execute();

                if (response.SpreadsheetUrl != null)
                {
                    ret = response.SpreadsheetUrl;
                }
                else
                {
                    new Exception("No data found");
                }
            }
            catch (Exception)
            {
                throw;
            }

            return ret;
        }

        private IList<Restaurant> GetRestaurants()
        {
            var range = $"{sheet}!A4:99";
            IList<Restaurant> ret = new List<Restaurant>();
            try
            {
                var lignes = GetValuesByRange(range);
                if (lignes != null && lignes.Count > 0)
                {
                    foreach (var ligneResto in lignes)
                    {
                        String Date = ligneResto[0] as string;
                        String Nom = ligneResto[1] as string;
                        String ParLaFauteDe = ligneResto[2] as string;
                        String Lieu = ligneResto[3] as string;
                        String Type = ligneResto[4] as string;
                        IList<Note> Notes = GetNotes(ligneResto);
                        String Moyenne = ligneResto[13] as string;
                        String Commentaire = ligneResto.Count > 14 ? ligneResto[14] as string : string.Empty;
                        Restaurant restaurant = new Restaurant(Date, Nom, ParLaFauteDe, Lieu, Type, Notes, Moyenne, Commentaire);
                        ret.Add(restaurant);
                    }
                }
                else
                {
                    new Exception("No data found");
                }
            }
            catch (Exception)
            {
                throw;
            }

            return ret;
        }

        private IList<Note> GetNotes(IList<object> ligneResto)
        {
            IList<Note> ret = new List<Note>();
            var testeurs = GetValuesByRange($"{sheet}!F3:M3")[0];
            int cptTesteurs = 0;
            for (int i = 5; i < ligneResto.Count - 2; i++)
            {
                ret.Add(new Note(testeurs[cptTesteurs].ToString(), ligneResto[i] as string));
                cptTesteurs++;
            }

            return ret;
        }

        private void PrintError(Exception ex)
        {
            _logger.LogError($"Erreur {DateTime.Now} : \r\nStackTrace - {ex.StackTrace}\r\nMessage - {ex.Message}");
        }
    }
}
