using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace PC_Master
{
    public partial class Main : Form
    {
        List<string> columns = new List<string>();
        Form authForm;
        int itemId = 0;

        int rows = 0;
        public Main(String name, String role, Form Auth)
        {
            InitializeComponent();
            label1.Text = role;
            label2.Text = name;
            authForm = Auth;
            int tableCount = tableChoice.Items.Count;
            if (role != "Администратор")
            {
                for (int i = 4; i < tableCount; i++)
                {
                    tableChoice.Items.Remove(tableChoice.Items[tableChoice.Items.Count-1]);
                }
                tableChoice.Items.Remove(tableChoice.Items[0]);
            }
        }

        private void tableChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tableChoice.SelectedIndex < 4)
            {
                dataGridView1.Height = 253;
                queryTable(tableChoice.SelectedItem.ToString());
            }
            if (tableChoice.SelectedIndex > 6)
            {
                hideControl();
                dataGridView1.Height = 427;
                queryReport(tableChoice.SelectedItem.ToString());
            }
            
        }

        private void queryTable(String table_name)
        {
            itemId = 0;
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
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
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
                            string value = "";
                            if (reader.GetValue(column).GetType() == typeof(DateTime))
                            {
                                DateTime time = (DateTime) reader.GetValue(column);
                                value = time.ToString("yyyy-MM-dd");
                            }
                            else value = Convert.ToString(reader.GetValue(column));
                            dataGridView1.Rows[row].Cells[column].Value = value;
                        }
                        row++;
                    }
                    rows = row;
                }

            }
            conn.Close();
            conn.Dispose();

            showControl();
        }

        private void queryReport(String report_name)
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            columns.Clear();
            clearTextBoxes();
            MySqlConnection conn = DBConnection.GetDBConnection();
            conn.Open();

            report_name = translateReport(report_name);

            string sql = "Select * from " + report_name;

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
                        string сname = col.Field<String>("ColumnName");
                        dataGridView1.Columns.Add(col.Field<String>("ColumnName").ToUpper(), сname);
                        count++;
                    }
                    for (int i = 0; i < count; i++)
                        dataGridView1.Columns[i].Width = (int)(dataGridView1.Width * 1 / count);
                    if (report_name == "sales_report" || report_name == "employees_report")
                        dataGridView1.Columns[count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    int row = 0;
                    while (reader.Read())
                    {
                        dataGridView1.Rows.Add();
                        for (int column = 0; column < count; column++)
                        {
                            string value = "";
                            if (reader.GetValue(column).GetType() == typeof(DateTime))
                            {
                                DateTime time = (DateTime)reader.GetValue(column);
                                value = time.ToString("yyyy-MM-dd");
                            }
                            else value = Convert.ToString(reader.GetValue(column));
                            dataGridView1.Rows[row].Cells[column].Value = value;
                        }
                        row++;
                    }
                    rows = row;
                }

            }
            conn.Close();
            conn.Dispose();

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

        private string translateReport(String name)
        {
            if (name == "По продажам") name = "sales_report";
            if (name == "По закупкам") name = "supplies_report";
            if (name == "По товарам") name = "products_report";
            if (name == "По прибыли") name = "profits_report";
            if (name == "По сотрудникам") name = "employees_report";
            return name;
        }

        private void showControl()
        {
            TextBox[] textBoxes = new TextBox[6] { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6 };
            Label[] labels = new Label[6] { column1, column2, column3, column4, column5, column6 };
            for (int i = 0; i < columns.Count-1; i++)
            {
                textBoxes[i].Visible = true;
                labels[i].Visible = true;
                labels[i].Text = translateColumn(columns[i+1]);
            }
            btnDelete.Visible = true;
            btnAdd.Visible = true;
            btnChange.Visible = true;
        }

        private void hideControl()
        {
            TextBox[] textBoxes = new TextBox[6] { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6 };
            Label[] labels = new Label[6] { column1, column2, column3, column4, column5, column6 };
            for (int i = 0; i < 6; i++)
            {
                textBoxes[i].Visible = false;
                textBoxes[i].Text = "";
                labels[i].Visible = false;
                labels[i].Text = "";
            }
            btnDelete.Visible = false;
            btnAdd.Visible = false;
            btnChange.Visible = false;
        }

        private void clearTextBoxes()
        {
            TextBox[] textBoxes = new TextBox[6] { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6 };
            Label[] labels = new Label[6] { column1, column2, column3, column4, column5, column6 };
            for (int i = 0; i < 6; i++)
            {
                textBoxes[i].Visible = false;
                textBoxes[i].Text = "";
                labels[i].Text = "";
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Int32.TryParse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString(), out itemId);
            TextBox[] textBoxes = new TextBox[6] { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6 };
            for (int i = 0; i < columns.Count-1; i++)
                textBoxes[i].Text = dataGridView1.Rows[e.RowIndex].Cells[i+1].Value.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = DBConnection.GetDBConnection();
            conn.Open();

            String table_name = translateTable(tableChoice.SelectedItem.ToString());
            String clmns = "";

            for (int i = 1; i < columns.Count; i++)
            {
                clmns += columns[i] + ", ";
            }
            clmns = clmns.Substring(0, clmns.Length - 2);
            String text = "";
            TextBox[] textBoxes = new TextBox[6] { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6 };
            for (int i = 0; i < columns.Count - 1; i++)
            {
                text += "'" + textBoxes[i].Text + "', ";
            }
            text = text.Substring(0, text.Length - 2);
            string sql = "Insert into " + table_name + " (" + clmns + ") values (" + text + ")";

            MySqlCommand cmd = new MySqlCommand();

            cmd.Connection = conn;
            cmd.CommandText = sql;
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Строка добавлена!", "Message");
            }
            catch (Exception)
            {
                MessageBox.Show("Проверьте введенные данные", "Ошибка");
            }
            conn.Close();
            conn.Dispose();

            queryTable(table_name);
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (itemId != 0)
            {
                String table_name = translateTable(tableChoice.SelectedItem.ToString());

                MySqlConnection conn = DBConnection.GetDBConnection();
                conn.Open();

                String text = "";
                TextBox[] textBoxes = new TextBox[6] { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6 };
                for (int i = 1; i < columns.Count; i++)
                {
                    text += columns[i] + "='" + textBoxes[i - 1].Text + "', ";
                }
                text = text.Substring(0, text.Length - 2);
                string sql = "UPDATE " + table_name + " SET " + text +
                    " WHERE " + columns[0] + "=" + itemId;

                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = conn;
                cmd.CommandText = sql;
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Строка изменена!", "Message");
                }
                catch (Exception)
                {
                    MessageBox.Show("Проверьте введенные данные", "Ошибка");
                }
                conn.Close();

                queryTable(table_name);
            } else MessageBox.Show("Ни одна строка не выбрана!", "Ошибка");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (itemId != 0)
            {
                String table_name = translateTable(tableChoice.SelectedItem.ToString());

                MySqlConnection conn = DBConnection.GetDBConnection();
                conn.Open();

                string sql = "DELETE FROM " + table_name + " WHERE " + columns[0] + "=" + itemId;

                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = conn;
                cmd.CommandText = sql;
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Строка удалена!", "Message");
                }
                catch (Exception)
                {
                    MessageBox.Show("Проверьте введенные данные", "Ошибка");
                }
                conn.Close();

                queryTable(table_name);
            } else MessageBox.Show("Ни одна строка не выбрана!", "Ошибка");
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            Excel.Application ExcelApp = new Excel.Application();
            Excel.Workbook ExcelWorkBook;
            Excel.Worksheet ExcelWorkSheet;
            ExcelWorkBook = ExcelApp.Workbooks.Add(System.Reflection.Missing.Value);
            ExcelWorkSheet = (Excel.Worksheet)ExcelWorkBook.Worksheets.get_Item(1);
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    ExcelApp.Cells[1, j + 1] = dataGridView1.Columns[j].HeaderText.ToString();
                    ExcelApp.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value;
                }
            }
            ExcelApp.Visible = true;
            ExcelApp.UserControl = true;
            ExcelApp.Rows.AutoFit();
            ExcelApp.Columns.AutoFit();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
            authForm.Show();
        }
    }
}
