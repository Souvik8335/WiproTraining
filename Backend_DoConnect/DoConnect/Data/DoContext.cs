using Microsoft.EntityFrameworkCore;
using Model;
using System;

namespace DoConnect
{
     public class DoContext : DbContext
 {                                 //Tables create hobe sob class er name e User,Answer,Question,Images
     public DoContext(DbContextOptions<DoContext> options) : base(options)
     {

     }
     public DbSet<User> Users{ get; set; }
     public DbSet<Questions> Questions{ get; set; }
     public DbSet<Answers> Answers{ get; set; }
     public DbSet<Images> Images{ get; set; }
 }
}