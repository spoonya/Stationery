using Bunifu.Framework.UI;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Stationery
{
    public partial class MainForm : Form
    {
        private string conStr = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog = Stationery; Integrated Security = False; " + 
            "Connect Timeout = 30; Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private SqlConnection con;
        private SqlCommand cmd;
        private SqlDataReader reader;
        private int curPage = 0;
        private List<int> CodeProviderForDelivery = new List<int>();
        private List<int> CodeProductForDeliveries = new List<int>();
        private List<int> CodeProductForAlloc = new List<int>();
        private List<int> CodeStaffForAlloc = new List<int>();
        private Microsoft.Office.Interop.Excel.Application application;
        private Workbook workBook;
        private Worksheet worksheet;

        public MainForm()
        {
            InitializeComponent();
            StaffFill();
            ProductsFill();
            ProvidersFill();
            ProvidersListFill();
            ProductsListFillForDeliveries();
            ProductsListFillForAllocation();
            StaffListFill();
            DeliveriesFill();
            AllocationFill();
        }

        private void Reset()
        {
            Bunifu.UI.WinForm.BunifuShadowPanel.BunifuShadowPanel sp = new Bunifu.UI.WinForm.BunifuShadowPanel.BunifuShadowPanel();
            foreach (Control x in pagesOptions.TabPages[pagesOptions.SelectedIndex].Controls)
            {
                if (x is Bunifu.UI.WinForm.BunifuShadowPanel.BunifuShadowPanel)
                {
                    sp = (Bunifu.UI.WinForm.BunifuShadowPanel.BunifuShadowPanel)x;
                }
            }

            foreach (Control x in sp.Controls)
            {
                if (x is Bunifu.UI.WinForms.BunifuTextbox.BunifuTextBox)
                    ((Bunifu.UI.WinForms.BunifuTextbox.BunifuTextBox)x).Text = string.Empty;
                else if (x is Bunifu.UI.WinForms.BunifuDropdown)
                    ((Bunifu.UI.WinForms.BunifuDropdown)x).SelectedIndex = -1;
            }

            ddAllocProd.Text = "Канцтовар";
            ddAllocStaff.Text = "Сотрудник";
            ddProducts.Text = "Канцтовар";
            ddProductsUpd.Text = "Канцтовар";
            ddProviders.Text = "Поставщик";
            ddProvidersUpd.Text = "Поставщик";
            ddUpdAllocProd.Text = "Канцтовар";
            ddUpdAllocStaff.Text = "Сотрудник";
        }

        /*SELECT
         ===========================*/
        private void StaffFill()
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC StaffFill", con))
            {
                try
                {
                    con.Open();
                    List<string[]> data = new List<string[]>();

                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            data.Add(new string[3]);
                            data[data.Count - 1][0] = reader[0].ToString();
                            data[data.Count - 1][1] = reader[1].ToString();
                            data[data.Count - 1][2] = reader[2].ToString();
                        }

                    foreach (string[] s in data)
                        dgvStaff.Rows.Add(s);
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        private void ProductsFill()
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC ProductsFill", con))
            {
                try
                {
                    con.Open();
                    List<string[]> data = new List<string[]>();

                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            data.Add(new string[3]);
                            data[data.Count - 1][0] = reader[0].ToString();
                            data[data.Count - 1][1] = reader[1].ToString();
                            data[data.Count - 1][2] = reader[2].ToString();
                        }

                    foreach (string[] s in data)
                        dgvProductsSprav.Rows.Add(s);
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        private void ProductsListFillForDeliveries()
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC ProductsListFillForDeliveries", con))
            {         
                if (ddProducts.Items.Count > 0) ddProducts.Items.Clear();
                if (ddProductsUpd.Items.Count > 0) ddProductsUpd.Items.Clear();
                CodeProductForDeliveries.Clear();
                reader = null;
                try
                {
                    con.Open();
                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            CodeProductForDeliveries.Add(new int { });
                            CodeProductForDeliveries[CodeProductForDeliveries.Count - 1] = Convert.ToInt32(reader[1].ToString());
                            ddProducts.Items.Add(reader[0].ToString());
                            ddProductsUpd.Items.Add(reader[0].ToString());
                        }
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        private void ProductsListFillForAllocation()
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC ProductsListFillForAllocation", con))
            {
                if (ddAllocProd.Items.Count > 0) ddAllocProd.Items.Clear();
                if (ddUpdAllocProd.Items.Count > 0) ddUpdAllocProd.Items.Clear();
                CodeProductForAlloc.Clear();
                reader = null;
                try
                {
                    con.Open();
                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            CodeProductForAlloc.Add(new int { });
                            CodeProductForAlloc[CodeProductForAlloc.Count - 1] = Convert.ToInt32(reader[1].ToString());
                            ddAllocProd.Items.Add(reader[0].ToString());
                            ddUpdAllocProd.Items.Add(reader[0].ToString());
                        }
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        private void ProvidersFill()
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC ProvidersFill", con))
            {
                try
                {
                    con.Open();
                    List<string[]> data = new List<string[]>();

                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            data.Add(new string[4]);
                            data[data.Count - 1][0] = reader[0].ToString();
                            data[data.Count - 1][1] = reader[1].ToString();
                            data[data.Count - 1][2] = reader[2].ToString();
                            data[data.Count - 1][3] = reader[3].ToString();
                        }

                    foreach (string[] s in data)
                        dgvProviders.Rows.Add(s);
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        private void ProvidersListFill()
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC ProvidersListFill", con))
            {
                if (ddProviders.Items.Count > 0) ddProviders.Items.Clear();
                if (ddProvidersUpd.Items.Count > 0) ddProvidersUpd.Items.Clear();
                CodeProviderForDelivery.Clear();
                reader = null;
                try
                {
                    con.Open();
                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            CodeProviderForDelivery.Add(new int { });
                            CodeProviderForDelivery[CodeProviderForDelivery.Count - 1] = Convert.ToInt32(reader[1].ToString());
                            ddProviders.Items.Add(reader[0].ToString());
                            ddProvidersUpd.Items.Add(reader[0].ToString());
                        }
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        private void StaffListFill()
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC StaffListFill", con))
            {
                if (ddAllocStaff.Items.Count > 0) ddAllocStaff.Items.Clear();
                if (ddUpdAllocStaff.Items.Count > 0) ddUpdAllocStaff.Items.Clear();
                CodeStaffForAlloc.Clear();
                reader = null;
                try
                {
                    con.Open();
                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            CodeStaffForAlloc.Add(new int { });
                            CodeStaffForAlloc[CodeStaffForAlloc.Count - 1] = Convert.ToInt32(reader[1].ToString());
                            ddAllocStaff.Items.Add(reader[0].ToString());
                            ddUpdAllocStaff.Items.Add(reader[0].ToString());
                        }
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        private string SelectNameProviderFromCode(int code)
        {
            string name = null;
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC SelectNameProviderFromCode @code", con))
            {
                cmd.Parameters.AddWithValue("@code", code);
                try
                {
                    con.Open();

                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            name = reader[0].ToString();
                        }
                    return name;
                }
                catch (SqlException)
                {
                    throw;
                }
            }
        }

        private string SelectNameProductFromCode(int code)
        {
            string name = null;
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC SelectNameProductFromCode @code", con))
            {
                cmd.Parameters.AddWithValue("@code", code);
                try
                {
                    con.Open();

                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            name = reader[0].ToString();
                        }
                    return name;
                }
                catch (SqlException)
                {
                    throw;
                }
            }
        }

        private string SelectNameStaffFromCode(int code)
        {
            string name = null;
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC SelectNameStaffFromCode @code", con))
            {
                cmd.Parameters.AddWithValue("@code", code);
                try
                {
                    con.Open();

                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            name = reader[0].ToString();
                        }
                    return name;
                }
                catch (SqlException)
                {
                    throw;
                }
            }
        }

        private void DeliveriesFill()
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC DeliveriesFill", con))
            {
                try
                {
                    con.Open();
                    List<string[]> data = new List<string[]>();

                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            data.Add(new string[10]);
                            data[data.Count - 1][0] = reader[0].ToString();
                            data[data.Count - 1][1] = reader[1].ToString();
                            data[data.Count - 1][2] = reader[2].ToString();
                            data[data.Count - 1][3] = reader[3].ToString();
                            data[data.Count - 1][5] = reader[4].ToString();
                            data[data.Count - 1][6] = reader[5].ToString();
                            data[data.Count - 1][8] = reader[7].ToString();
                            data[data.Count - 1][9] = reader[8].ToString();

                        }

                    //Получение имени поставщика по коду
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (!(data[i][3]).Equals(""))
                            data[i][4] = SelectNameProviderFromCode(Convert.ToInt32(data[i][3]));
                        else data[i][4] = "Не установлен";
                    }

                    //Получение названия канцтовара по коду
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (!(data[i][6]).Equals(""))
                            data[i][7] = SelectNameProductFromCode(Convert.ToInt32(data[i][6]));
                        else data[i][7] = "Не установлен";
                    }

                    foreach (string[] s in data)
                        dgvDeliveries.Rows.Add(s);
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        private void AllocationFill()
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC AllocationFill", con))
            {
                try
                {
                    con.Open();
                    List<string[]> data = new List<string[]>();

                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            data.Add(new string[7]);
                            data[data.Count - 1][0] = reader[0].ToString();
                            data[data.Count - 1][1] = reader[1].ToString();
                            data[data.Count - 1][3] = reader[2].ToString();
                            data[data.Count - 1][5] = reader[3].ToString();
                            data[data.Count - 1][6] = reader[4].ToString();
                        }

                    //Получение названия канцтовара по коду
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (!(data[i][1]).Equals(""))
                            data[i][2] = SelectNameStaffFromCode(Convert.ToInt32(data[i][1]));
                        else data[i][2] = "Не установлен";
                    }

                    //Получение имени сорудника по коду
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (!(data[i][3]).Equals(""))
                            data[i][4] = SelectNameProductFromCode(Convert.ToInt32(data[i][3]));
                        else data[i][4] = "Не установлен";
                    }

                    foreach (string[] s in data)
                        dgvAlloc.Rows.Add(s);
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        private int SelectCountProd(int code)
        {
            int count = 0;
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC CountCheck @code", con))
            {
                cmd.Parameters.AddWithValue("@code", code);
                try
                {
                    con.Open();

                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            count = Convert.ToInt32(reader[0].ToString());
                        }
                    return count;
                }
                catch (SqlException)
                {
                    throw;
                }
            }
        }

        /*INSERT
         ===========================*/
        private void StaffInsert()
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC StaffInsert @name, @phone", con))
            {
                if (!tbNameStaff.Text.Equals("") && !tbPhoneStaff.Text.Equals(""))
                {
                    cmd.Parameters.AddWithValue("@name", tbNameStaff.Text);
                    cmd.Parameters.AddWithValue("@phone", tbPhoneStaff.Text);
                }

                try
                {
                    con.Open();

                    cmd.ExecuteNonQuery();
                    dgvStaff.Rows.Clear();
                    StaffFill();
                    Reset();
                    MessageBox.Show("Запись добавлена", "Уведомление", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                }
                catch (SqlException)
                {
                    MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void ProductsInsert()
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC ProductsInsert @name, @unit", con))
            {
                if (!tbProdSpravName.Text.Equals("") && !tbProdSpravPhone.Text.Equals(""))
                {
                    cmd.Parameters.AddWithValue("@name", tbProdSpravName.Text);
                    cmd.Parameters.AddWithValue("@unit", tbProdSpravPhone.Text);
                }

                try
                {
                    con.Open();

                    cmd.ExecuteNonQuery();
                    dgvProductsSprav.Rows.Clear();
                    ProductsFill();
                    Reset();
                    MessageBox.Show("Запись добавлена", "Уведомление", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                }
                catch (SqlException)
                {
                    //throw;
                    MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void ProvidersInsert()
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC ProvidersInsert @name, @phone, @address", con))
            {
                if (!tbProviderName.Text.Equals("") && !tbProviderPhone.Text.Equals("") && !tbProviderAddress.Text.Equals(""))
                {
                    cmd.Parameters.AddWithValue("@name", tbProviderName.Text);
                    cmd.Parameters.AddWithValue("@phone", tbProviderPhone.Text);
                    cmd.Parameters.AddWithValue("@address", tbProviderAddress.Text);
                }

                try
                {
                    con.Open();

                    cmd.ExecuteNonQuery();
                    dgvProviders.Rows.Clear();
                    ProvidersFill();
                    Reset();
                    MessageBox.Show("Запись добавлена", "Уведомление", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                }
                catch (SqlException)
                {
                    //throw;
                    MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void DeliveriesInsert()
        {
            int lastAddedDelivery = 0;

            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC DeliveriesInsert @date, @ttn, @id_provider", con))
            {
                if (!dateDelivery.Text.Equals("") && !tbTtnDelivery.Text.Equals("") && ddProviders.SelectedIndex != -1
                    && !tbCountProductsInfo.Text.Equals("") && !tbPriceProductsInfo.Text.Equals("") &&
                    ddProducts.SelectedIndex != -1)
                {
                    DateTime date = Convert.ToDateTime(dateDelivery.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@ttn", tbTtnDelivery.Text);
                    cmd.Parameters.AddWithValue("@id_provider", CodeProviderForDelivery[ddProviders.SelectedIndex]);        

                    try
                    {
                        con.Open();

                        lastAddedDelivery = Convert.ToInt32(cmd.ExecuteScalar());
                        ProductsInfoInsert(lastAddedDelivery);
                        dgvDeliveries.Rows.Clear();
                        DeliveriesFill();
                        Reset();
                        MessageBox.Show("Запись добавлена", "Уведомление", MessageBoxButtons.OK,
                        MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    }
                    catch (SqlException)
                    {
                        //throw;
                        MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ProductsInfoInsert(int lastAddedDelivery)
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC ProductsInfoInsert @id_product, @id_delivery, @count, @price", con))
            {
                cmd.Parameters.AddWithValue("@id_product", CodeProductForDeliveries[ddProducts.SelectedIndex]);
                cmd.Parameters.AddWithValue("@id_delivery", lastAddedDelivery);
                cmd.Parameters.AddWithValue("@count", tbCountProductsInfo.Text);
                cmd.Parameters.AddWithValue("@price", tbPriceProductsInfo.Text);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        private void AllocationInsert()
        {
            int count = 0;
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC AllocationInsert @staff, @product, @count, @date", con))
            {
                if (!tbAllocCount.Text.Equals("") && ddAllocProd.SelectedIndex != -1 && ddAllocStaff.SelectedIndex != -1)
                {
                    cmd.Parameters.AddWithValue("@staff", CodeStaffForAlloc[ddAllocStaff.SelectedIndex]);
                    cmd.Parameters.AddWithValue("@product", CodeProductForAlloc[ddAllocProd.SelectedIndex]);
                    cmd.Parameters.AddWithValue("@count", tbAllocCount.Text);
                    DateTime date = Convert.ToDateTime(dateAlloc.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@date", date);
                }

                try
                {
                    con.Open();
                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            count = Convert.ToInt32(reader[0].ToString());
                        }

                    if (count >= 0 && Convert.ToInt32(tbAllocCount.Text) > 0)
                    {
                        dgvAlloc.Rows.Clear();
                        AllocationFill();
                        Reset();
                        MessageBox.Show("Запись добавлена", "Уведомление", MessageBoxButtons.OK,
                        MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    }
                    else
                        MessageBox.Show("Недопустимое количество", "Ошибка", MessageBoxButtons.OK,
                        MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                }
                catch (SqlException)
                {
                    //MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    throw;
                }
            }
        }

        /*UPDATE
         ===========================*/
        private void StaffUpdate(int code)
        {
            int curRow = 0;
            if (dgvStaff.SelectedRows.Count > 0)
                curRow = dgvStaff.SelectedRows[0].Index;

            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC StaffUpdate @code, @name, @phone", con))
            {
                if (!tbUpdNameStaff.Text.Equals("") && !tbUpdPhoneStaff.Text.Equals(""))
                {
                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@name", tbUpdNameStaff.Text);
                    cmd.Parameters.AddWithValue("@phone", tbUpdPhoneStaff.Text);
                }

                try
                {
                    con.Open();

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    dgvStaff.Rows.Clear();
                    StaffFill();
                    dgvStaff.ClearSelection();
                    dgvStaff.Rows[curRow].Selected = true;
                    dgvStaff.CurrentCell = dgvStaff[dgvStaff.ColumnCount - 1, curRow];

                    MessageBox.Show("Редактирование успешно выполнено", "Уведомление", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                }
                catch (SqlException)
                {
                    //throw;
                    MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void ProductsUpdate(int code)
        {
            int curRow = 0;
            if (dgvProductsSprav.SelectedRows.Count > 0)
                curRow = dgvProductsSprav.SelectedRows[0].Index;

            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC ProductsUpdate @code, @name, @phone", con))
            {
                if (!tbUpdNameProdSprav.Text.Equals("") && !tbUpdUnitProdSprav.Text.Equals(""))
                {
                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@name", tbUpdNameProdSprav.Text);
                    cmd.Parameters.AddWithValue("@phone", tbUpdUnitProdSprav.Text);
                }

                try
                {
                    con.Open();

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    dgvProductsSprav.Rows.Clear();
                    ProductsFill();
                    dgvProductsSprav.ClearSelection();
                    dgvProductsSprav.Rows[curRow].Selected = true;
                    dgvProductsSprav.CurrentCell = dgvProductsSprav[dgvProductsSprav.ColumnCount - 1, curRow];

                    MessageBox.Show("Редактирование успешно выполнено", "Уведомление", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                }
                catch (SqlException)
                {
                    //throw;
                    MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void ProvidersUpdate(int code)
        {
            int curRow = 0;
            if (dgvProviders.SelectedRows.Count > 0)
                curRow = dgvProviders.SelectedRows[0].Index;

            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC ProvidersUpdate @code, @name, @phone, @address", con))
            {
                if (!tbUpdProviderName.Text.Equals("") && !tbUpdProviderPhone.Text.Equals("") && 
                    !tbUpdProviderAddress.Text.Equals(""))
                {
                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@name", tbUpdProviderName.Text);
                    cmd.Parameters.AddWithValue("@phone", tbUpdProviderPhone.Text);
                    cmd.Parameters.AddWithValue("@address", tbUpdProviderAddress.Text);
                }

                try
                {
                    con.Open();

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    dgvProviders.Rows.Clear();
                    ProvidersFill();
                    dgvProviders.ClearSelection();
                    dgvProviders.Rows[curRow].Selected = true;
                    dgvProviders.CurrentCell = dgvProviders[dgvProviders.ColumnCount - 1, curRow];

                    MessageBox.Show("Редактирование успешно выполнено", "Уведомление", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                }
                catch (SqlException)
                {
                    //throw;
                    MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void DeliveriesUpdate(int code)
        {
            int curRow = 0;
            if (dgvDeliveries.SelectedRows.Count > 0)
                curRow = dgvDeliveries.SelectedRows[0].Index;

            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC DeliveryUpdate @code, @date, @ttn, @provider", con))
            {
                if (!dateUpdDelivery.Text.Equals("") && !tbUpdTtnDelivery.Text.Equals("") 
                    && !tbUpdCountProductsInfo.Text.Equals("") && !tbUpdPriceProductsInfo.Text.Equals(""))
                {
                    cmd.Parameters.AddWithValue("@code", code);
                    DateTime date = Convert.ToDateTime(dateUpdDelivery.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@ttn", tbUpdTtnDelivery.Text);
                    if (ddProvidersUpd.SelectedIndex != -1)
                        cmd.Parameters.AddWithValue("@provider", CodeProviderForDelivery[ddProvidersUpd.SelectedIndex]);
                    else
                        cmd.Parameters.AddWithValue("@provider", dgvDeliveries[3, curRow].Value.ToString());
                }

                try
                {
                    con.Open();

                    cmd.ExecuteNonQuery();                   

                }
                catch (SqlException)
                {
                    //throw;
                    MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void ProductsInfoUpdate(int code)
        {
            int curRow = 0, count = 0;
            if (dgvDeliveries.SelectedRows.Count > 0)
                curRow = dgvDeliveries.SelectedRows[0].Index;

            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC ProductsInfoUpdate @code, @product, @count, @price", con))
            {
                cmd.Parameters.AddWithValue("@code", code);
                if (ddProductsUpd.SelectedIndex != -1)
                    cmd.Parameters.AddWithValue("@product", CodeProductForDeliveries[ddProductsUpd.SelectedIndex]);
                else
                    cmd.Parameters.AddWithValue("@product", dgvDeliveries[6, curRow].Value.ToString());
                cmd.Parameters.AddWithValue("@count", tbUpdCountProductsInfo.Text);
                cmd.Parameters.AddWithValue("@price", tbUpdPriceProductsInfo.Text);

                try
                {
                    con.Open();
                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            count = Convert.ToInt32(reader[0].ToString());
                        }

                    if (count >= 0)
                    {
                        DeliveriesUpdate(code);
                        cmd.Parameters.Clear();
                        dgvDeliveries.Rows.Clear();
                        DeliveriesFill();
                        dgvDeliveries.ClearSelection();
                        dgvDeliveries.Rows[curRow].Selected = true;
                        dgvDeliveries.CurrentCell = dgvDeliveries[dgvDeliveries.ColumnCount - 1, curRow];

                        MessageBox.Show("Редактирование успешно выполнено", "Уведомление", MessageBoxButtons.OK,
                        MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    }
                    else
                    {
                        MessageBox.Show("Недопустимое количество", "Ошибка", MessageBoxButtons.OK,
                        MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);                      
                    }
                }
            
                catch (SqlException)
                {
                    //throw;
                    MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        private void AllocationUpdate(int code)
        {
            int curRow = 0, count = 0;
            if (dgvAlloc.SelectedRows.Count > 0)
                curRow = dgvAlloc.SelectedRows[0].Index;

            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC AllocationUpdate @code, @staff, @product, @count, @oldCount, @date", con))
            {
                if (!dateAllocUpd.Text.Equals("") && !tbUpdAllocCount.Text.Equals(""))
                {
                    cmd.Parameters.AddWithValue("@code", code);
                    DateTime date = Convert.ToDateTime(dateAllocUpd.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@count", tbUpdAllocCount.Text);
                    cmd.Parameters.AddWithValue("@oldCount", dgvAlloc[5, curRow].Value.ToString());
                    if (ddUpdAllocProd.SelectedIndex != -1)
                        cmd.Parameters.AddWithValue("@product", CodeProductForDeliveries[ddUpdAllocProd.SelectedIndex]);
                    else
                        cmd.Parameters.AddWithValue("@product", dgvAlloc[3, curRow].Value.ToString());
                    if (ddUpdAllocStaff.SelectedIndex != -1)
                        cmd.Parameters.AddWithValue("@staff", CodeStaffForAlloc[ddUpdAllocStaff.SelectedIndex]);
                    else
                        cmd.Parameters.AddWithValue("@staff", dgvAlloc[1, curRow].Value.ToString());
                }

                try
                {
                    con.Open();
                    using (reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            count = Convert.ToInt32(reader[0].ToString());
                        }

                    if (count >= 0 && Convert.ToInt32(tbUpdAllocCount.Text) > 0)
                    {
                        dgvAlloc.Rows.Clear();
                        AllocationFill();                                          
                        dgvAlloc.ClearSelection();
                        dgvAlloc.Rows[curRow].Selected = true;
                        dgvAlloc.CurrentCell = dgvAlloc[dgvAlloc.ColumnCount - 1, curRow];
                        Reset();
                        MessageBox.Show("Редактирование успешно выполнено", "Уведомление", MessageBoxButtons.OK,
                        MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                    }
                    else
                        MessageBox.Show("Недопустимое количество", "Ошибка", MessageBoxButtons.OK,
                        MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);              
                }
                catch (SqlException)
                {
                    //throw;
                    MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /*DELETE
         ===========================*/
        private int[] DeleteRows(DataGridView dgv)
        {
            int[] deletedRows = new int[dgv.SelectedRows.Count];
            int i = 0;

            foreach (DataGridViewRow item in dgv.SelectedRows)
            {
                deletedRows[i++] = Convert.ToInt32(dgv[0, item.Index].Value.ToString());
                dgv.Rows.RemoveAt(item.Index);
            }
            return deletedRows;
        }

        private void StaffDelete(int[] deletedRows)
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC StaffDelete @code", con))
            {
                try
                {
                    con.Open();

                    for (int i = 0; i < deletedRows.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@code", Convert.ToInt32(deletedRows[i]));
                        cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                    }
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        private void ProductsDelete(int[] deletedRows)
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC ProductsDelete @code", con))
            {
                try
                {
                    con.Open();

                    for (int i = 0; i < deletedRows.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@code", Convert.ToInt32(deletedRows[i]));
                        cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                    }
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        private void ProvidersDelete(int[] deletedRows)
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC ProvidersDelete @code", con))
            {
                try
                {
                    con.Open();

                    for (int i = 0; i < deletedRows.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@code", Convert.ToInt32(deletedRows[i]));
                        cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                    }
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        private void DeliveriesDelete(int[] deletedRows)
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC DeliveryDelete @code", con))
            {
                try
                {
                    con.Open();

                    for (int i = 0; i < deletedRows.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@code", Convert.ToInt32(deletedRows[i]));
                        cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                    }
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        private void AllocationDelete(int[] deletedRows)
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC AllocationDelete @code", con))
            {
                try
                {
                    con.Open();

                    for (int i = 0; i < deletedRows.Length; i++)
                    {
                        cmd.Parameters.AddWithValue("@code", Convert.ToInt32(deletedRows[i]));
                        cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                    }
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        /*SEARCH
         ===========================*/
        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            int n = pages.SelectedIndex;
            switch (n)
            {
                case 0: ProductsSearch(); break;
                case 1: StaffSearch(); break;
                case 2: ProvidersSearch(); break;
                case 3: DeliveriesSearch(); break;
                case 4: AllocationSearch(); break;
            }
        }

        private void StaffSearch()
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC StaffSearch @name", con))
            {
                cmd.Parameters.AddWithValue("@name", tbSearch.Text);

                try
                {
                    dgvStaff.Rows.Clear();

                    if (tbSearch.Text.Equals(""))
                        StaffFill();
                    else
                    {
                        con.Open();

                        List<string[]> data = new List<string[]>();

                        using (reader = cmd.ExecuteReader())
                            while (reader.Read())
                            {
                                data.Add(new string[3]);

                                data[data.Count - 1][0] = reader[0].ToString();
                                data[data.Count - 1][1] = reader[1].ToString();
                                data[data.Count - 1][2] = reader[2].ToString();
                            }
                        foreach (string[] s in data)
                            dgvStaff.Rows.Add(s);
                    }
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        private void ProductsSearch()
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC ProductsSearch @name", con))
            {
                cmd.Parameters.AddWithValue("@name", tbSearch.Text);

                try
                {
                    dgvProductsSprav.Rows.Clear();

                    if (tbSearch.Text.Equals(""))
                        ProductsFill();
                    else
                    {
                        con.Open();

                        List<string[]> data = new List<string[]>();

                        using (reader = cmd.ExecuteReader())
                            while (reader.Read())
                            {
                                data.Add(new string[3]);

                                data[data.Count - 1][0] = reader[0].ToString();
                                data[data.Count - 1][1] = reader[1].ToString();
                                data[data.Count - 1][2] = reader[2].ToString();
                            }
                        foreach (string[] s in data)
                            dgvProductsSprav.Rows.Add(s);
                    }
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        private void ProvidersSearch()
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC ProvidersSearch @name", con))
            {
                cmd.Parameters.AddWithValue("@name", tbSearch.Text);

                try
                {
                    dgvProviders.Rows.Clear();

                    if (tbSearch.Text.Equals(""))
                        ProvidersFill();
                    else
                    {
                        con.Open();

                        List<string[]> data = new List<string[]>();

                        using (reader = cmd.ExecuteReader())
                            while (reader.Read())
                            {
                                data.Add(new string[4]);

                                data[data.Count - 1][0] = reader[0].ToString();
                                data[data.Count - 1][1] = reader[1].ToString();
                                data[data.Count - 1][2] = reader[2].ToString();
                                data[data.Count - 1][3] = reader[3].ToString();
                            }
                        foreach (string[] s in data)
                            dgvProviders.Rows.Add(s);
                    }
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        private void DeliveriesSearch()
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC DeliveriesSearch @str", con))
            {
                cmd.Parameters.AddWithValue("@str", tbSearch.Text);

                try
                {
                    dgvDeliveries.Rows.Clear();

                    if (tbSearch.Text.Equals(""))
                        DeliveriesFill();
                    else
                    {
                        con.Open();

                        List<string[]> data = new List<string[]>();

                        using (reader = cmd.ExecuteReader())
                            while (reader.Read())
                            {
                                data.Add(new string[10]);
                                data[data.Count - 1][0] = reader[6].ToString();
                                data[data.Count - 1][1] = reader[0].ToString();
                                data[data.Count - 1][2] = reader[1].ToString();
                                data[data.Count - 1][3] = reader[2].ToString();
                                data[data.Count - 1][5] = reader[7].ToString();
                                data[data.Count - 1][6] = reader[3].ToString();
                                data[data.Count - 1][8] = reader[4].ToString();
                                data[data.Count - 1][9] = reader[5].ToString();
                            }

                        //Получение имени поставщика по коду
                        for (int i = 0; i < data.Count; i++)
                        {
                            if (!(data[i][3]).Equals(""))
                                data[i][4] = SelectNameProviderFromCode(Convert.ToInt32(data[i][3]));
                            else data[i][4] = "Не установлен";
                        }

                        //Получение названия канцтовара по коду
                        for (int i = 0; i < data.Count; i++)
                        {
                            if (!(data[i][6]).Equals(""))
                                data[i][7] = SelectNameProductFromCode(Convert.ToInt32(data[i][6]));
                            else data[i][7] = "Не установлен";
                        }

                        foreach (string[] s in data)
                            dgvDeliveries.Rows.Add(s);
                    }
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        private void AllocationSearch()
        {
            using (con = new SqlConnection(conStr))
            using (cmd = new SqlCommand("EXEC AllocationSearch @str", con))
            {
                cmd.Parameters.AddWithValue("@str", tbSearch.Text);

                try
                {
                    dgvAlloc.Rows.Clear();

                    if (tbSearch.Text.Equals(""))
                        AllocationFill();
                    else
                    {
                        con.Open();

                        List<string[]> data = new List<string[]>();

                        using (reader = cmd.ExecuteReader())
                            while (reader.Read())
                            {
                                data.Add(new string[7]);
                                data[data.Count - 1][0] = reader[0].ToString();
                                data[data.Count - 1][1] = reader[1].ToString();
                                data[data.Count - 1][3] = reader[2].ToString();
                                data[data.Count - 1][5] = reader[3].ToString();
                                data[data.Count - 1][6] = reader[4].ToString();
                            }

                        //Получение названия канцтовара по коду
                        for (int i = 0; i < data.Count; i++)
                        {
                            if (!(data[i][1]).Equals(""))
                                data[i][2] = SelectNameStaffFromCode(Convert.ToInt32(data[i][1]));
                            else data[i][2] = "Не установлен";
                        }

                        //Получение имени сорудника по коду
                        for (int i = 0; i < data.Count; i++)
                        {
                            if (!(data[i][3]).Equals(""))
                                data[i][4] = SelectNameProductFromCode(Convert.ToInt32(data[i][3]));
                            else data[i][4] = "Не установлен";
                        }

                        foreach (string[] s in data)
                            dgvAlloc.Rows.Add(s);
                    }
                }
                catch (SqlException)
                {
                    //throw;
                }
            }
        }

        /*Функциональное меню
         =======================*/
        private void insertRecord_Click(object sender, EventArgs e)
        {
            curPage = pagesOptions.SelectedIndex;
            int n = pages.SelectedIndex;
            switch (n)
            {
                case 0: pagesOptions.SetPage("Доб товара_справ"); break;
                case 1: pagesOptions.SetPage("Доб сотр"); break;
                case 2: pagesOptions.SetPage("Доб пост"); break;
                case 3: pagesOptions.SetPage("Доб поставки"); break;
                case 4: pagesOptions.SetPage("Доб выд"); break;
            }
            OpenTrans();
        }

        private void updateRecord_Click(object sender, EventArgs e)
        {
            curPage = pagesOptions.SelectedIndex;
            int n = pages.SelectedIndex;
            switch (n)
            {
                case 0: pagesOptions.SetPage("Ред товара_справ"); break;
                case 1: pagesOptions.SetPage("Ред сотр"); break;
                case 2: pagesOptions.SetPage("Ред пост"); break;
                case 3: pagesOptions.SetPage("Ред поставки"); break;
                case 4: pagesOptions.SetPage("Ред выд"); break;
            }
            OpenTrans();
        }

        private void deleteRecord_Click(object sender, EventArgs e)
        {
            bool success = false;
            if (MessageBox.Show("Удалить запись(и)?", "Удаление", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                int n = pages.SelectedIndex;
                switch (n)
                {
                    case 0:
                        if (dgvProductsSprav.RowCount > 0)
                        {
                            ProductsDelete(DeleteRows(dgvProductsSprav));
                            dgvDeliveries.Rows.Clear();
                            DeliveriesFill();
                            dgvAlloc.Rows.Clear();
                            AllocationFill();
                            ProductsListFillForDeliveries();
                            ProductsListFillForAllocation();
                            success = true;
                        }
                        break;
                    case 1:
                        if (dgvStaff.RowCount > 0)
                        {
                            StaffDelete(DeleteRows(dgvStaff));
                            dgvAlloc.Rows.Clear();
                            AllocationFill();
                            StaffListFill();
                            success = true;
                        }
                        break;
                    case 2:
                        if (dgvProviders.RowCount > 0)
                        {
                            ProvidersDelete(DeleteRows(dgvProviders));
                            dgvDeliveries.Rows.Clear();
                            DeliveriesFill();
                            success = true;
                        }
                        break;
                    case 3:
                        if (dgvDeliveries.RowCount > 0)
                        {
                            DeliveriesDelete(DeleteRows(dgvDeliveries));
                            dgvAlloc.Rows.Clear();
                            AllocationFill();
                            ProductsListFillForAllocation();
                            success = true;
                        }
                        break;
                    case 4:
                        if (dgvAlloc.RowCount > 0)
                        {
                            AllocationDelete(DeleteRows(dgvAlloc));
                            success = true;
                        }
                        break;
                }
                if (success)
                    MessageBox.Show("Удаление успешно выполнено", "Уведомление", MessageBoxButtons.OK,
                        MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                else
                    MessageBox.Show("Строка не выбрана", "Уведомление", MessageBoxButtons.OK,
                        MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                success = false;
            }
        }

        private void searchRecord_Click(object sender, EventArgs e)
        {
            curPage = pagesOptions.SelectedIndex;
            pagesOptions.SetPage("Поиск");
            OpenTrans();
        }

        /*Кнопки вызова процедур и функций
         =======================*/
        private void btnAddStaff_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);
            StaffInsert();
            StaffListFill();
        }

        private void btnProdSpravAdd_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);
            ProductsInsert();
            ProductsListFillForDeliveries();
        }

        private void btnProviderAdd_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);
            ProvidersInsert();
            ProvidersListFill();
        }

        private void btnDeliveryAdd_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);
            DeliveriesInsert();
            ProductsListFillForAllocation();
        }

        private void btnAllocAdd_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);
            AllocationInsert();         
        }

        private void btnAllocEdit_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);

            int curRow = 0;

            if (dgvAlloc.RowCount > 0 && dgvAlloc.SelectedRows.Count > 0)
            {
                curRow = dgvAlloc.SelectedRows[0].Index;

                AllocationUpdate(Convert.ToInt32(dgvAlloc[0, curRow].Value.ToString()));

                //dgvAllocation.Rows.Clear();
                //AllocationFill();
            }
            else MessageBox.Show("Строка не выбрана!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnProdSpravEdit_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);

            int curRow = 0;

            if (dgvProductsSprav.RowCount > 0 && dgvProductsSprav.SelectedRows.Count > 0)
            {
                curRow = dgvProductsSprav.SelectedRows[0].Index;

                ProductsUpdate(Convert.ToInt32(dgvProductsSprav[0, curRow].Value.ToString()));

                dgvDeliveries.Rows.Clear();
                DeliveriesFill();
                dgvAlloc.Rows.Clear();
                AllocationFill();
                ProductsListFillForDeliveries();
            }
            else MessageBox.Show("Строка не выбрана!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnProvidersUpd_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);

            int curRow = 0;

            if (dgvProviders.RowCount > 0 && dgvProviders.SelectedRows.Count > 0)
            {
                curRow = dgvProviders.SelectedRows[0].Index;

                ProvidersUpdate(Convert.ToInt32(dgvProviders[0, curRow].Value.ToString()));

                dgvDeliveries.Rows.Clear();
                DeliveriesFill();
                dgvDeliveries.Rows.Clear();
                DeliveriesFill();
                ProvidersListFill();
            }
            else MessageBox.Show("Строка не выбрана!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnUpdDelivery_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);

            int curRow = 0;

            if (dgvDeliveries.RowCount > 0 && dgvDeliveries.SelectedRows.Count > 0)
            {
                curRow = dgvDeliveries.SelectedRows[0].Index;

                ProductsInfoUpdate(Convert.ToInt32(dgvDeliveries[0, curRow].Value.ToString()));

                ProductsListFillForAllocation();
            }
            else MessageBox.Show("Строка не выбрана!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnAlloc_Click(object sender, EventArgs e)
        {
            button_Click(sender, e);
            pages.SetPage("Выдача");
        }

        private void btnUpdStaff_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);

            int curRow = 0;

            if (dgvStaff.RowCount > 0 && dgvStaff.SelectedRows.Count > 0)
            {
                curRow = dgvStaff.SelectedRows[0].Index;

                StaffUpdate(Convert.ToInt32(dgvStaff[0, curRow].Value.ToString()));

                dgvAlloc.Rows.Clear();
                AllocationFill();
                StaffListFill();
            }
            else MessageBox.Show("Строка не выбрана!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnDeliveries_Click(object sender, EventArgs e)
        {
            button_Click(sender, e);
            pages.SetPage("Поставки");
        }

        private void btnProviders_Click(object sender, EventArgs e)
        {
            button_Click(sender, e);
            pages.SetPage("Поставщики");
        }

        /*Навигационное меню
         ==========================*/
        private void btnMinMenu_Click(object sender, EventArgs e)
        {
            pnlMenu.Visible = false;
            logo.Visible = false;
            pnlMenu.Width = 53;
            transMenu.ShowSync(pnlMenu);
            btnMaxMenu.Visible = true;
            btnMinMenu.Visible = false;
        }

        private void btnMaxMenu_Click(object sender, EventArgs e)
        {
            btnMaxMenu.Visible = false;
            pnlMenu.Visible = false;
            logo.Visible = true;
            pnlMenu.Width = 192;
            transMenu.ShowSync(pnlMenu);
            btnMinMenu.Visible = true;
        }

        private void btnProducts_sprav_Click(object sender, EventArgs e)
        {
            button_Click(sender, e);
            pages.SetPage("Канцтовары_справочник");
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            button_Click(sender, e);
            pages.SetPage("Канцтовары");
        }

        private void btnStaff_Click(object sender, EventArgs e)
        {
            button_Click(sender, e);
            pages.SetPage("Сотрудники");
        }

        /*Побочные функции
         =======================*/
        private void button_Click(object sender, EventArgs e)
        {
            pnlSelector.Visible = false;
            pnlSelector.Top = ((Control)sender).Top;
            pnlSelector.Height = ((Control)sender).Height;
            transButton.ShowSync(pnlSelector);

            pagesOptions.Visible = false;
        }

        private void transColorButton_Click(object sender, EventArgs e)
        {
            ((BunifuFlatButton)sender).Visible = false;
            transColorBtn.ShowSync(((BunifuFlatButton)sender));
        }

        private void OpenTrans()
        {
            if (pagesOptions.Visible == true && pagesOptions.SelectedIndex == curPage)
            {
                pagesOptions.Visible = false;
            }
            else
            {
                transOptions.ShowSync(pagesOptions);
            }
        }

        private void dgvStaff_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            int curRow = 0;

            if (dgvStaff.SelectedRows.Count > 0)
                curRow = dgvStaff.SelectedRows[0].Index;

            tbUpdNameStaff.Text = dgvStaff[1, curRow].Value.ToString();
            tbUpdPhoneStaff.Text = dgvStaff[2, curRow].Value.ToString();
        }

        private void dgvProductsSprav_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            int curRow = 0;

            if (dgvProductsSprav.SelectedRows.Count > 0)
                curRow = dgvProductsSprav.SelectedRows[0].Index;

            tbUpdNameProdSprav.Text = dgvProductsSprav[1, curRow].Value.ToString();
            tbUpdUnitProdSprav.Text = dgvProductsSprav[2, curRow].Value.ToString();
        }

        private void dgvProviders_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            int curRow = 0;

            if (dgvProviders.SelectedRows.Count > 0)
                curRow = dgvProviders.SelectedRows[0].Index;

            tbUpdProviderName.Text = dgvProviders[1, curRow].Value.ToString();
            tbUpdProviderPhone.Text = dgvProviders[2, curRow].Value.ToString();
            tbUpdProviderAddress.Text = dgvProviders[3, curRow].Value.ToString();
        }

        private void dgvDeliveries_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            int curRow = 0;

            if (dgvDeliveries.SelectedRows.Count > 0)
                curRow = dgvDeliveries.SelectedRows[0].Index;

            dateUpdDelivery.Text = dgvDeliveries[1, curRow].Value.ToString();
            tbUpdTtnDelivery.Text = dgvDeliveries[2, curRow].Value.ToString(); 
            ddProvidersUpd.Text = dgvDeliveries[4, curRow].Value.ToString();
            ddProductsUpd.Text = dgvDeliveries[7, curRow].Value.ToString();
            tbUpdPriceProductsInfo.Text = dgvDeliveries[9, curRow].Value.ToString();
            tbUpdCountProductsInfo.Text = dgvDeliveries[8, curRow].Value.ToString();          
        }

        private void dgvAlloc_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            int curRow = 0, code = 0;

            if (dgvAlloc.SelectedRows.Count > 0)
                curRow = dgvAlloc.SelectedRows[0].Index;

            ddUpdAllocStaff.Text = dgvAlloc[2, curRow].Value.ToString();
            ddUpdAllocProd.Text = dgvAlloc[4, curRow].Value.ToString();
            tbUpdAllocCount.Text = dgvAlloc[5, curRow].Value.ToString();
            dateAllocUpd.Text = dgvAlloc[6, curRow].Value.ToString();
            code = Convert.ToInt32(dgvAlloc[3, curRow].Value.ToString());
            tbAllocFreeUpd.Text = SelectCountProd(code).ToString();
        }

        private void отчётToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvAlloc.RowCount > 0 && dgvAlloc.SelectedRows.Count > 0)
            {
                SqlDataReader localReader = null;
                int curRow = 0, i = 24;
                curRow = dgvAlloc.SelectedRows[0].Index;
                application = new Microsoft.Office.Interop.Excel.Application
                {
                    DisplayAlerts = false
                };
                const string template = "Report1.xlsx";
                workBook = application.Workbooks.Open(Path.Combine(Environment.CurrentDirectory, template));
                worksheet = workBook.ActiveSheet as Worksheet;


                using (con = new SqlConnection(conStr))
                using (cmd = new SqlCommand("EXEC Statement @staff, @date", con))
                {
                    cmd.Parameters.AddWithValue("@staff", dgvAlloc[1, curRow].Value.ToString());
                    DateTime date = Convert.ToDateTime(dgvAlloc[6, curRow].Value.ToString());
                    cmd.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));

                    try
                    {
                        con.Open();

                        using (localReader = cmd.ExecuteReader())
                            while (localReader.Read())
                            {
                                worksheet.Range["C17"].Value = SelectNameStaffFromCode(Convert.ToInt32(localReader[0].ToString()));
                                worksheet.Range["A" + i].Value = SelectNameProductFromCode(Convert.ToInt32(localReader[1].ToString()));
                                worksheet.Range["E" + i].Value = localReader[2].ToString();
                                worksheet.Range["F" + i].Value = localReader[3].ToString();
                                worksheet.Range["G" + i].Value = localReader[4].ToString();
                                worksheet.Range["A12"].Value = localReader[5].ToString();

                                //Range range = worksheet.get_Range(worksheet.Cells[24, 1], worksheet.Cells[i, 5]);
                                //range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
                                //range.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
                                //range.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlContinuous;
                                //range.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
                                //range.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;

                                i++;
                            }                       
                    }
                    catch (SqlException)
                    {
                        //throw;
                    }                 
                }

                using (con = new SqlConnection(conStr))
                using (cmd = new SqlCommand("EXEC StatementTotal @staff, @date", con))
                {
                    int under = i;
                    under--;
                    cmd.Parameters.AddWithValue("@staff", dgvAlloc[1, curRow].Value.ToString());
                    DateTime date = Convert.ToDateTime(dgvAlloc[6, curRow].Value.ToString());
                    cmd.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));

                    try
                    {
                        con.Open();

                        using (reader = cmd.ExecuteReader())
                            while (reader.Read())
                            {
                                worksheet.Range["A" + i].Value = "Итого";
                                worksheet.Range["E" + i].Formula = reader[0].ToString();//"=СУММ(E24:" + "E" + under + ")";
                                worksheet.Range["F" + i].Formula = reader[1].ToString();//"=СУММ(F24:" + "F" + under + ")";
                                worksheet.Range["G" + i].Formula = reader[2].ToString();//"=СУММ(G24:" + "G" + under + ")";
                            }

                        application.Visible = true;
                        TopMost = true;
                    }
                    catch (SqlException)
                    {
                        //throw;
                    }
                }
            }
            else MessageBox.Show("Строка не выбрана!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ddAllocProd_SelectedValueChanged(object sender, EventArgs e)
        {   
            if (ddAllocProd.SelectedIndex != -1)
                tbAllocFree.Text = SelectCountProd(CodeProductForAlloc[ddAllocProd.SelectedIndex]).ToString();
        }

        private void ddUpdAllocProd_SelectedValueChanged(object sender, EventArgs e)
        {
            int curRow = 0, code = 0;

            if (dgvAlloc.SelectedRows.Count > 0)
                curRow = dgvAlloc.SelectedRows[0].Index;

            if (ddUpdAllocProd.SelectedIndex != -1)
            {
                code = Convert.ToInt32(dgvAlloc[3, curRow].Value.ToString());
                tbAllocFreeUpd.Text = SelectCountProd(CodeProductForAlloc[ddUpdAllocProd.SelectedIndex]).ToString();
            }
        }
    }
}
