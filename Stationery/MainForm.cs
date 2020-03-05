﻿using Bunifu.Framework.UI;
using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Collections.Generic;

namespace Stationery
{
    public partial class MainForm : Form
    {
        private string conStr = @"Data Source=|DataDirectory|\Stationery.db;Version=3";
        private SQLiteConnection con;
        private SQLiteCommand cmd;
        private SQLiteDataReader reader;
        private int curPage = 0;

        public MainForm()
        {
            InitializeComponent();
            StaffFill();
        }

        /*INSERT
         ===========================*/
        private void StaffFill()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT * FROM Staff", con))
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
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

        private void InsertStaff()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("INSERT into Staff(name_staff, phone_staff) VALUES (@name, @phone)", con))
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
                    //Reset();
                    MessageBox.Show("Запись добавлена", "Уведомление", MessageBoxButtons.OK,
                    MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                }
                catch (SQLiteException)
                {
                    MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /*UPDATE
         ===========================*/
        private void UpdateStaff(int code)
        {
            int curRow = 0;
            if (dgvStaff.SelectedRows.Count > 0)
                curRow = dgvStaff.SelectedRows[0].Index;

            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("UPDATE Staff SET name_staff = @name, phone_staff = @phone " +
                "WHERE id_staff = @code", con))
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
                catch (SQLiteException)
                {
                    throw;
                    //MessageBox.Show("Заполните все данные!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /*DELETE
         ===========================*/
        private void DeleteStaff(int[] deletedRows)
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("PRAGMA foreign_keys = ON; DELETE FROM Staff WHERE id_staff = @code;", con))
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
                catch (SQLiteException)
                {
                    throw;
                }
            }
        }

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

        /*SEARCH
         ===========================*/
        private void SearchStaff()
        {
            using (con = new SQLiteConnection(conStr))
            using (cmd = new SQLiteCommand("SELECT * FROM Staff WHERE name_staff LIKE @name", con))
            {
                cmd.Parameters.AddWithValue("@name", '%' + tbSearch.Text + '%');

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
                catch (SQLiteException)
                {
                    throw;
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
                case 0: pagesOptions.SetPage(""); break;
                case 1: pagesOptions.SetPage("Доб сотр"); break;
                case 2: pagesOptions.SetPage(""); break;
                case 3: pagesOptions.SetPage(""); break;
                case 4: pagesOptions.SetPage(""); break;
            }
            OpenTrans();
        }

        private void updateRecord_Click(object sender, EventArgs e)
        {
            curPage = pagesOptions.SelectedIndex;
            int n = pages.SelectedIndex;
            switch (n)
            {
                case 0: pagesOptions.SetPage(""); break;
                case 1: pagesOptions.SetPage("Ред сотр"); break;
                case 2: pagesOptions.SetPage(""); break;
                case 3: pagesOptions.SetPage(""); break;
                case 4: pagesOptions.SetPage(""); break;
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
                    //case 0:
                    //    if (dgvEquip.RowCount > 0)
                    //    {
                    //        DeleteEquip(DeleteRows(dgvEquip));
                    //        dgvAllocation.Rows.Clear();
                    //        AllocationFill();
                    //        dgvCancellation.Rows.Clear();
                    //        CancellationFill();
                    //        success = true;
                    //    }
                    //    break;
                    case 1:
                        if (dgvStaff.RowCount > 0)
                        {
                            DeleteStaff(DeleteRows(dgvStaff));
                            //dgvAllocation.Rows.Clear();
                            //dgvProducts.Rows.Clear();
                            //EquipmentFill();
                            //AllocationFill();
                            success = true;
                        }
                        break;
                        //case 2:
                        //    if (dgvAllocation.RowCount > 0)
                        //    {
                        //        DeleteAllocation(DeleteRows(dgvAllocation));
                        //        dgvEquip.Rows.Clear();
                        //        EquipmentFill();
                        //        success = true;
                        //    }
                        //    break;
                        //case 3:
                        //    if (dgvCancellation.RowCount > 0)
                        //    {
                        //        DeleteCancellation(DeleteRows(dgvCancellation));
                        //        dgvEquip.Rows.Clear();
                        //        EquipmentFill();
                        //        success = true;
                        //    }
                        //    break;
                        //case 4:
                        //    if (dgvProviders.RowCount > 0)
                        //    {
                        //        DeleteProvider(DeleteRows(dgvProviders));
                        //        dgvEquip.Rows.Clear();
                        //        EquipmentFill();
                        //        success = true;
                        //    }
                        //    break;
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

        /*Кнопки выполнения операций
         =======================*/
        private void btnAddStaff_Click(object sender, EventArgs e)
        {
            transColorButton_Click(sender, e);
            InsertStaff();
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

                UpdateStaff(Convert.ToInt32(dgvStaff[0, curRow].Value.ToString()));

                //dgvAllocation.Rows.Clear();
                //AllocationFill();
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
        private void dgvStaff_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            int curRow = 0;

            if (dgvStaff.SelectedRows.Count > 0)
                curRow = dgvStaff.SelectedRows[0].Index;

            tbUpdNameStaff.Text = dgvStaff[1, curRow].Value.ToString();
            tbUpdPhoneStaff.Text = dgvStaff[2, curRow].Value.ToString();
        }

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
    }
}
