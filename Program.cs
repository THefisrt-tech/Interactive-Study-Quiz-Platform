using System;
using System.Collections.Generic;
using System.Linq;

namespace InteractiveStudyPlatform
{
    // Object-Oriented Domain Models
    public class Question
    {
        public string Prompt { get; set; }
        public List<string> Options { get; set; }
        public int CorrectOptionIndex { get; set; }

        public Question(string prompt, List<string> options, int correctOptionIndex)
        {
            Prompt = prompt;
            Options = options;
            CorrectOptionIndex = correctOptionIndex;
        }

        public bool ValidateAnswer(int userChoice)
        {
            return userChoice == CorrectOptionIndex;
        }
    }

    public class Quiz
    {
        public string Title { get; set; }
        public string Subject { get; set; }
        public List<Question> Questions { get; set; }

        public Quiz(string title, string subject)
        {
            Title = title;
            Subject = subject;
            Questions = new List<Question>();
        }

        public void AddQuestion(Question question)
        {
            Questions.Add(question);
        }
    }

    public class UserProfile
    {
        public string StudentName { get; set; }
        public int TotalQuizzesTaken { get; private set; }
        public int TotalScore { get; private set; }

        public UserProfile(string studentName)
        {
            StudentName = studentName;
            TotalQuizzesTaken = 0;
            TotalScore = 0;
        }

        public void RecordQuizResult(int score)
        {
            TotalQuizzesTaken++;
            TotalScore += score;
        }

        public double CalculateAverageScore()
        {
            return TotalQuizzesTaken == 0 ? 0.0 : (double)TotalScore / TotalQuizzesTaken;
        }
    }

    // Quiz Platform Management Service
    public class QuizPlatformService
    {
        private List<Quiz> availableQuizzes = new List<Quiz>();
        private List<UserProfile> studentLeaderboard = new List<UserProfile>();

        public void RegisterStudent(UserProfile profile)
        {
            studentLeaderboard.Add(profile);
        }

        public void AddQuiz(Quiz quiz)
        {
            availableQuizzes.Add(quiz);
        }

        public void ConductQuiz(string quizTitle, UserProfile student, List<int> studentAnswers)
        {
            var quiz = availableQuizzes.FirstOrDefault(q => q.Title.Equals(quizTitle, StringComparison.OrdinalIgnoreCase));
            if (quiz == null)
            {
                Console.WriteLine($"Quiz '{quizTitle}' not found.");
                return;
            }

            int currentScore = 0;
            Console.WriteLine($"\n=== Starting Quiz: {quiz.Title} ({quiz.Subject}) for {student.StudentName} ===");

            for (int i = 0; i < quiz.Questions.Count; i++)
            {
                var q = quiz.Questions[i];
                bool isCorrect = i < studentAnswers.Count && q.ValidateAnswer(studentAnswers[i]);
                
                if (isCorrect)
                {
                    currentScore += 10;
                    Console.WriteLine($"Question {i + 1}: Correct! (+10 pts)");
                }
                else
                {
                    Console.WriteLine($"Question {i + 1}: Incorrect.");
                }
            }

            student.RecordQuizResult(currentScore);
            Console.WriteLine($"Final Score for {quiz.Title}: {currentScore} pts");
        }

        public void DisplayLeaderboard()
        {
            Console.WriteLine("\n=== Student Performance Leaderboard ===");
            var sortedList = studentLeaderboard.OrderByDescending(s => s.TotalScore).ToList();

            Console.WriteLine($"{"Rank",-6}{"Student Name",-20}{"Quizzes Taken",-15}{"Total Score",-15}{"Avg Score",-10}");
            Console.WriteLine(new string('-', 66));

            for (int i = 0; i < sortedList.Count; i++)
            {
                var s = sortedList[i];
                Console.WriteLine($"#{i + 1,-5}{s.StudentName,-20}{s.TotalQuizzesTaken,-15}{s.TotalScore,-15}{s.CalculateAverageScore(),-10:F1}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            QuizPlatformService platform = new QuizPlatformService();

            // Setup Students
            UserProfile student1 = new UserProfile("Abel Wolde");
            UserProfile student2 = new UserProfile("Jane Smith");
            platform.RegisterStudent(student1);
            platform.RegisterStudent(student2);

            // Create Quiz
            Quiz csQuiz = new Quiz("C# & Object-Oriented Design Basics", "Computer Science");
            csQuiz.AddQuestion(new Question("Which OOP principle wraps data and methods into a single unit?", 
                new List<string> { "Inheritance", "Encapsulation", "Polymorphism", "Abstraction" }, 1));
            csQuiz.AddQuestion(new Question("What keyword is used to create an instance of a class in C#?", 
                new List<string> { "new", "this", "create", "base" }, 0));

            platform.AddQuiz(csQuiz);

            // Simulate Taking Quizzes
            platform.ConductQuiz("C# & Object-Oriented Design Basics", student1, new List<int> { 1, 0 }); // Both correct
            platform.ConductQuiz("C# & Object-Oriented Design Basics", student2, new List<int> { 1, 2 }); // One correct

            // Display Statistics & Leaderboard
            platform.DisplayLeaderboard();
        }
    }
}
