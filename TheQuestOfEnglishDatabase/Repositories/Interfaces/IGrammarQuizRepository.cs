﻿using System.Threading.Tasks;

namespace TheEnglishQuestDatabase
{
    public interface IGrammarQuizRepository
    {
        Task<bool> AddNewQuiz(GrammarQuiz quiz);
        Task<bool> RemoveQuiz(GrammarQuiz quiz);
        Task<bool> FindQuiz(int id);
        
    }
}