﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RelationsNaN.Data;
using RelationsNaN.Models;

namespace RelationsNaN.Controllers
{
    public class GamesController : Controller
    {
        private readonly RelationsNaNContext _context;

        public GamesController(RelationsNaNContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult RemovePlatform(int id, int platformId)
        {
            var game = _context.Game.Include(g => g.Platforms).FirstOrDefault(g => g.Id == id);
            if (game != null)
            {
                var platform = game.Platforms.FirstOrDefault(p => p.Id == platformId);
                if (platform != null)
                {
                    game.Platforms.Remove(platform);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Edit", new { id });
        }

        [HttpPost]
        public IActionResult AddPlatform(int id, int platformId)
        {
            var game = _context.Game.Include(g => g.Platforms).FirstOrDefault(g => g.Id == id);
            var platform = _context.Platform.FirstOrDefault(p => p.Id == platformId);
            if (game != null && platform != null && !game.Platforms.Contains(platform))
            {
                game.Platforms.Add(platform);
                _context.SaveChanges();
            }
            return RedirectToAction("Edit", new { id });
        }
        // GET: Games
        public async Task<IActionResult> Index()
        {
            var relationsNaNContext = _context.Game.Include(g => g.Genre).Include(g => g.Platforms);
            return View(await relationsNaNContext.ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name");
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Image,ReleaseYear,GenreId")] Game game)
        {
            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", game.GenreId);
            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game.Include(g => g.Platforms).FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
            {
                return NotFound();
            }
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", game.GenreId);
            ViewData["platformId"] = new SelectList(_context.Platform, "Id", "Name", game.PlatformsId);
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Image,ReleaseYear,GenreId")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", game.GenreId);
            ViewData["platformId"] = new SelectList(_context.Platform, "Id", "Name", game.PlatformsId);
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Game.FindAsync(id);
            if (game != null)
            {
                _context.Game.Remove(game);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _context.Game.Any(e => e.Id == id);
        }

    }
}
