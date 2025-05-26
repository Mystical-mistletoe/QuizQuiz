using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizQuiz
{
    public class Question
    {
        public string Text;             // Текст вопроса
        public List<string> Answers;    // Варианты ответов
        public int CorrectIndex;        // Номер правильного ответа
        public QuestionType Type;       // Тип вопроса

        public Question(string text, List<string> answers, int correctIndex, QuestionType type)
        {
            Text = text;
            Answers = answers;
            CorrectIndex = correctIndex;
            Type = type;
        }

    }
}
