namespace QuizQuiz
{
    class Program
    {
        static List<Question> questions = new List<Question>();
        static string filePath = "questions.txt";

        static void Main()
        {
            LoadQuestions();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Викторина");
                Console.WriteLine("1. Начать викторину");
                Console.WriteLine("2. Добавить вопрос");
                //Console.WriteLine("3. Сгенерировать случайные вопросы");
                Console.WriteLine("4. Выход");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        StartQuiz();
                        break;
                    case "2":
                        AddQuestion();
                        break;
                    //case "3":
                        //GenerateRandomQuestions();
                        //break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void StartQuiz()
        {
            Console.Clear();

            // Если нет вопросов, генерируем 10 случайных
            if (questions.Count == 0)
            {
                Console.WriteLine("В базе нет вопросов. Генерация 10 случайных вопросов...");
                questions = QuestionsGenerator(10);
                SaveQuestions();
                Console.WriteLine("10 вопросов сгенерировано. Нажмите любую клавишу для начала викторины...");
                Console.ReadKey();
            }

            Console.WriteLine("Викторина началась!");
            Console.WriteLine($"Всего доступно вопросов: {questions.Count}");

            // Выбираем 3 сложных и 7 легких вопросов
            var hardQuestions = questions.Where(q => q.Type == QuestionType.Hard)
                                       .OrderBy(x => Guid.NewGuid())
                                       .Take(3)
                                       .ToList();

            var easyQuestions = questions.Where(q => q.Type == QuestionType.Easy)
                                        .OrderBy(x => Guid.NewGuid())
                                        .Take(7)
                                        .ToList();

            var quizQuestions = new List<Question>();
            quizQuestions.AddRange(hardQuestions);
            quizQuestions.AddRange(easyQuestions);
            quizQuestions = quizQuestions.OrderBy(x => Guid.NewGuid()).ToList();

            int score = 0;

            foreach (var question in quizQuestions)
            {
                Console.Clear();
                Console.WriteLine($"Вопрос {quizQuestions.IndexOf(question) + 1} из {quizQuestions.Count}");

                // Цвет вопроса
                if (question.Type == QuestionType.Hard)
                    Console.ForegroundColor = ConsoleColor.Red;
                else
                    Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine(question.Text);
                Console.ResetColor();

                // Вывод вариантов
                for (int i = 0; i < question.Answers.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {question.Answers[i]}");
                }

                // Ввод ответа
                int answer;
                while (true)
                {
                    Console.Write("Ваш ответ (номер): ");
                    if (int.TryParse(Console.ReadLine(), out answer) && answer >= 1 && answer <= question.Answers.Count)
                        break;
                    Console.WriteLine("Некорректный ввод. Попробуйте еще раз.");
                }

                // Проверка
                if (answer - 1 == question.CorrectIndex)
                {
                    Console.WriteLine("Правильно!");
                    score += question.Type == QuestionType.Hard ? 5 : 1;
                }
                else
                {
                    Console.WriteLine($"Неправильно! Правильный ответ: {question.CorrectIndex + 1}");
                }

                Console.WriteLine($"Текущий счет: {score}");
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }

            Console.Clear();
            Console.WriteLine("=== Результаты викторины ===");
            Console.WriteLine($"Правильных ответов: {score}/{quizQuestions.Count}");
            Console.WriteLine($"Набрано баллов: {score}");
            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }

        static void AddQuestion()
        {
            Console.Clear();
            Console.WriteLine("Добавление вопроса");

            Console.Write("Текст вопроса: ");
            string text = Console.ReadLine();

            Console.Write("Тип (1 - легкий, 2 - сложный): ");
            QuestionType type = Console.ReadLine() == "1" ? QuestionType.Easy : QuestionType.Hard;

            var answers = new List<string>();
            Console.WriteLine("Введите варианты ответов (минимум 3):");

            while (true)
            {
                Console.Write($"Ответ {answers.Count + 1}: ");
                string answer = Console.ReadLine();
                answers.Add(answer);

                if (answers.Count >= 3)
                {
                    Console.Write("Добавить еще? (да/нет): ");
                    if (Console.ReadLine().ToLower() != "да") break;
                }
            }

            Console.Write("Номер правильного ответа: ");
            int correctIndex = int.Parse(Console.ReadLine()) - 1;

            questions.Add(new Question(text, answers, correctIndex, type));
            SaveQuestions();

            Console.WriteLine("Вопрос добавлен!");
        }

        /*static void GenerateRandomQuestions()
        {
            Console.Clear();
            Console.Write("Сколько вопросов сгенерировать? ");
            int count = int.Parse(Console.ReadLine());

            List<Question> newQuestions = QuestionsGenerator(count);
            questions.AddRange(newQuestions);
            SaveQuestions();

            Console.WriteLine($"Сгенерировано {count} новых вопросов!");
            Console.WriteLine($"Всего вопросов: {questions.Count}");
        }
        */

        static List<Question> QuestionsGenerator(int count)
        {
            List<Question> generatedQuestions = new();
            Random random = new();

            // Подготовленные данные для генерации вопросов
            List<string> easyQuestions = new List<string>
        {
            "Сколько будет 2+2?",
            "Как называется столица Франции?",
            "Какой газ преобладает в атмосфере Земли?",
            "Кто написал 'Евгения Онегина'?",
            "Сколько цветов у радуги?"
        };

            List<string> hardQuestions = new List<string>
        {
            "В каком году произошла Куликовская битва?",
            "Какова температура плавления вольфрама?",
            "Кто открыл закон сохранения энергии?",
            "Как называется самая длинная река в Южной Америке?",
            "Какой элемент обозначается химическим символом 'Hg'?"
        };

            List<List<string>> easyAnswers = new List<List<string>>
        {
            new List<string> {"3", "4", "5", "6"},
            new List<string> {"Лондон", "Берлин", "Париж", "Мадрид"},
            new List<string> {"Кислород", "Азот", "Углекислый газ", "Водород"},
            new List<string> {"Пушкин", "Лермонтов", "Толстой", "Достоевский"},
            new List<string> {"5", "6", "7", "8"}
        };

            List<List<string>> hardAnswers = new List<List<string>>
        {
            new List<string> {"1240", "1380", "1480", "1580"},
            new List<string> {"1000°C", "2000°C", "3000°C", "3422°C"},
            new List<string> {"Ньютон", "Эйнштейн", "Ломоносов", "Майер"},
            new List<string> {"Амазонка", "Нил", "Миссисипи", "Конго"},
            new List<string> {"Золото", "Ртуть", "Серебро", "Гелий"}
        };

            // Метод для выбора случайного типа вопроса
            QuestionType GenerateQuestionType()
            {
                return random.Next(2) == 0 ? QuestionType.Easy : QuestionType.Hard;
            }

            for (int i = 0; i < count; i++)
            {
                QuestionType type = GenerateQuestionType();
                int questionIndex;
                List<string> answers;
                int correctIndex;
                string text;

                if (type == QuestionType.Easy)
                {
                    questionIndex = random.Next(easyQuestions.Count);
                    text = easyQuestions[questionIndex];
                    answers = new List<string>(easyAnswers[questionIndex]);
                    correctIndex = random.Next(answers.Count);
                }
                else
                {
                    questionIndex = random.Next(hardQuestions.Count);
                    text = hardQuestions[questionIndex];
                    answers = new List<string>(hardAnswers[questionIndex]);
                    correctIndex = random.Next(answers.Count);
                }

                // Перемешиваем ответы, чтобы правильный не всегда был на одном месте
                string correctAnswer = answers[correctIndex];
                answers = answers.OrderBy(x => random.Next()).ToList();
                correctIndex = answers.IndexOf(correctAnswer);

                Question question = new Question(text, answers, correctIndex, type);
                generatedQuestions.Add(question);
            }

            return generatedQuestions;
        }

        static void LoadQuestions()
        {
            if (!File.Exists(filePath)) return;

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split('|');

                string text = parts[0];
                QuestionType type = parts[1] == "Hard" ? QuestionType.Hard : QuestionType.Easy;
                int correctIndex = int.Parse(parts[2]);

                var answers = new List<string>();
                for (int i = 3; i < parts.Length; i++)
                {
                    answers.Add(parts[i]);
                }

                questions.Add(new Question(text, answers, correctIndex, type));
            }
        }

        static void SaveQuestions()
        {
            var lines = new List<string>();

            foreach (var q in questions)
            {
                string line = $"{q.Text}|{q.Type}|{q.CorrectIndex}";
                foreach (var a in q.Answers)
                {
                    line += $"|{a}";
                }
                lines.Add(line);
            }

            File.WriteAllLines(filePath, lines);
        }
    }
}
