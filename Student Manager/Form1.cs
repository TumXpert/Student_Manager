using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;


namespace Student_Manager
{
    public partial class Form1 : Form
    {
        private List<Student> students = new List<Student>();
        private int selectedStudentId = -1;
        private readonly string connectionString = @"Server=DESKTOP-7SQ7EUU\SQLEXPRESS;Database=StudentManager;Trusted_Connection=True;";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadStudentsFromDatabase();
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }
        private void label1_Click(object sender, EventArgs e)
        {
            Student student = new Student();
        }

        private void Add_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO Students (FirstName, LastName, Age, BirthDate, Grade)
                                    VALUES (@FirstName, @LastName, @Age, @BirthDate, @Grade)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@Age", int.Parse(intAge.Text));
                    cmd.Parameters.AddWithValue("@BirthDate", dtDob.Value);
                    cmd.Parameters.AddWithValue("@Grade", txtGrade.Text);
                    cmd.ExecuteNonQuery();
                };

                MessageBox.Show("Student Added to the Database");
                LoadStudentsFromDatabase();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
        private void dvgStudents_SelectionChanged(object sender, EventArgs e)
        {
            btnUpdate.Enabled = true;
            btnDelete.Enabled = true;
            if (dgvStudents.CurrentRow != null && dgvStudents.CurrentRow.DataBoundItem is Student s) 
            {
                selectedStudentId = s.Id;
                txtFirstName.Text = s.FirstName;
                txtLastName.Text = s.LastName;
                intAge.Text = s.Age.ToString();
                dtDob.Value = s.BirthDate;
                txtGrade.Text = s.Grade;

            }
        }

        public void LoadStudentsFromDatabase()
        {
            students.Clear();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Students";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    students.Add(new Student
                    {
                        Id = Convert.ToInt32(reader["ID"]),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Age = Convert.ToInt32(reader["Age"]),
                        BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                        Grade = reader["Grade"].ToString()

                    });
                }
                reader.Close();
            }
            dgvStudents.DataSource = null;
            dgvStudents.DataSource = students;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedStudentId > 0 )
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = @"UPDATE Students SET FirstName = @FirstName, LastName = @LastName, Age = @Age, BirthDate = @BirthDate, Grade = @Grade WHERE Id = @Id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                        cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                        cmd.Parameters.AddWithValue("@Age", int.Parse(intAge.Text));
                        cmd.Parameters.AddWithValue("@BirthDate", dtDob.Value);
                        cmd.Parameters.AddWithValue("@Grade", txtGrade.Text);
                        cmd.Parameters.AddWithValue("@Id", selectedStudentId);

                        cmd.ExecuteNonQuery();


                    }
                    LoadStudentsFromDatabase();
                    MessageBox.Show("Student updated successfully");

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Updating student: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please Select a Student to Update");
            }
        }

        
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedStudentId > 0)
            {
                var confirm = MessageBox.Show("Are you sure you want to delete this student?", "Confirm", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM Students WHERE Id = @Id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", selectedStudentId);
                        cmd.ExecuteNonQuery ();
                    }
                    selectedStudentId = -1;
                    LoadStudentsFromDatabase ();
                    MessageBox.Show("Student deleted");
                }
            }
        }

        private void RefreshGrid()
        {
            dgvStudents.DataSource = null;
            dgvStudents.DataSource = students;
        }

        private void SaveToFile()
        {
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string query = txtSearch.Text.ToLower();

            var filtered = students.FindAll(s =>
                s.FirstName.ToLower().Contains(query) ||
                s.LastName.ToLower().Contains(query));

            dgvStudents.DataSource = null;
            dgvStudents.DataSource = filtered;
        }

    }
}
