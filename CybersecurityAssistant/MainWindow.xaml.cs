using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CybersecurityAssistant
{
    public partial class MainWindow : Window
    {
        private List<SecurityTask> tasks = new List<SecurityTask>();
        private List<ActionLogEntry> actionLog = new List<ActionLogEntry>();
        private List<QuizQuestion> quizQuestions = new List<QuizQuestion>();
        private QuizSession currentQuizSession;
        private int currentQuestionIndex = 0;
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            InitializeQuizQuestions();
            InitializeDefaultTasks();
            LogAction("System started", "Application initialized");
        }

        private void InitializeQuizQuestions()
        {
            quizQuestions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What does 2FA stand for?",
                    Options = new[] { "Two-Factor Authentication", "Two-File Access", "Two-Factor Authorization", "Two-Form Authentication" },
                    CorrectAnswer = 0
                },
                new QuizQuestion
                {
                    Question = "Which of the following is the strongest password?",
                    Options = new[] { "password123", "P@ssw0rd!", "MyDog2023", "Tr0ub4dor&3" },
                    CorrectAnswer = 3
                },
                new QuizQuestion
                {
                    Question = "What is phishing?",
                    Options = new[] { "A type of malware", "A social engineering attack", "A hardware vulnerability", "A network protocol" },
                    CorrectAnswer = 1
                },
                new QuizQuestion
                {
                    Question = "How often should you update your passwords?",
                    Options = new[] { "Never", "Every 10 years", "Every 3-6 months", "Every day" },
                    CorrectAnswer = 2
                },
                new QuizQuestion
                {
                    Question = "What is the purpose of a firewall?",
                    Options = new[] { "To cool down computers", "To block unauthorized network access", "To speed up internet", "To backup data" },
                    CorrectAnswer = 1
                },
                new QuizQuestion
                {
                    Question = "What should you do if you receive a suspicious email?",
                    Options = new[] { "Click all links to investigate", "Forward it to friends", "Delete it immediately", "Report it and delete it" },
                    CorrectAnswer = 3
                },
                new QuizQuestion
                {
                    Question = "What is ransomware?",
                    Options = new[] { "Free software", "Malware that encrypts files for money", "A type of antivirus", "A backup solution" },
                    CorrectAnswer = 1
                },
                new QuizQuestion
                {
                    Question = "Which is safer for online banking?",
                    Options = new[] { "Public WiFi", "Home WiFi with WPA3", "Hotel WiFi", "Coffee shop WiFi" },
                    CorrectAnswer = 1
                },
                new QuizQuestion
                {
                    Question = "How can you identify a secure website?",
                    Options = new[] { "It has ads", "It starts with https://", "It has many colors", "It loads quickly" },
                    CorrectAnswer = 1
                },
                new QuizQuestion
                {
                    Question = "What is social engineering?",
                    Options = new[] { "Building social networks", "Manipulating people to reveal information", "Engineering social media", "Creating social apps" },
                    CorrectAnswer = 1
                }
            };
        }

        private void InitializeDefaultTasks()
        {
            var defaultTasks = new[]
            {
                "Enable two-factor authentication on all accounts",
                "Update all software and operating systems",
                "Review and update passwords",
                "Check privacy settings on social media",
                "Install reputable antivirus software",
                "Backup important data",
                "Review bank and credit card statements",
                "Enable automatic screen lock",
                "Update router firmware",
                "Review app permissions on mobile devices"
            };

            foreach (var taskDesc in defaultTasks)
            {
                tasks.Add(new SecurityTask
                {
                    Id = Guid.NewGuid(),
                    Description = taskDesc,
                    CreatedDate = DateTime.Now,
                    Priority = "Medium",
                    IsCompleted = false
                });
            }

            UpdateTasksDisplay();
            LogAction("Tasks initialized", $"Added {defaultTasks.Length} default cybersecurity tasks");
        }

        private void ChatInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            var message = ChatInput.Text.Trim();
            if (string.IsNullOrEmpty(message) || message == "Type your message here...")
                return;

            // Add user message to chat
            AddChatMessage("You", message, "#3498DB");
            ChatInput.Text = "";

            // Process the message
            var response = ProcessUserMessage(message);
            AddChatMessage("Assistant", response, "#27AE60");

            LogAction("Chat message", $"User: {message}");
        }

        private void AddChatMessage(string sender, string message, string color)
        {
            var border = new Border
            {
                Style = (Style)FindResource("TaskItemStyle"),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color == "#3498DB" ? "#EBF5FB" : "#E8F8F5")),
                Margin = new Thickness(sender == "You" ? 50 : 0, 2, sender == "You" ? 0 : 50, 2)
            };

            var stackPanel = new StackPanel();

            var senderText = new TextBlock
            {
                Text = $"{sender}:",
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color)),
                Margin = new Thickness(0, 0, 0, 5)
            };

            var messageText = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14
            };

            stackPanel.Children.Add(senderText);
            stackPanel.Children.Add(messageText);
            border.Child = stackPanel;

            ChatPanel.Children.Add(border);
            ChatScrollViewer.ScrollToEnd();
        }

        private string ProcessUserMessage(string message)
        {
            message = message.ToLower();

            // Task-related patterns
            if (Regex.IsMatch(message, @"add.*task|create.*task|new.*task"))
            {
                var taskMatch = Regex.Match(message, @"(?:add|create|new).*?task[:\s]*(.+)");
                if (taskMatch.Success)
                {
                    var taskDesc = taskMatch.Groups[1].Value.Trim();
                    return AddTaskFromChat(taskDesc);
                }
                return "I'd be happy to add a task for you! Please specify what cybersecurity task you'd like to add. For example: 'Add task: Enable firewall'";
            }

            if (Regex.IsMatch(message, @"show.*task|list.*task|view.*task|my.*task"))
            {
                return ShowTasksFromChat();
            }

            if (Regex.IsMatch(message, @"complete.*task|finish.*task|done.*task"))
            {
                return "To complete a task, please go to the Tasks tab and click the checkbox next to the task you've finished.";
            }

            // Quiz-related patterns
            if (Regex.IsMatch(message, @"quiz|test|question|challenge"))
            {
                if (Regex.IsMatch(message, @"start|begin|take"))
                {
                    return StartQuizFromChat();
                }
                return "I can help you test your cybersecurity knowledge! Would you like to start a quiz? Just say 'start quiz' or click the quiz button.";
            }

            // Reminder patterns
            if (Regex.IsMatch(message, @"remind|reminder|alert"))
            {
                return "I've noted your reminder request! For now, I recommend setting calendar reminders for your cybersecurity tasks. Future versions will include automated reminders.";
            }

            // Log patterns
            if (Regex.IsMatch(message, @"log|history|actions"))
            {
                return ShowLogFromChat();
            }

            // Help patterns
            if (Regex.IsMatch(message, @"help|what.*can.*do|commands"))
            {
                return @"I can help you with:
🔹 Task Management: 'Add task: [description]', 'Show my tasks'
🔹 Security Quiz: 'Start quiz', 'Take a test'
🔹 Action History: 'Show action log', 'View history'
🔹 General Help: Ask about cybersecurity best practices

Try saying things like:
• 'Add task: Update antivirus software'
• 'Start a cybersecurity quiz'
• 'Show my action log'";
            }

            // Security advice patterns
            if (Regex.IsMatch(message, @"password|2fa|two.*factor|authentication"))
            {
                return "Great question about password security! Here are key tips:\n• Use unique, complex passwords for each account\n• Enable two-factor authentication wherever possible\n• Consider using a password manager\n• Update passwords regularly (every 3-6 months)\n\nWould you like me to add any password-related tasks to your list?";
            }

            if (Regex.IsMatch(message, @"phishing|email.*security|suspicious.*email"))
            {
                return "Phishing emails are a major threat! Here's how to stay safe:\n• Never click links in suspicious emails\n• Verify sender identity independently\n• Look for spelling/grammar errors\n• Check for urgency tactics\n• When in doubt, delete the email\n\nWould you like to add email security tasks to your list?";
            }

            if (Regex.IsMatch(message, @"backup|data.*protection"))
            {
                return "Data backup is crucial for cybersecurity! Follow the 3-2-1 rule:\n• 3 copies of important data\n• 2 different types of storage media\n• 1 copy stored off-site\n\nRegular backups protect against ransomware and data loss. Should I add backup tasks to your list?";
            }

            // Default response
            return "I'm here to help with your cybersecurity needs! I can manage tasks, quiz your knowledge, and provide security advice. Try asking me to 'add a task', 'start a quiz', or ask about cybersecurity topics like passwords, phishing, or backups.";
        }

        private string AddTaskFromChat(string taskDescription)
        {
            var task = new SecurityTask
            {
                Id = Guid.NewGuid(),
                Description = taskDescription,
                CreatedDate = DateTime.Now,
                Priority = "Medium",
                IsCompleted = false
            };

            tasks.Add(task);
            UpdateTasksDisplay();
            LogAction("Task added", $"Added task: {taskDescription}");

            return $"✅ Task added successfully: \"{taskDescription}\"\n\nYou can view all your tasks in the Tasks tab or say 'show my tasks'.";
        }

        private string ShowTasksFromChat()
        {
            if (tasks.Count == 0)
            {
                return "You don't have any tasks yet. Would you like me to add some cybersecurity tasks for you?";
            }

            var activeTasks = tasks.Where(t => !t.IsCompleted).ToList();
            var completedTasks = tasks.Where(t => t.IsCompleted).ToList();

            var response = $"📋 Your Cybersecurity Tasks:\n\n";
            response += $"Active Tasks ({activeTasks.Count}):\n";

            foreach (var task in activeTasks.Take(5))
            {
                response += $"• {task.Description}\n";
            }

            if (activeTasks.Count > 5)
            {
                response += $"... and {activeTasks.Count - 5} more tasks\n";
            }

            response += $"\n✅ Completed: {completedTasks.Count} tasks\n";
            response += "\nView the Tasks tab to manage your tasks or mark them as complete.";

            return response;
        }

        private string StartQuizFromChat()
        {
            currentQuizSession = new QuizSession
            {
                Id = Guid.NewGuid(),
                StartTime = DateTime.Now,
                Score = 0,
                TotalQuestions = 5
            };

            currentQuestionIndex = 0;
            LogAction("Quiz started", "Started new cybersecurity quiz");

            return "🎯 Starting your cybersecurity quiz! Please go to the Quiz tab to answer the questions. Good luck!";
        }

        private string ShowLogFromChat()
        {
            var recentLogs = actionLog.Skip(Math.Max(0, actionLog.Count - 5)).ToList();
            if (recentLogs.Count == 0)
            {
                return "No actions logged yet.";
            }

            var response = "📊 Recent Actions:\n\n";
            foreach (var log in recentLogs)
            {
                response += $"• {log.Timestamp:HH:mm} - {log.Action}: {log.Details}\n";
            }

            response += $"\nTotal actions logged: {actionLog.Count}\nView the Action Log tab for complete history.";
            return response;
        }

        private void ShowTasks_Click(object sender, RoutedEventArgs e)
        {
            var tabControl = (TabControl)((Button)sender).Tag ?? FindName("TabControl") as TabControl;
            if (tabControl == null)
            {
                // Find the TabControl
                var parent = VisualTreeHelper.GetParent((Button)sender);
                while (parent != null && !(parent is TabControl))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }
                tabControl = parent as TabControl;
            }

            // Switch to Tasks tab (index 1)
            var mainTabControl = FindTabControl(this);
            if (mainTabControl != null)
            {
                mainTabControl.SelectedIndex = 1;
            }
        }

        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            var mainTabControl = FindTabControl(this);
            if (mainTabControl != null)
            {
                mainTabControl.SelectedIndex = 2;
                StartNewQuiz();
            }
        }

        private void ShowLog_Click(object sender, RoutedEventArgs e)
        {
            var mainTabControl = FindTabControl(this);
            if (mainTabControl != null)
            {
                mainTabControl.SelectedIndex = 3;
                UpdateLogDisplay();
            }
        }

        private TabControl FindTabControl(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is TabControl tabControl)
                    return tabControl;

                var result = FindTabControl(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            var taskText = NewTaskInput.Text.Trim();
            if (string.IsNullOrEmpty(taskText) || taskText == "Enter new cybersecurity task...")
                return;

            var task = new SecurityTask
            {
                Id = Guid.NewGuid(),
                Description = taskText,
                CreatedDate = DateTime.Now,
                Priority = "Medium",
                IsCompleted = false
            };

            tasks.Add(task);
            UpdateTasksDisplay();
            LogAction("Task added", $"Added task: {taskText}");

            NewTaskInput.Text = "Enter new cybersecurity task...";
        }

        private void UpdateTasksDisplay()
        {
            TasksPanel.Children.Clear();

            foreach (var task in tasks.OrderBy(t => t.IsCompleted).ThenByDescending(t => t.CreatedDate))
            {
                var border = new Border
                {
                    Style = (Style)FindResource("TaskItemStyle"),
                    Background = new SolidColorBrush(task.IsCompleted ?
                        (Color)ColorConverter.ConvertFromString("#D5EDDA") :
                        (Color)ColorConverter.ConvertFromString("#FFF3CD"))
                };

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

                var checkbox = new CheckBox
                {
                    IsChecked = task.IsCompleted,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 0, 10, 0),
                    Tag = task
                };
                checkbox.Checked += TaskCheckbox_Checked;
                checkbox.Unchecked += TaskCheckbox_Unchecked;

                var textBlock = new TextBlock
                {
                    Text = task.Description,
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 14,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextDecorations = task.IsCompleted ? TextDecorations.Strikethrough : null,
                    Foreground = new SolidColorBrush(task.IsCompleted ? Colors.Gray : Colors.Black)
                };

                var dateText = new TextBlock
                {
                    Text = task.CreatedDate.ToString("MMM dd"),
                    FontSize = 12,
                    Foreground = Brushes.Gray,
                    VerticalAlignment = VerticalAlignment.Top
                };

                Grid.SetColumn(checkbox, 0);
                Grid.SetColumn(textBlock, 1);
                Grid.SetColumn(dateText, 2);

                grid.Children.Add(checkbox);
                grid.Children.Add(textBlock);
                grid.Children.Add(dateText);

                border.Child = grid;
                TasksPanel.Children.Add(border);
            }

            UpdateTaskStats();
        }

        private void TaskCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = (CheckBox)sender;
            var task = (SecurityTask)checkbox.Tag;
            task.IsCompleted = true;
            task.CompletedDate = DateTime.Now;

            UpdateTasksDisplay();
            LogAction("Task completed", $"Completed task: {task.Description}");
        }

        private void TaskCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkbox = (CheckBox)sender;
            var task = (SecurityTask)checkbox.Tag;
            task.IsCompleted = false;
            task.CompletedDate = null;

            UpdateTasksDisplay();
            LogAction("Task uncompleted", $"Uncompleted task: {task.Description}");
        }

        private void UpdateTaskStats()
        {
            var totalTasks = tasks.Count;
            var completedTasks = tasks.Count(t => t.IsCompleted);
            TaskStatsText.Text = $"Tasks: {totalTasks} Total, {completedTasks} Completed";
        }

        private void StartNewQuiz()
        {
            currentQuizSession = new QuizSession
            {
                Id = Guid.NewGuid(),
                StartTime = DateTime.Now,
                Score = 0,
                TotalQuestions = 5
            };

            currentQuestionIndex = 0;
            DisplayNextQuestion();
            LogAction("Quiz started", "Started new cybersecurity quiz");
        }

        private void DisplayNextQuestion()
        {
            QuizPanel.Children.Clear();

            if (currentQuestionIndex >= currentQuizSession.TotalQuestions)
            {
                DisplayQuizResults();
                return;
            }

            var questionIndex = random.Next(quizQuestions.Count);
            var question = quizQuestions[questionIndex];

            var border = new Border
            {
                Style = (Style)FindResource("TaskItemStyle"),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8F4FD"))
            };

            var stackPanel = new StackPanel();

            // Question header
            var headerText = new TextBlock
            {
                Text = $"Question {currentQuestionIndex + 1} of {currentQuizSession.TotalQuestions}",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C3E50")),
                Margin = new Thickness(0, 0, 0, 10)
            };

            // Question text
            var questionText = new TextBlock
            {
                Text = question.Question,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 15)
            };

            stackPanel.Children.Add(headerText);
            stackPanel.Children.Add(questionText);

            // Answer options
            for (int i = 0; i < question.Options.Length; i++)
            {
                var button = new Button
                {
                    Content = $"{(char)('A' + i)}. {question.Options[i]}",
                    Style = (Style)FindResource("ButtonStyle"),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(0, 5, 0, 0),
                    Tag = new { QuestionIndex = questionIndex, AnswerIndex = i }
                };
                button.Click += QuizAnswer_Click;
                stackPanel.Children.Add(button);
            }

            border.Child = stackPanel;
            QuizPanel.Children.Add(border);

            // Progress indicator
            var progressBorder = new Border
            {
                Style = (Style)FindResource("TaskItemStyle"),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ECF0F1")),
                Margin = new Thickness(0, 10, 0, 0)
            };

            var progressText = new TextBlock
            {
                Text = $"Progress: {currentQuestionIndex + 1}/{currentQuizSession.TotalQuestions} • Score: {currentQuizSession.Score}",
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            progressBorder.Child = progressText;
            QuizPanel.Children.Add(progressBorder);
        }

        private void QuizAnswer_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var answerData = (dynamic)button.Tag;
            var questionIndex = (int)answerData.QuestionIndex;
            var answerIndex = (int)answerData.AnswerIndex;
            var question = quizQuestions[questionIndex];

            bool isCorrect = answerIndex == question.CorrectAnswer;

            if (isCorrect)
            {
                currentQuizSession.Score++;
                ShowAnswerFeedback(true, question.Options[question.CorrectAnswer]);
            }
            else
            {
                ShowAnswerFeedback(false, question.Options[question.CorrectAnswer]);
            }

            LogAction("Quiz answer", $"Question {currentQuestionIndex + 1}: {(isCorrect ? "Correct" : "Incorrect")}");
        }

        private void ShowAnswerFeedback(bool isCorrect, string correctAnswer)
        {
            var feedbackBorder = new Border
            {
                Style = (Style)FindResource("TaskItemStyle"),
                Background = new SolidColorBrush(isCorrect ?
                    (Color)ColorConverter.ConvertFromString("#D4EDDA") :
                    (Color)ColorConverter.ConvertFromString("#F8D7DA")),
                Margin = new Thickness(0, 10, 0, 0)
            };

            var feedbackPanel = new StackPanel();

            var resultText = new TextBlock
            {
                Text = isCorrect ? "✅ Correct!" : "❌ Incorrect",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(isCorrect ? Colors.Green : Colors.Red),
                Margin = new Thickness(0, 0, 0, 5)
            };

            var correctAnswerText = new TextBlock
            {
                Text = $"Correct answer: {correctAnswer}",
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var nextButton = new Button
            {
                Content = currentQuestionIndex + 1 >= currentQuizSession.TotalQuestions ? "Finish Quiz" : "Next Question",
                Style = (Style)FindResource("QuizButtonStyle"),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            nextButton.Click += NextQuestion_Click;

            feedbackPanel.Children.Add(resultText);
            if (!isCorrect)
            {
                feedbackPanel.Children.Add(correctAnswerText);
            }
            feedbackPanel.Children.Add(nextButton);

            feedbackBorder.Child = feedbackPanel;
            QuizPanel.Children.Add(feedbackBorder);
        }

        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            currentQuestionIndex++;
            DisplayNextQuestion();
        }

        private void DisplayQuizResults()
        {
            currentQuizSession.EndTime = DateTime.Now;
            var percentage = (int)((double)currentQuizSession.Score / currentQuizSession.TotalQuestions * 100);

            QuizPanel.Children.Clear();

            var resultBorder = new Border
            {
                Style = (Style)FindResource("TaskItemStyle"),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8F5E8"))
            };

            var resultPanel = new StackPanel();

            var headerText = new TextBlock
            {
                Text = "🎉 Quiz Complete!",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 15)
            };

            var scoreText = new TextBlock
            {
                Text = $"Your Score: {currentQuizSession.Score}/{currentQuizSession.TotalQuestions} ({percentage}%)",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var performanceText = new TextBlock
            {
                Text = GetPerformanceMessage(percentage),
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 15)
            };

            var newQuizButton = new Button
            {
                Content = "🎯 Take Another Quiz",
                Style = (Style)FindResource("QuizButtonStyle"),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            newQuizButton.Click += StartQuiz_Click;

            resultPanel.Children.Add(headerText);
            resultPanel.Children.Add(scoreText);
            resultPanel.Children.Add(performanceText);
            resultPanel.Children.Add(newQuizButton);

            resultBorder.Child = resultPanel;
            QuizPanel.Children.Add(resultBorder);

            UpdateQuizStats();
            LogAction("Quiz completed", $"Score: {currentQuizSession.Score}/{currentQuizSession.TotalQuestions} ({percentage}%)");
        }

        private string GetPerformanceMessage(int percentage)
        {
            if (percentage >= 90)
                return "🌟 Excellent! You're a cybersecurity expert!";
            else if (percentage >= 70)
                return "👍 Good job! You have solid cybersecurity knowledge.";
            else if (percentage >= 50)
                return "📚 Not bad! Consider reviewing cybersecurity best practices.";
            else
                return "🔍 Keep learning! Cybersecurity knowledge is crucial for staying safe online.";
        }

        private void UpdateQuizStats()
        {
            // This would typically load from persistent storage
            QuizStatsText.Text = "Quiz Stats: 1 attempt, 80% average score";
        }

        private void RefreshLog_Click(object sender, RoutedEventArgs e)
        {
            UpdateLogDisplay();
            LogAction("Log refreshed", "User refreshed the action log");
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear the action log?",
                                       "Clear Log", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                actionLog.Clear();
                UpdateLogDisplay();
                LogAction("Log cleared", "Action log was cleared by user");
            }
        }

        private void UpdateLogDisplay()
        {
            LogPanel.Children.Clear();

            if (actionLog.Count == 0)
            {
                var nologBorder = new Border
                {
                    Style = (Style)FindResource("TaskItemStyle")
                };

                var noLogText = new TextBlock
                {
                    Text = "No actions logged yet.",
                    FontStyle = FontStyles.Italic,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                nologBorder.Child = noLogText;
                LogPanel.Children.Add(nologBorder);
                return;
            }

            foreach (var log in actionLog.OrderByDescending(l => l.Timestamp))
            {
                var border = new Border
                {
                    Style = (Style)FindResource("TaskItemStyle"),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8F9FA"))
                };

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var timeText = new TextBlock
                {
                    Text = log.Timestamp.ToString("MM/dd HH:mm:ss"),
                    FontSize = 12,
                    Foreground = Brushes.Gray,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 0, 10, 0)
                };

                var contentPanel = new StackPanel();

                var actionText = new TextBlock
                {
                    Text = log.Action,
                    FontWeight = FontWeights.Bold,
                    FontSize = 14
                };

                var detailsText = new TextBlock
                {
                    Text = log.Details,
                    FontSize = 12,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = Brushes.DarkGray
                };

                contentPanel.Children.Add(actionText);
                contentPanel.Children.Add(detailsText);

                Grid.SetColumn(timeText, 0);
                Grid.SetColumn(contentPanel, 1);

                grid.Children.Add(timeText);
                grid.Children.Add(contentPanel);

                border.Child = grid;
                LogPanel.Children.Add(border);
            }
        }

        private void LogAction(string action, string details)
        {
            var logEntry = new ActionLogEntry
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                Action = action,
                Details = details
            };

            actionLog.Add(logEntry);

            // Keep only the last 100 log entries to prevent memory issues
            if (actionLog.Count > 100)
            {
                actionLog.RemoveAt(0);
            }
        }
    }

    // Data Models
    public class SecurityTask
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Priority { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class QuizQuestion
    {
        public string Question { get; set; }
        public string[] Options { get; set; }
        public int CorrectAnswer { get; set; }
    }

    public class QuizSession
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
    }

    public class ActionLogEntry
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
    }
}