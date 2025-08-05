using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;


namespace Student_Manager
{
    public partial class Login : Form
    {
        private readonly string connectionString = @"Server=DESKTOP-7SQ7EUU\SQLEXPRESS;Database=StudentManager;Trusted_Connection=True;";
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsename.Text.Trim();
            string password = txtPassword.Text;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT COUNT(*) FROM Users WHERE Username = @Username and PasswordHash = HASHBYTES('SHA2_256', CONVERT(VARBINARY, @Password))";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                int result = (int)cmd.ExecuteScalar();
                if (result == 0)
                {
                    this.Hide();
                    Form1 dashboard = new Form1();
                    dashboard.Show();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
