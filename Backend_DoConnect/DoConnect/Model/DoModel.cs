using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace Model   //
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User";
    }

    public class Questions
    {
        [Key]
        public int QuestionsId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionText { get; set; }
        public bool Status { get; set; } 
    }
    public class Answers
    {
        [Key]
        public int AnswersId { get; set; }
        [ForeignKey("Ques")]
        public int QuestionsId { get; set; }
        public Questions? Ques { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public string AnswersText{ get; set; }
        public User? User { get; set; }
        public bool Status { get; set; }
    }
    public class Images
    {
        [Key]
        public int ImagesId { get; set; }
        public string ImagePath { get; set; }
        [ForeignKey("Question")]
        public int? QuestionsId { get; set; }
        public Questions? Question { get; set; }
        [ForeignKey("Answer")]
        public int? AnswersId { get; set; }
        public Answers? Answer { get; set; }

    }
}
