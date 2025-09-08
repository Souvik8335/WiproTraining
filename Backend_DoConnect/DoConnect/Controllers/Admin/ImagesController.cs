using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoConnect;
using Model;

namespace DoConnect.Controllers_Admin
{
    public class ImagesController : Controller
    {
        private readonly DoContext _context;

        public ImagesController(DoContext context)
        {
            _context = context;
        }

        // GET: Images
        public async Task<IActionResult> Index()
        {
            var doContext = _context.Images.Include(i => i.Answer).Include(i => i.Question);
            return View(await doContext.ToListAsync());
        }

        // GET: Images/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var images = await _context.Images
                .Include(i => i.Answer)
                .Include(i => i.Question)
                .FirstOrDefaultAsync(m => m.ImagesId == id);
            if (images == null)
            {
                return NotFound();
            }

            return View(images);
        }

        // GET: Images/Create
        public IActionResult Create()
        {
            ViewData["AnswersId"] = new SelectList(_context.Answers, "AnswersId", "AnswersId");
            ViewData["QuestionsId"] = new SelectList(_context.Questions, "QuestionsId", "QuestionsId");
            return View();
        }

        // POST: Images/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ImagesId,ImagePath,QuestionsId,AnswersId")] Images images)
        {
            if (ModelState.IsValid)
            {
                _context.Add(images);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnswersId"] = new SelectList(_context.Answers, "AnswersId", "AnswersId", images.AnswersId);
            ViewData["QuestionsId"] = new SelectList(_context.Questions, "QuestionsId", "QuestionsId", images.QuestionsId);
            return View(images);
        }

        // GET: Images/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var images = await _context.Images.FindAsync(id);
            if (images == null)
            {
                return NotFound();
            }
            ViewData["AnswersId"] = new SelectList(_context.Answers, "AnswersId", "AnswersId", images.AnswersId);
            ViewData["QuestionsId"] = new SelectList(_context.Questions, "QuestionsId", "QuestionsId", images.QuestionsId);
            return View(images);
        }

        // POST: Images/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ImagesId,ImagePath,QuestionsId,AnswersId")] Images images)
        {
            if (id != images.ImagesId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(images);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImagesExists(images.ImagesId))
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
            ViewData["AnswersId"] = new SelectList(_context.Answers, "AnswersId", "AnswersId", images.AnswersId);
            ViewData["QuestionsId"] = new SelectList(_context.Questions, "QuestionsId", "QuestionsId", images.QuestionsId);
            return View(images);
        }

        // GET: Images/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var images = await _context.Images
                .Include(i => i.Answer)
                .Include(i => i.Question)
                .FirstOrDefaultAsync(m => m.ImagesId == id);
            if (images == null)
            {
                return NotFound();
            }

            return View(images);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var images = await _context.Images.FindAsync(id);
            if (images != null)
            {
                _context.Images.Remove(images);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImagesExists(int id)
        {
            return _context.Images.Any(e => e.ImagesId == id);
        }
    }
}
