using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace ChatBotPOE
{
    /// <summary>
    /// DatabaseHelper handles all database operations for cybersecurity tasks.
    /// Supports CRUD (Create, Read, Update, Delete) operations on the tasks table.
    /// Uses MySQL Connector to communicate with the database.
    /// </summary>
    public class DatabaseHelper
    {
        private string connectionString = "server=localhost;database=cyberbot_db;uid=root;pwd=Lethabo-2026;";

        /// <summary>
        /// Creates a new task in the database.
        /// Stores title, description, and optional reminder date.
        /// Returns true if successful, false if an error occurs.
        /// </summary>
        public bool AddTask(string title, string description, DateTime? reminder = null)
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO tasks (title, description, reminder_date) 
                                     VALUES (@title, @desc, @reminder)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@desc", description);
                        cmd.Parameters.AddWithValue("@reminder", (object?)reminder ?? DBNull.Value);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Database error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retrieves all tasks from the database.
        /// Returns a DataTable containing all task records.
        /// Used to populate the DataGrid on the Tasks tab.
        /// </summary>
        public DataTable GetAllTasks()
        {
            DataTable dt = new DataTable();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id, title, description, reminder_date, is_completed FROM tasks ORDER BY created_at DESC";
                using (var cmd = new MySqlCommand(query, conn))
                using (var da = new MySqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        /// <summary>
        /// Marks a specific task as completed.
        /// Updates the is_completed column in the database.
        /// Returns true if successful.
        /// </summary>
        public bool CompleteTask(int taskId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE tasks SET is_completed = TRUE WHERE id = @id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", taskId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Permanently removes a task from the database.
        /// Returns true if the deletion was successful.
        /// </summary>
        public bool DeleteTask(int taskId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM tasks WHERE id = @id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", taskId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
