using System;
using System.Linq;
using System.Windows.Forms;
using library_management.model;

namespace PosSystem
{
    public partial class frmCategory : Form
    {
        private Category selected;
        private readonly Button deleteButton = new Button();
        public frmCategory()
        {
            InitializeComponent();
            deleteButton.Text = "Delete"; deleteButton.SetBounds(500, 190, 100, 37); deleteButton.Click += btnDelete_Click; Controls.Add(deleteButton);
            txtSearchBy.TextChanged += (s, e) => RefreshGrid();
            RefreshGrid();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Enter a category name."); return; }
            Run(() => DatabaseService.SaveCategory(new Category { Name = txtName.Text.Trim(), Description = txtDescription.Text.Trim() }));
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selected == null) { MessageBox.Show("Select a category first."); return; }
            Run(() => DatabaseService.SaveCategory(new Category { Id = selected.Id, Name = txtName.Text.Trim(), Description = txtDescription.Text.Trim() }));
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selected == null) { MessageBox.Show("Select a category first."); return; }
            if (MessageBox.Show("Delete this category? Books using it prevent deletion.", "Delete category", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            Run(() => DatabaseService.DeleteCategory(selected.Id));
        }
        private void Run(Action action) { try { action(); RefreshGrid(); ClearForm(); } catch (Exception ex) { MessageBox.Show(ex.Message, "Category", MessageBoxButtons.OK, MessageBoxIcon.Error); } }
        private void dgCategories_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgCategories.Rows.Count) return;
            var id = Convert.ToString(dgCategories.Rows[e.RowIndex].Cells["Id"].Value);
            selected = DatabaseService.Categories().FirstOrDefault(c => c.Id == id);
            if (selected != null) { txtName.Text = selected.Name; txtDescription.Text = selected.Description; }
        }
        private void RefreshGrid() { dgCategories.DataSource = DatabaseService.Categories(txtSearchBy.Text.Trim()).Select(c => new { c.Id, c.Name, c.Description }).ToList(); }
        private void ClearForm() { txtName.Clear(); txtDescription.Clear(); selected = null; }
        private void button3_Click(object sender, EventArgs e) { ClearForm(); }
        private void pictureBox2_Click(object sender, EventArgs e) { Close(); }
        private void frmProduct_Load(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e) { }
        private void button1_Click(object sender, EventArgs e) { }
        private void button2_Click(object sender, EventArgs e) { }
    }
}
