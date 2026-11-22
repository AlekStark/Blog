using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Blog.Controllers;

public class TagController : Controller
{
    private readonly BlogContext _context;

    public TagController(BlogContext context)
    {
        _context = context;
    }

    // GET: Tag
    public IActionResult Index()
    {
        var tags = _context.Tags
            .Include(t => t.ArticleTags).ThenInclude(at => at.Article)
            .ToList();
        return View(tags);
    }

    // GET: Tag/Details/5
    public IActionResult Details(int id)
    {
        var tag = _context.Tags
            .Include(t => t.ArticleTags).ThenInclude(at => at.Article)
            .FirstOrDefault(t => t.Id == id);
        if (tag == null)
        {
            return NotFound();
        }
        return View(tag);
    }

    // GET: Tag/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Tag/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Tag tag)
    {
        if (ModelState.IsValid)
        {
            // Проверка уникальности названия тега
            if (_context.Tags.Any(t => t.Name == tag.Name))
            {
                ModelState.AddModelError("Name", "Тег с таким названием уже существует.");
                return View(tag);
            }
            _context.Tags.Add(tag);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(tag);
    }

    // GET: Tag/Edit/5
    public IActionResult Edit(int id)
    {
        var tag = _context.Tags.FirstOrDefault(t => t.Id == id);
        if (tag == null)
        {
            return NotFound();
        }
        return View(tag);
    }

    // POST: Tag/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Tag tag)
    {
        if (id != tag.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var dbTag = _context.Tags.FirstOrDefault(t => t.Id == id);
            if (dbTag == null)
            {
                return NotFound();
            }
            // Проверка уникальности названия тега
            if (_context.Tags.Any(t => t.Name == tag.Name && t.Id != id))
            {
                ModelState.AddModelError("Name", "Тег с таким названием уже существует.");
                return View(tag);
            }
            dbTag.Name = tag.Name;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(tag);
    }

    // GET: Tag/Delete/5
    public IActionResult Delete(int id)
    {
        var tag = _context.Tags
            .Include(t => t.ArticleTags)
            .FirstOrDefault(t => t.Id == id);
        if (tag == null)
        {
            return NotFound();
        }
        return View(tag);
    }

    // POST: Tag/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var tag = _context.Tags
            .Include(t => t.ArticleTags)
            .FirstOrDefault(t => t.Id == id);
        if (tag != null)
        {
            _context.ArticleTags.RemoveRange(tag.ArticleTags);
            _context.Tags.Remove(tag);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}