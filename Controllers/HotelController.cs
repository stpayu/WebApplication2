﻿using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Controllers
{
    public class HotelController : Controller
    {
        private readonly AppDbContext _context;

        public HotelController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var hotelBookings = _context.Hotels.ToList();
            return View(hotelBookings);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Hotel booking)
        {
            if (ModelState.IsValid)
            {
                _context.Hotels.Add(booking);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
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
        public IActionResult Edit(int id)
        {
            var hotel = _context.Hotels.Find(id);
            if (hotel == null)
            {
                return NotFound();
            }
            return View(hotel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Hotel hotel)
        {
            if (id != hotel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Hotels.Update(hotel);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelExists(hotel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(hotel);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var hotel = _context.Hotels.FirstOrDefault(p => p.Id == id);
            if (hotel == null)
            {
                return NotFound();
            }
            return View(hotel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var hotel = _context.Hotels.Find(id);
            if (hotel != null)
            {
                _context.Hotels.Remove(hotel);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Search(string hotelName, string location)
        {
            var hotelQuery = from p in _context.Rentals
                               select p;

            bool searchPerformed = !String.IsNullOrEmpty(hotelName)
                                || !String.IsNullOrEmpty(location);


            if (searchPerformed)
            {
                if (!String.IsNullOrEmpty(hotelName))
                {
                    hotelQuery = hotelQuery.Where(p => p.ModelName.Contains(hotelName));
                }

                if (!String.IsNullOrEmpty(location))
                {
                    hotelQuery = hotelQuery.Where(p => p.RentalProvider.Contains(location));
                }


            }
            var hotel = await hotelQuery.ToListAsync();
            ViewData["SearchedPerformed"] = searchPerformed;
            return View("Index", hotel);
        }

        public IActionResult Book(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = _context.Hotels.FirstOrDefault(m => m.Id == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }


        [HttpPost]
        public IActionResult Book(int id, [Bind("Id,Name")] HotelBooking hotelBooking)
        {
            if (ModelState.IsValid)
            {
                // Save booking to the database
                _context.HotelBookings.Add(hotelBooking);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(hotelBooking);
        }



        private bool HotelExists(int id)
        {
            return _context.Rentals.Any(p => p.Id == id);
        }
    }
}
