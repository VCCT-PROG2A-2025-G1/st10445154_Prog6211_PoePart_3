using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CybersecurityAssistant
{
    public partial class MainWindow : Window
    {
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

        public class SecurityTask
        {
            public Guid Id { get; set; }
            public string Description { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? CompletedDate { get; set; }
            public string Priority { get; set; }
            public bool IsCompleted { get; set; }
        }

