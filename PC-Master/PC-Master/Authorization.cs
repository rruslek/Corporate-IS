using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PC_Master
{
    public partial class Authorization : Form
    {

        String name = "";
        String role = "";

        public Authorization()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (loginInput.Text != "" && passwordInput.Text != "")
            {
                bool userExists = checkPassword();
                if (userExists)
                {
                    loginInput.Text = "";
                    passwordInput.Text = "";
                    this.Hide();
                    Main mainForm = new Main(name, role, this);
                    mainForm.Show();
                }
                else
                {
                    MessageBox.Show("Вы ввели неверные данные!",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.DefaultDesktopOnly);
                }
            }
            else
            {
                MessageBox.Show("Введите логин и пароль!",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private bool checkPassword()
        {
            MySqlConnection conn = DBConnection.GetDBConnection();
            conn.Open();
            string sql = "Select * from employee WHERE employee_login='"+loginInput.Text+"' AND employee_password='"+passwordInput.Text+"'";

            MySqlCommand cmd = new MySqlCommand();

            cmd.Connection = conn;
            cmd.CommandText = sql;

            using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        name = Convert.ToString(reader.GetValue(1));
                        role = Convert.ToString(reader.GetValue(4));
                    }
                }

            }
            Console.WriteLine(name);
            conn.Close();
            conn.Dispose();
            if (name == "") return false;
            else return true;
        }


        private bool checkPassword1()
        {
            MySqlConnection conn = DBConnection.GetDBConnection();
            conn.Open();

            String sql = "SELECT COUNT(*) FROM employee WHERE (employee_login = '" + loginInput.Text + "' AND employee_password = '" + passwordInput.Text + "')";
            MySqlCommand cmd = new MySqlCommand();

            cmd.Connection = conn;
            cmd.CommandText = sql;
            object obj = cmd.ExecuteScalar();
            if (Convert.ToInt32(obj) > 0)
            {
                conn.Close();
                conn.Dispose();
                return true;
            }
            else
            {
                conn.Close();
                conn.Dispose();
                return false;
            }

            
        }
    }
}
