using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Blog.Controllers;

public class CommentController : Controller
{
    private readonly BlogContext _context;

    public CommentController(BlogContext context)
    {
        _context = context;
    }

    // GET: Comment
    public IActionResult Index()
    {
        var comments = _context.Comments
            .Include(c => c.Author)
            .Include(c => c.Article)
            .ToList();
        return View(comments);
    }

    // GET: Comment/Details/5
    public IActionResult Details(int id)
    {
        var comment = _context.Comments
            .Include(c => c.Author)
            .Include(c => c.Article)
            .FirstOrDefault(c => c.Id == id);
        if (comment == null)
        {
            return NotFound();
        }
        return View(comment);
    }

    // GET: Comment/Create
    public IActionResult Create()
    {
        ViewBag.Articles = _context.Articles.ToList();
        ViewBag.Users = _context.Users.ToList();
        return View();
    }

    // POST: Comment/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Comment comment)
    {
        if (ModelState.IsValid)
        {
            _context.Comments.Add(comment);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Articles = _context.Articles.ToList();
        ViewBag.Users = _context.Users.ToList();
        return View(comment);
    }

    // GET: Comment/Edit/5
    public IActionResult Edit(int id)
    {
        var comment = _context.Comments.FirstOrDefault(c => c.Id == id);
        if (comment == null)
        {
            return NotFound();
        }
        ViewBag.Articles = _context.Articles.ToList();
        ViewBag.Users = _context.Users.ToList();
        return View(comment);
    }

    // POST: Comment/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Comment comment)
    {
        if (id != comment.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var dbComment = _context.Comments.FirstOrDefault(c => c.Id == id);
            if (dbComment == null)
            {
                return NotFound();
            }
            dbComment.Content = comment.Content;
            dbComment.AuthorId = comment.AuthorId;
            dbComment.ArticleId = comment.ArticleId;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Articles = _context.Articles.ToList();
        ViewBag.Users = _context.Users.ToList();
        return View(comment);
    }

    // GET: Comment/Delete/5
    public IActionResult Delete(int id)
    {
        var comment = _context.Comments
            .Include(c => c.Author)
            .Include(c => c.Article)
            .FirstOrDefault(c => c.Id == id);
        if (comment == null)
        {
            return NotFound();
        }
        return View(comment);
    }

    // POST: Comment/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var comment = _context.Comments.FirstOrDefault(c => c.Id == id);
        if (comment != null)
        {
            _context.Comments.Remove(comment);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}