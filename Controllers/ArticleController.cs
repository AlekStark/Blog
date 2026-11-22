using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Blog.Controllers;

public class ArticleController : Controller
{
    private readonly BlogContext _context;

    public ArticleController(BlogContext context)
    {
        _context = context;
    }

    // GET: Article
    public IActionResult Index()
    {
        var articles = _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
            .ToList();
        return View(articles);
    }

    // GET: Article/GetByAuthorId/5
    public IActionResult GetByAuthorId(int authorId)
    {
        var articles = _context.Articles
            .Where(a => a.AuthorId == authorId)
            .Include(a => a.Author)
            .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
            .ToList();
        return View("Index", articles);
    }

    // GET: Article/Details/5
    public IActionResult Details(int id)
    {
        var article = _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
            .FirstOrDefault(a => a.Id == id);
        if (article == null)
        {
            return NotFound();
        }
        return View(article);
    }

    // GET: Article/Create
    public IActionResult Create()
    {
        ViewBag.Tags = _context.Tags.ToList();
        ViewBag.Users = _context.Users.ToList();
        return View();
    }

    // POST: Article/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Article article, int[] selectedTags)
    {
        if (ModelState.IsValid)
        {
            _context.Articles.Add(article);
            _context.SaveChanges();

            if (selectedTags != null)
            {
                foreach (var tagId in selectedTags)
                {
                    _context.ArticleTags.Add(new ArticleTag { ArticleId = article.Id, TagId = tagId });
                }
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Tags = _context.Tags.ToList();
        ViewBag.Users = _context.Users.ToList();
        return View(article);
    }

    // GET: Article/Edit/5
    public IActionResult Edit(int id)
    {
        var article = _context.Articles
            .Include(a => a.ArticleTags)
            .FirstOrDefault(a => a.Id == id);
        if (article == null)
        {
            return NotFound();
        }
        ViewBag.Tags = _context.Tags.ToList();
        ViewBag.SelectedTags = article.ArticleTags.Select(at => at.TagId).ToList();
        ViewBag.Users = _context.Users.ToList();
        return View(article);
    }

    // POST: Article/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Article article, int[] selectedTags)
    {
        if (id != article.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var dbArticle = _context.Articles
                .Include(a => a.ArticleTags)
                .FirstOrDefault(a => a.Id == id);
            if (dbArticle == null)
            {
                return NotFound();
            }

            dbArticle.Title = article.Title;
            dbArticle.Content = article.Content;
            dbArticle.AuthorId = article.AuthorId;

            // Обновляем связи с тегами
            dbArticle.ArticleTags.Clear();
            if (selectedTags != null)
            {
                foreach (var tagId in selectedTags)
                {
                    dbArticle.ArticleTags.Add(new ArticleTag { ArticleId = dbArticle.Id, TagId = tagId });
                }
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Tags = _context.Tags.ToList();
        ViewBag.SelectedTags = selectedTags?.ToList() ?? new List<int>();
        ViewBag.Users = _context.Users.ToList();
        return View(article);
    }

    // GET: Article/Delete/5
    public IActionResult Delete(int id)
    {
        var article = _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
            .FirstOrDefault(a => a.Id == id);
        if (article == null)
        {
            return NotFound();
        }
        return View(article);
    }

    // POST: Article/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var article = _context.Articles
            .Include(a => a.ArticleTags)
            .FirstOrDefault(a => a.Id == id);
        if (article != null)
        {
            _context.ArticleTags.RemoveRange(article.ArticleTags);
            _context.Articles.Remove(article);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}