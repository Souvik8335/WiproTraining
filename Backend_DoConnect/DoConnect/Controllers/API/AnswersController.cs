using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DoConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Model;

namespace DoConnect.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswersController : Controller
    {
         private readonly DoContext _db;
        private readonly IWebHostEnvironment _env;

        public AnswersController(DoContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

[HttpGet("all")]
    public IActionResult GetAll()
    {
        var answers = _db.Answers
            .Include(a => a.User)
            .Include(a => a.Ques)
            .Select(a => new {
                user = a.User!.Username,
                questionId = a.QuestionsId,
                answersId = a.AnswersId,
                question = a.Ques!.QuestionTitle,
                answersText = a.AnswersText,
                status = a.Status
            })
            .ToList();

        return Ok(answers);
    }
[HttpGet("questions")]
    public IActionResult GetQuestions()
    {
        var questions = _db.Questions
            .Where(q => q.Status)
            .Select(q => new { q.QuestionsId, q.QuestionText })
            .ToList();

        return Ok(questions);
    }

[Authorize] // ========== No login no Answer upload
[HttpPost("add")]
public async Task<IActionResult> Add([FromForm] string text, [FromForm] int Questionid, IFormFile? image)
{
    var userIdClaim = User.FindFirstValue("UserId");  // safer claim extraction
    if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
    {
        return Unauthorized("User not logged in or invalid token");
    }

    // Check if user exists in database
    var user = await _db.Users.FindAsync(userId);
    if (user == null)
    {
        return Unauthorized("User not found");
    }

    Console.WriteLine("Logged in UserId = " + userId);

    var ans = new Answers
    {
        QuestionsId = Questionid,
        AnswersText = text,
        UserId = userId,
        Status = false
    };

    _db.Answers.Add(ans);
    await _db.SaveChangesAsync(); 

    if (image != null && image.Length > 0)
    {
        string uploads = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

        string fileName = Guid.NewGuid().ToString() + "_" + image.FileName;
        string filePath = Path.Combine(uploads, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
            await image.CopyToAsync(stream);

        var img = new Images
        {
            ImagePath = "uploads/" + fileName,
            AnswersId = ans.AnswersId  //---------  Image upload jeta hobe ei id te store hobe
        };

        _db.Images.Add(img);
        await _db.SaveChangesAsync();
    }

      // Load navigation properties
      ans = await _db.Answers
          .Include(a => a.User)
          .Include(a => a.Ques)
          .FirstOrDefaultAsync(a => a.AnswersId == ans.AnswersId);

      return Ok(new {
          answersId = ans.AnswersId,
          answersText = ans.AnswersText,
          status = ans.Status,
          user = ans.User?.Username,
          question = ans.Ques?.QuestionTitle,
          questionId = ans.QuestionsId
      });
}

 [HttpPut("{AnswersId}")]
 public async Task<IActionResult> Update(int AnswersId, Answers answers)
 {
     if (AnswersId != answers.AnswersId) return BadRequest();

     _db.Entry(answers).State = EntityState.Modified;
     await _db.SaveChangesAsync();
     return NoContent();
 }

 [HttpDelete("{AnswersId}")]
 public async Task<IActionResult> Delete(int AnswersId)
 {
     var answers = await _db.Answers.FindAsync(AnswersId);
     if (answers == null) return NotFound();

     _db.Answers.Remove(answers);
     await _db.SaveChangesAsync();
     return NoContent();
 }

 [Authorize(Roles = "Admin")] // Only admin can access pending answers
 [HttpGet("pending")]
 public IActionResult GetPending()
     => Ok(_db.Answers.Where(a => !a.Status).Include(a => a.User).Include(a => a.Ques).ToList());

 [Authorize(Roles = "Admin")] // Only admin can approve answers
 [HttpPost("approve/{AnswersId}")]
 public IActionResult Approve(int AnswersId)
 {
     var a = _db.Answers.Find(AnswersId);
     if (a == null) return NotFound();
     a.Status = true;
     _db.SaveChanges();
     return Ok("Answer approved");
 }
    }
}
