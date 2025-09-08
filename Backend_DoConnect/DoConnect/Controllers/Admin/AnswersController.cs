using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoConnect;
using Model;
using System.Security.Claims;

namespace DoConnect.Controllers_Admin
{
    public class AnswersController : Controller
    {
        private readonly DoContext _context;

        public AnswersController(DoContext context)
        {
            _context = context;
        }

        // GET: Answers
        public async Task<IActionResult> Index()
        {
            var doContext = _context.Answers.Include(a => a.Ques).Include(a => a.User);
            return View(await doContext.ToListAsync());
        }

        // GET: Answers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var answers = await _context.Answers
                .Include(a => a.Ques)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AnswersId == id);
            if (answers == null)
            {
                return NotFound();
            }

            return View(answers);
        }

        // GET: Answers/Create
        public IActionResult Create()
        {
            ViewData["QuestionsId"] = new SelectList(_context.Questions, "QuestionsId", "QuestionsId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Answers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AnswersId,QuestionsId,Status")] Answers answers)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            answers.UserId = userId;

            if (ModelState.IsValid)
            {
                _context.Add(answers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["QuestionsId"] = new SelectList(_context.Questions, "QuestionsId", "QuestionsId", answers.QuestionsId);
            return View(answers);
        }

        // GET: Answers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var answers = await _context.Answers.FindAsync(id);
            if (answers == null)
            {
                return NotFound();
            }
            ViewData["QuestionsId"] = new SelectList(_context.Questions, "QuestionsId", "QuestionsId", answers.QuestionsId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", answers.UserId);
            return View(answers);
        }

        // POST: Answers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AnswersId,QuestionsId,UserId,AnswersText,Status")] Answers answers)
        {
            if (id != answers.AnswersId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(answers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnswersExists(answers.AnswersId))
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
            ViewData["QuestionsId"] = new SelectList(_context.Questions, "QuestionsId", "QuestionsId", answers.QuestionsId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", answers.UserId);
            return View(answers);
        }

        // POST: Answers/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var answers = await _context.Answers.FindAsync(id);
            if (answers == null)
            {
                return NotFound();
            }

            answers.Status = true; // Approve the answer
            _context.Update(answers);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Answers/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var answers = await _context.Answers.FindAsync(id);
            if (answers == null)
            {
                return NotFound();
            }

            answers.Status = false; // Keep as rejected/pending
            _context.Update(answers);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Answers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var answers = await _context.Answers
                .Include(a => a.Ques)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AnswersId == id);
            if (answers == null)
            {
                return NotFound();
            }

            return View(answers);
        }

        // POST: Answers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var answers = await _context.Answers.FindAsync(id);
            if (answers != null)
            {
                _context.Answers.Remove(answers);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnswersExists(int id)
        {
            return _context.Answers.Any(e => e.AnswersId == id);
        }
    }
}
