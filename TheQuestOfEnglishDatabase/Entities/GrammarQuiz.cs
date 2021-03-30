﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TheEnglishQuestDatabase.Entities;

namespace TheEnglishQuestDatabase
{
    public class GrammarQuiz
    {
        [Key]
        public int Id{ get; set; }
        [Required]
        public string Name { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        [NotMapped]
        public IEnumerable<GrammarTask> GrammarTasks { get; set; }

    }
}