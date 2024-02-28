﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class RentalController : Controller
    {
        private readonly AppDbContext _context;

        public RentalController (AppDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var carrent = _context.Rentals.FirstOrDefault(p => p.Id == id);
            if (carrent == null)
            {
                return NotFound();
            }
            return View(carrent);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Rental rental)
        {
            if (ModelState.IsValid)
            {
                _context.Rentals.Add(rental);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rental);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var rental = _context.Rentals.Find(id);
            if (rental == null)
            {
                return NotFound();
            }
            return View(rental);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Rental rental)
        {
            if (id != rental.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Rentals.Update(rental);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalExists(rental.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(rental);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var rental = _context.Rentals.FirstOrDefault(p => p.Id == id);
            if (rental == null)
            {
                return NotFound();
            }
            return View(rental);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var rental = _context.Rentals.Find(id);
            if (rental != null)
            {
                _context.Rentals.Remove(rental);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Search(string modelName, string renralProvider)
        {
            var rentalQuery = from p in _context.Rentals
                               select p;

            bool searchPerformed = !String.IsNullOrEmpty(modelName)
                                || !String.IsNullOrEmpty(renralProvider);


            if (searchPerformed)
            {
                if (!String.IsNullOrEmpty(modelName))
                {
                    rentalQuery = rentalQuery.Where(p => p.ModelName.Contains(modelName));
                }

                if (!String.IsNullOrEmpty(renralProvider))
                {
                    rentalQuery = rentalQuery.Where(p => p.RentalProvider.Contains(renralProvider));
                }


            }
            var rental = await rentalQuery.ToListAsync();
            ViewData["SearchedPerformed"] = searchPerformed;
            return View("Index", rental);
        }

        public IActionResult Book(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = _context.Rentals.FirstOrDefault(m => m.Id == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }


        [HttpPost]
        public IActionResult Book(int id, [Bind("Id,Name")] RentalBooking rentalBooking)
        {


            if (ModelState.IsValid)
            {
                // Save booking to the database
                _context.RentalBookings.Add(rentalBooking);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(rentalBooking);
        }



        private bool RentalExists(int id)
        {
            return _context.Rentals.Any(p => p.Id == id);
        }
    }
}
