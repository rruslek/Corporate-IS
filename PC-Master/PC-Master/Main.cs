using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PC_Master
{
    public partial class Main : Form
    {
        List<string> columns = new List<string>();
        Form authForm;

        int rows = 0;
        public Main(String name, String role, Form Auth)
        {
            InitializeComponent();
            label1.Text = role;
            label2.Text = name;

            authForm = Auth;
        }

        private void tableChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            QueryTable(tableChoice.SelectedItem.ToString());
        }

        private void QueryTable(String table_name)
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            columns.Clear();
            clearTextBoxes();

            MySqlConnection conn = DBConnection.GetDBConnection();
            conn.Open();

            table_name = translateTable(table_name);

            string sql = "Select * from " + table_name;

            MySqlCommand cmd = new MySqlCommand();

            cmd.Connection = conn;
            cmd.CommandText = sql;

            columns = new List<string>();
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    int count = 0;
                    DataTable schema = null;
                    schema = reader.GetSchemaTable();
                    foreach (DataRow col in schema.Rows)
                    {
                        string сname = translateColumn(col.Field<String>("ColumnName"));
                        dataGridView1.Columns.Add(col.Field<String>("ColumnName").ToUpper(), сname);
                        columns.Add(col.Field<String>("ColumnName"));
                        count++;
                    }
                    for (int i = 0; i < count; i++)
                        dataGridView1.Columns[i].Width = (int)(dataGridView1.Width * 1 / count);
                    dataGridView1.Columns[0].Width -= 50;
                    dataGridView1.Columns[1].Width += 50;
                    dataGridView1.Columns[count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    int row = 0;
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add();
                        for (int column = 0; column < count; column++)
                        {
                            string value = Convert.ToString(reader.GetValue(column));
                            dataGridView1.Rows[row].Cells[column].Value = value;
                        }
                        row++;
                    }
                    rows = row;
                }

            }
            conn.Close();
            conn.Dispose();

            showTextBoxes();
        }

        private void addRow(String table_name)
        {
            MySqlConnection conn = DBConnection.GetDBConnection();
            conn.Open();

            table_name = translateTable(table_name);
            int count = 0;
            String clmns = "";
            foreach (string c in columns)
                clmns += c + ", ";
            clmns = clmns.Substring(0, clmns.Length - 2);
            String text = "";
            TextBox[] textBoxes = new TextBox[6] { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6 };
            for (int i = 1; i < columns.Count; i++)
            {
                text = "'" + textBoxes[i].Text + "', ";
            }
            text = text.Substring(0, text.Length - 2);
            string sql = "Insert into " + table_name + " (" + clmns + ") values (" + text + ")";

            MySqlCommand cmd = new MySqlCommand();

            cmd.Connection = conn;
            cmd.CommandText = sql;
            Console.WriteLine(sql);
            int rowCount = cmd.ExecuteNonQuery();
            count += rowCount;
            MessageBox.Show("Строка добавлена!" + count, "Message");
            conn.Close();
            conn.Dispose();

            QueryTable(table_name);
        }

        private string translateTable(String name)
        {
            if (name == "Сотрудники") name = "employee";
            if (name == "Товары") name = "product";
            if (name == "Продажи") name = "sale";
            if (name == "Закупки") name = "supply";
            if (name == "Поставщики") name = "supplier";
            return name;
        }

        private string translateColumn(String name)
        {
            if (name == "employee_id") name = "Номер Сотрудника";
            if (name == "product_id") name = "Номер Товара";
            if (name == "sale_id") name = "Номер Продажи";
            if (name == "supply_id") name = "Номер Закупки";
            if (name == "supplier_id") name = "Номер Поставщика";
            if (name.EndsWith("_name")) name = "Имя";
            if (name.EndsWith("_login")) name = "Логин";
            if (name.EndsWith("_password")) name = "Пароль";
            if (name.EndsWith("_role")) name = "Роль";
            if (name.EndsWith("_date")) name = "Дата";
            if (name.EndsWith("_price")) name = "Цена";
            if (name.EndsWith("_count")) name = "Количество";
            if (name.EndsWith("_type")) name = "Тип";
            return name;
        }

        private void showTextBoxes()
        {
            TextBox[] textBoxes = new TextBox[6] { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6 };
            Label[] labels = new Label[6] { column1, column2, column3, column4, column5, column6 };
            for (int i = 0; i < columns.Count-1; i++)
            {
                textBoxes[i].Visible = true;
                labels[i].Text = translateColumn(columns[i+1]);
            }
        }

        private void clearTextBoxes()
        {
            TextBox[] textBoxes = new TextBox[6] { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6 };
            Label[] labels = new Label[6] { column1, column2, column3, column4, column5, column6 };
            for (int i = 0; i < 6; i++)
            {
                textBoxes[i].Visible = false;
                labels[i].Text = "";
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            addRow(tableChoice.SelectedItem.ToString());
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            authForm.Show();
        }
    }
}
