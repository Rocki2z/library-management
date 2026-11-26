using System;
using System.Linq;
using System.Windows.Forms;
using library_management.model;

namespace PosSystem
{
    public partial class frmAuthor : Form
    {
        private Author selected;
        public frmAuthor() { InitializeComponent(); RefreshGrid(); }
        private void frmAuthor_Load(object sender, EventArgs e) { }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Enter an author name."); return; }
            Run(() => DatabaseService.SaveAuthor(new Author { Name = txtName.Text.Trim(), Biography = txtBiography.Text.Trim() }));
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selected == null) { MessageBox.Show("Select an author first."); return; }
            Run(() => DatabaseService.SaveAuthor(new Author { Id = selected.Id, Name = txtName.Text.Trim(), Biography = txtBiography.Text.Trim() }));
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selected == null) { MessageBox.Show("Select an author first."); return; }
            if (MessageBox.Show("Delete this author? Books using this author prevent deletion.", "Delete author", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            Run(() => DatabaseService.DeleteAuthor(selected.Id));
        }
        private void Run(Action action) { try { action(); RefreshGrid(); ClearForm(); } catch (Exception ex) { MessageBox.Show(ex.Message, "Author", MessageBoxButtons.OK, MessageBoxIcon.Error); } }
        private void dgAuthors_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgAuthors.Rows.Count) return;
            var row = dgAuthors.Rows[e.RowIndex]; var id = Convert.ToString(row.Cells["Id"].Value);
            selected = DatabaseService.Authors().FirstOrDefault(a => a.Id == id);
            if (selected != null) { txtName.Text = selected.Name; txtBiography.Text = selected.Biography; }
        }
        private void RefreshGrid() { dgAuthors.DataSource = DatabaseService.Authors(txtSearch.Text.Trim()).Select(a => new { a.Id, a.Name, a.Biography }).ToList(); }
        private void ClearForm() { txtName.Clear(); txtBiography.Clear(); selected = null; }
        private void txtSearch_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) { RefreshGrid(); e.SuppressKeyPress = true; } }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e) { }
    }
}
