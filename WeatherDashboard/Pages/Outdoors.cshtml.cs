using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WeatherDashboard.Data;

namespace WeatherDashboard.Pages
{
    public class OutdoorsModel : PageModel
    {
        private readonly EFContext _context;

        public IList<Models.WeatherData> OutdoorData { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime AverageTempratureDay { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool AverageTempratureDayVisible { get; set; }

        public IList<(DateTime Date, double Temperature)> AverageTemperatures { get; set; }
        public IList<(DateTime Date, double Humidity)> AverageHumidity { get; set; }

        public IList<(DateTime Date, double MoldRisk)> MoldRisk { get; set; }

        public OutdoorsModel(EFContext context)
        {
            _context = context;
            AverageTemperatures = new List<(DateTime, double)>();
            AverageHumidity = new List<(DateTime, double)>();
            MoldRisk = new List<(DateTime, double)>();

            AverageTempratureDayVisible = false;
        }

        public async Task OnGetAsync()
        {
            OutdoorData = await _context.WeatherData
                .Where(wd => wd.Place == "Ute")
                .ToListAsync();

            foreach (var group in OutdoorData.GroupBy(g => g.Date.Date))
            {
                double avgTemperature = Math.Round(group.Average(g => g.Temperature), 2);
                double avgHumidity = Math.Round(group.Average(g => g.Humidity), 2);
                double moldRisk = Math.Round((((avgHumidity - 78) * (avgTemperature / 15)) / 0.22), 2);
                moldRisk = moldRisk > 0 ? moldRisk : 0;
                AverageTemperatures.Add((group.Key, avgTemperature));
                AverageHumidity.Add((group.Key, avgHumidity));
                MoldRisk.Add((group.Key, moldRisk));
            }

            AverageTemperatures = AverageTemperatures.OrderByDescending(at => at.Temperature).ToList();
            AverageHumidity = AverageHumidity.OrderByDescending(ah => ah.Humidity).ToList();
            MoldRisk = MoldRisk.OrderBy(mr => mr.MoldRisk).ToList();
        }


        public IActionResult OnPost()
        {
            var x = AverageTempratureDay;

            AverageTempratureDayVisible = true;

            return RedirectToPage(new { AverageTempratureDay, AverageTempratureDayVisible });
        }
    }
}