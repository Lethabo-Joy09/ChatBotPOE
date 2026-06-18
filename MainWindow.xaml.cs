using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChatBotPOE
{
    /// <summary>
    /// Main application window for CyberBot - Cybersecurity Awareness Assistant.
    /// Integrates all features: Chat, Task Management, Quiz, Activity Log, NLP,
    /// Memory, Sentiment Analysis, and Voice Greeting.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Core managers and engines
        private TaskManager taskManager = new TaskManager();
        private QuizEngine quizEngine = new QuizEngine();
        private NLPSimulator nlp = new NLPSimulator();
        private ActivityLogger logger = ActivityLogger.Instance;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        /// <summary>
        /// Window load event - initializes the chatbot with voice greeting,
        /// loads tasks from database, and prepares the quiz.
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            VoiceGreeting.Speak("Welcome to CyberBot. I'm here to help you stay safe online.");
            RefreshTasks();
            logger.Log("Application started");
            ShowQuizQuestion();
        }

        // ------------------- CHAT HANDLING -------------------

        /// <summary>
        /// Handles Send button click - processes user input and generates response.
        /// </summary>
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserInput(userInputTextBox.Text);
            userInputTextBox.Clear();
        }

        /// <summary>
        /// Allows Enter key to send messages.
        /// </summary>
        private void UserInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SendButton_Click(sender, e);
        }

        /// <summary>
        /// Main chat processing logic:
        /// - Detects intent using NLP
        /// - Routes to appropriate handler (task, reminder, quiz, activity log, memory)
        /// - Generates responses with sentiment analysis
        /// - Logs all actions
        /// - Speaks responses via voice synthesis
        /// </summary>
        private void ProcessUserInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            logger.Log($"User: {input}");

            string intent = nlp.DetectIntent(input);
            string response = "";

            // Handle Quiz Intent
            if (intent == "quiz" || input.ToLower().Contains("start quiz"))
            {
                quizEngine.Reset();
                response = "Quiz started! Answer the questions.";
                ShowQuizQuestion();
                logger.Log("Quiz started");
            }
            // Handle Task Intent
            else if (intent == "task")
            {
                string description = nlp.ExtractDescription(input, "task");
                string reminderPhrase = nlp.ExtractReminderTime(input);

                if (!string.IsNullOrEmpty(description))
                {
                    bool added = taskManager.AddTask(description, description, reminderPhrase);
                    if (added)
                    {
                        response = $"Task added: '{description}'" +
                                   (string.IsNullOrEmpty(reminderPhrase) ? "" : $" with reminder {reminderPhrase}.");
                        if (string.IsNullOrEmpty(reminderPhrase))
                            response += " Would you like a reminder? (say 'remind me in X days')";
                        logger.Log($"Task added via NLP: {description}");
                    }
                    else
                        response = "Sorry, I couldn't add the task. Please try again.";
                }
                else
                    response = "I didn't understand the task description. Please be more specific.";
            }
            // Handle Reminder Intent
            else if (intent == "reminder")
            {
                string description = nlp.ExtractDescription(input, "reminder");
                string reminderPhrase = nlp.ExtractReminderTime(input);
                if (!string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(reminderPhrase))
                {
                    bool added = taskManager.AddTask(description, description, reminderPhrase);
                    response = added ? $"Reminder set for '{description}' on {reminderPhrase}." : "Failed to set reminder.";
                }
                else
                {
                    var due = taskManager.GetDueReminders();
                    if (due.Count > 0)
                        response = "Here are your due reminders: " + string.Join("; ", due);
                    else
                        response = "You have no due reminders at the moment.";
                }
            }
            // Handle Activity Log Request
            else if (input.ToLower().Contains("show activity log") || input.ToLower().Contains("what have you done"))
            {
                var logs = logger.GetRecentLogs(10);
                response = "Recent actions:\n" + string.Join("\n", logs);
                logger.Log("Activity log viewed");
            }
            // Handle Memory Summary Request
            else if (input.ToLower().Contains("summary") || input.ToLower().Contains("what do you remember"))
            {
                string summary = MemoryManager.GetSummary();
                if (string.IsNullOrEmpty(summary) || summary == "I don't remember anything yet.")
                    response = "I don't remember anything yet. Tell me something about yourself!";
                else
                    response = summary;
            }
            // Generic Response with Sentiment and Memory
            else
            {
                string sentiment = SentimentAnalyzer.Analyze(input);
                if (sentiment == "positive")
                    response = "I'm glad you're feeling positive! How can I help you with cybersecurity?";
                else if (sentiment == "negative")
                    response = "I'm sorry to hear that. Let's work on improving your security together.";
                else
                    response = "I'm here to help. You can ask me to add tasks, set reminders, or take a quiz.";

                // Check if user introduced themselves
                if (input.ToLower().Contains("my name is"))
                {
                    var parts = input.Split(new[] { "my name is" }, StringSplitOptions.None);
                    if (parts.Length > 1)
                    {
                        string name = parts[1].Trim();
                        MemoryManager.Remember("name", name);
                        response += $" Great to meet you, {name}!";
                    }
                }
            }

            // Display in chat
            chatListBox.Items.Add($"You: {input}");
            chatListBox.Items.Add($"Bot: {response}");
            chatListBox.ScrollIntoView(chatListBox.Items[chatListBox.Items.Count - 1]);

            // Log and speak response
            logger.Log($"Bot: {response}");
            VoiceGreeting.Speak(response);
        }

        // ------------------- TASK GRID OPERATIONS -------------------

        /// <summary>
        /// Refreshes the task DataGrid with current data from database.
        /// </summary>
        private void RefreshTasks()
        {
            DataTable dt = taskManager.GetAllTasks();
            tasksDataGrid.ItemsSource = dt.DefaultView;
        }

        /// <summary>
        /// Refresh button click handler.
        /// </summary>
        private void RefreshTasks_Click(object sender, RoutedEventArgs e) => RefreshTasks();

        /// <summary>
        /// Marks the selected task as completed.
        /// </summary>
        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (tasksDataGrid.SelectedItem is DataRowView row)
            {
                int id = Convert.ToInt32(row["id"]);
                taskManager.CompleteTask(id);
                RefreshTasks();
                logger.Log($"Completed task ID {id}");
            }
            else
            {
                MessageBox.Show("Please select a task to mark as completed.", "No Selection",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Deletes the selected task permanently.
        /// </summary>
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (tasksDataGrid.SelectedItem is DataRowView row)
            {
                var result = MessageBox.Show("Are you sure you want to delete this task permanently?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    int id = Convert.ToInt32(row["id"]);
                    taskManager.DeleteTask(id);
                    RefreshTasks();
                    logger.Log($"Deleted task ID {id}");
                }
            }
            else
            {
                MessageBox.Show("Please select a task to delete.", "No Selection",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ------------------- QUIZ HANDLING -------------------

        /// <summary>
        /// Displays the current quiz question with its options.
        /// Shows final feedback when quiz is finished.
        /// </summary>
        private void ShowQuizQuestion()
        {
            var q = quizEngine.GetCurrentQuestion();
            if (q == null || quizEngine.IsFinished)
            {
                quizQuestionText.Text = "Quiz finished!";
                quizOptionsListBox.Items.Clear();
                quizFeedbackText.Text = quizEngine.GetFinalFeedback();
                // FIXED: Changed questions to Questions (uppercase)
                quizScoreText.Text = $"Score: {quizEngine.Score}/{quizEngine.Questions.Count}";
                return;
            }

            quizQuestionText.Text = q.Text;
            quizOptionsListBox.Items.Clear();
            for (int i = 0; i < q.Options.Count; i++)
            {
                quizOptionsListBox.Items.Add($"{(char)('A' + i)}. {q.Options[i]}");
            }
            quizFeedbackText.Text = "";
            // FIXED: Changed questions to Questions (uppercase)
            quizScoreText.Text = $"Score: {quizEngine.Score}/{quizEngine.Questions.Count}";
            quizOptionsListBox.IsEnabled = true;
            logger.Log($"Quiz question displayed: {q.Text}");
        }

        /// <summary>
        /// Handles option selection in the quiz.
        /// Provides immediate feedback (Correct/Incorrect with explanation).
        /// </summary>
        private void QuizOptionSelected(object sender, SelectionChangedEventArgs e)
        {
            if (quizOptionsListBox.SelectedIndex == -1) return;
            var q = quizEngine.GetCurrentQuestion();
            if (q == null) return;

            int selected = quizOptionsListBox.SelectedIndex;
            bool correct = quizEngine.AnswerCurrent(selected);
            string feedback = correct ? "✅ Correct! " : "❌ Incorrect. ";
            feedback += q.Explanation;
            quizFeedbackText.Text = feedback;
            // FIXED: Changed questions to Questions (uppercase)
            quizScoreText.Text = $"Score: {quizEngine.Score}/{quizEngine.Questions.Count}";
            logger.Log($"Quiz answered: {(correct ? "Correct" : "Incorrect")}");

            // Disable further selection until next question
            quizOptionsListBox.IsEnabled = false;
        }

        /// <summary>
        /// Moves to the next quiz question or shows final results.
        /// </summary>
        private void NextQuizQuestion_Click(object sender, RoutedEventArgs e)
        {
            quizOptionsListBox.IsEnabled = true;
            quizOptionsListBox.SelectedIndex = -1;
            ShowQuizQuestion();
            if (quizEngine.IsFinished)
            {
                logger.Log("Quiz completed");
                // Show final feedback in chat
                chatListBox.Items.Add($"Bot: {quizEngine.GetFinalFeedback()} (Score: {quizEngine.Score}/{quizEngine.Questions.Count})");
                VoiceGreeting.Speak(quizEngine.GetFinalFeedback());
            }
        }

        // ------------------- ACTIVITY LOG -------------------

        /// <summary>
        /// Displays the most recent activity log entries.
        /// </summary>
        private void ShowLog_Click(object sender, RoutedEventArgs e)
        {
            var logs = logger.GetRecentLogs(10);
            logListBox.Items.Clear();
            foreach (var log in logs)
                logListBox.Items.Add(log);
            logger.Log("Activity log viewed via button");
        }
    }
}