﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TempratureDashboard.Data;
using TempratureDashboard.Models;

namespace TempratureDashboard.Pages.WeatherData
{
    public class DeleteModel : PageModel
    {
        private readonly TempratureDashboard.Data.DBContext _context;

        public DeleteModel(TempratureDashboard.Data.DBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TempratureDashboard.Models.WeatherData WeatherData { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            WeatherData = await _context.WeatherData.FirstOrDefaultAsync(m => m.ID == id);

            if (WeatherData == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            WeatherData = await _context.WeatherData.FindAsync(id);

            if (WeatherData != null)
            {
                _context.WeatherData.Remove(WeatherData);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}