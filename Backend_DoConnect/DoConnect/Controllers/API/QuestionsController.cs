using DoConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace DoConnect.Models
{
    [Authorize] //==== Login na korle access korte parbe na ====
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly DoContext _context;
        //========== Iwebhostenvironment create a folder under wwwroot upload so we dont ned write whole path =======
        public readonly IWebHostEnvironment _env;

        public QuestionsController(DoContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromForm] string Questiontitle, [FromForm] string Questiontext, IFormFile? image)
        {
            var UserId = int.Parse(User.FindFirst("UserId")!.Value);
            var question = new Questions
            {
                UserId = UserId,
                QuestionTitle = Questiontitle,
                QuestionText = Questiontext,
                Status = false
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

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
                    QuestionsId = question.QuestionsId
                };

                _context.Images.Add(img);
                await _context.SaveChangesAsync();
            }

           return Ok(new { message = "Question submitted for approval" });
        }
        [HttpGet("all")]
        public IActionResult GetAll()
       => Ok(_context.Questions.Where(q => q.Status).ToList());

        [Authorize(Roles = "Admin")] // Only admin can access pending questions
        [HttpGet("pending")]
        public IActionResult GetPending()
            => Ok(_context.Questions.Where(q => !q.Status).ToList());

        [Authorize(Roles = "Admin")] // Only admin can approve questions
        [HttpPost("approve/{QuestionsId}")]
        public IActionResult Approve(int QuestionsId)
        {
            var q = _context.Questions.Find(QuestionsId);
            if (q == null) return NotFound();
            q.Status = true;
            _context.SaveChanges();
            return Ok("Question approved");
        }

        [HttpDelete("{QuestionsId}")]
        public IActionResult Remove(int QuestionsId)
        {
            var q = _context.Questions.Find(QuestionsId);
            if (q == null) return NotFound();

            // ================ Find related answers and their images for deletion ==============
            var answers = _context.Answers.Where(a => a.QuestionsId == QuestionsId).ToList();
            foreach (var answer in answers)
            {
                // Delete images associated with answers
                var answerImages = _context.Images.Where(i => i.AnswersId == answer.AnswersId).ToList();
                foreach (var img in answerImages)
                {
                    string path = Path.Combine(_env.WebRootPath, img.ImagePath);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);

                    _context.Images.Remove(img);
                }

                _context.Answers.Remove(answer);
            }

            // ================ Find related image for question deletion ==============
            var questionImage = _context.Images.FirstOrDefault(i => i.QuestionsId == q.QuestionsId);
            if (questionImage != null)
            {
                string path = Path.Combine(_env.WebRootPath, questionImage.ImagePath);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                _context.Images.Remove(questionImage);
            }

            _context.Questions.Remove(q);
            _context.SaveChanges();
            return Ok("Question removed");
        }
    }
}
