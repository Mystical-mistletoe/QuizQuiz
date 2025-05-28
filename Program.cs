using System;

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
                Console.WriteLine("Файл будет сохранён в: " + Path.GetFullPath(filePath));
                Console.WriteLine("Викторина");
                Console.WriteLine("1. Начать викторину");
                Console.WriteLine("2. Добавить вопрос");
                Console.WriteLine("3. Сгенерировать случайные вопросы");
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
                    case "3":
                        GenerateRandomQuestions();
                        break;
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

            // Создаем список для сложных вопросов
            List<Question> hardQuestions = new List<Question>();
            // Создаем список для легких вопросов
            List<Question> easyQuestions = new List<Question>();

            // Создаем генератор случайных чисел
            Random random = new Random();

            // Перемешиваем вопросы в случайном порядке
            List<Question> shuffledQuestions = new List<Question>(questions);

            // Простой алгоритм перемешивания
            for (int i = 0; i < shuffledQuestions.Count; i++)
            {
                int randomIndex = random.Next(shuffledQuestions.Count);
                Question temp = shuffledQuestions[i];
                shuffledQuestions[i] = shuffledQuestions[randomIndex];
                shuffledQuestions[randomIndex] = temp;
            }

            // Отбираем вопросы по типам
            foreach (var question in shuffledQuestions)
            {
                if (question.Type == QuestionType.Hard && hardQuestions.Count < 3)
                {
                    hardQuestions.Add(question);
                }
                else if (question.Type == QuestionType.Easy && easyQuestions.Count < 7)
                {
                    easyQuestions.Add(question);
                }

                // Прерываем цикл, если набрали нужное количество вопросов
                if (hardQuestions.Count == 3 && easyQuestions.Count == 7)
                    break;
            }
            /*
            var quizQuestions = new List<Question>();
            quizQuestions.AddRange(hardQuestions);
            quizQuestions.AddRange(easyQuestions);
            quizQuestions = quizQuestions.OrderBy(x => Guid.NewGuid()).ToList();
            */

            // Объединяем вопросы (сначала сложные, потом легкие)
            List<Question> quizQuestions = new List<Question>();
            quizQuestions.AddRange(hardQuestions);
            quizQuestions.AddRange(easyQuestions);

            // Счетчик правильных ответов
            int score = 0;
            int countBal = 0;

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
                    countBal += 1;
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
            Console.WriteLine($"Правильных ответов: {countBal}/{quizQuestions.Count}");
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

        static void GenerateRandomQuestions()
        {
            Console.Clear();
            Console.Write("Сколько вопросов сгенерировать? ");
            int count = int.Parse(Console.ReadLine());

            List<Question> newQuestions = QuestionsGenerator(count);
            questions.AddRange(newQuestions); //добавить в конец списка
            SaveQuestions();

            Console.WriteLine($"Сгенерировано {count} новых вопросов!");
            Console.WriteLine($"Всего вопросов: {questions.Count}");
        }

        static List<Question> QuestionsGenerator(int count)
        {
            // Создаем пустой список для хранения вопросов
            List<Question> generatedQuestions = new();
            Random random = new();

            // Подготовленные данные для генерации вопросов
            List<string> easyQuestions = new List<string>
            {
                "Сколько всего даров смерти?",
                "Как называется столица Англии?",
                "Как зовут эльфа Малфоев?",
                "Самое частое заклинание Гарри Поттера против противников?",
                "Первый крестраж, уничтоженный Гарри?",
                "Какой патронус у Гермионы Грейнджер?",
                "Какой патронус у Гарри Поттера?"
            };

            List<string> hardQuestions = new List<string>
            {
                "Имя матери Снейпа?",
                "Какое существо охраняет вход в кабинет директора?",
                "Из какихрастений состоит зелье изменения?",
                "Как взали дедушку Володи?",
                "Какой кентавр был изгнан из Запретного леса за то, что помогал гарри Поттеру?"
            };

            List<List<string>> easyAnswers = new List<List<string>>
            {
                new List<string> {"3", "4", "5", "6"},
                new List<string> {"Лондон", "Берлин", "Париж", "Мадрид"},
                new List<string> {"Добби", "Кикимер", "Винки", "Голем"},
                new List<string> {"экспеллиармус", "авадакедавра", "империо", "левиоса"},
                new List<string> {"дневник", "кольцо", "диадема", "Гарри"},
                new List<string> { "выдра", "сокол", "кот", "олень"},
                new List<string> { "олень", "выдра", "кот", "собака"}
            };

            List<List<string>> hardAnswers = new List<List<string>>
            {
                new List<string> {"Айлин", "Эвелин", "Ирма", "Марволо"},
                new List<string> {"горгулья", "феникс", "гиппогриф", "никто"},
                new List<string> {"змеиная кожа", "споры грибов", "горец птичий", "их нет"},
                new List<string> {"Марволо", "Морфин", "Том", "Реддл"},
                new List<string> {"Флоренц", "Бэйн", "Ронан", "Фьюри"}
            };

            int hardQuestionsCount = 0;
            const int MAX_HARD_QUESTIONS = 3;

            // Генерируем указанное количество вопросов
            for (int i = 0; i < count; i++)
            {
                // Проверяем, можно ли добавить сложный вопрос
                QuestionType type;
                if (hardQuestionsCount < MAX_HARD_QUESTIONS && random.Next(2) == 1)
                {
                    type = QuestionType.Hard;
                    hardQuestionsCount++;
                }
                else
                {
                    type = QuestionType.Easy;
                }

                int questionIndex;
                List<string> answers;
                int correctIndex = 0; // Правильный ответ всегда первый в списке
                string text;

                if (type == QuestionType.Easy )
                {
                    questionIndex = random.Next(easyQuestions.Count);
                    text = easyQuestions[questionIndex];
                    answers = new List<string>(easyAnswers[questionIndex]);
                }
                else
                {
                    questionIndex = random.Next(hardQuestions.Count);
                    text = hardQuestions[questionIndex];
                    answers = new List<string>(hardAnswers[questionIndex]);
                }

                // Перемешиваем ответы, чтобы правильный не всегда был на одном месте
                // Запоминаем правильный ответ
                string correctAnswer = answers[correctIndex];

                for (int j = 0; j < answers.Count; j++)
                {
                    int swapIndex = random.Next(answers.Count);
                    string temp = answers[j];
                    answers[j] = answers[swapIndex];
                    answers[swapIndex] = temp;
                }

                // Находим новый индекс правильного ответа после перемешивания
                correctIndex = -1;
                for (int k = 0; k < answers.Count; k++)
                {
                    if (answers[k] == correctAnswer)
                    {
                        correctIndex = k;
                        break;
                    }
                }
                // Создаем новый вопрос и добавляем его в список
                Question question = new Question(text, answers, correctIndex, type);
                generatedQuestions.Add(question);
            }
            // Возвращаем сгенерированный список вопросов
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

            File.WriteAllLines(filePath, lines); //rewrite
        }
    }
}
