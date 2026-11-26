using System;
using System.Linq;
using System.Windows.Forms;
using library_management.model;

namespace PosSystem
{
    public partial class frmBook : Form
    {
        private Book selected;
        private readonly NumericUpDown numCopies = new NumericUpDown();
        public frmBook() { InitializeComponent(); var copiesLabel = new Label { Text="Copies", Left=57, Top=252, Width=100 }; numCopies.SetBounds(217,249,100,26); numCopies.Minimum=1; numCopies.Maximum=10000; numCopies.Value=1; Controls.Add(copiesLabel); Controls.Add(numCopies); txtSearchBy.TextChanged += (s,e) => RefreshGrid(); }
        private void frmProduct_Load(object sender, EventArgs e) { LoadAuthors(); LoadCategories(); RefreshGrid(); }
        private void LoadAuthors() { var authors=DatabaseService.Authors(); cmbAuthorName.DisplayMember="Name"; cmbAuthorName.ValueMember="Id"; cmbAuthorName.DataSource=authors; cmbAuthorName.SelectedIndex=-1; }
        private void LoadCategories() { var categories=DatabaseService.Categories(); cmbCategory.DisplayMember="Name"; cmbCategory.ValueMember="Id"; cmbCategory.DataSource=categories; cmbCategory.SelectedIndex=-1; cmbDescription.DropDownStyle=ComboBoxStyle.DropDown; cmbDescription.DataSource=null; cmbDescription.Items.AddRange(categories.Select(c=>c.Description).Where(d=>!String.IsNullOrWhiteSpace(d)).Distinct().Cast<object>().ToArray()); }
        private Book ReadForm() { if(cmbCategory.SelectedValue==null || cmbAuthorName.SelectedValue==null) throw new InvalidOperationException("Choose a category and an author."); return new Book { Title=txtTittle.Text.Trim(), Description=cmbDescription.Text.Trim(), Category=Convert.ToString(cmbCategory.SelectedValue), AuthorId=Convert.ToString(cmbAuthorName.SelectedValue), PublishDate=dtpPublishDate.Value.ToString("yyyy-MM-dd"), Copies=(int)numCopies.Value }; }
        private void btnSave_Click(object sender, EventArgs e) { if(String.IsNullOrWhiteSpace(txtTittle.Text)){MessageBox.Show("Enter a book title.");return;} Run(()=>DatabaseService.SaveBook(ReadForm())); }
        private void btnUpdate_Click(object sender, EventArgs e) { if(selected==null){MessageBox.Show("Select a book first.");return;} if(String.IsNullOrWhiteSpace(txtTittle.Text)){MessageBox.Show("Enter a book title.");return;} Run(()=>{var b=ReadForm();b.Id=selected.Id;DatabaseService.SaveBook(b);}); }
        private void btnDelete_Click(object sender, EventArgs e) { if(selected==null){MessageBox.Show("Select a book first.");return;} if(MessageBox.Show("Delete this book? Books with loan history cannot be deleted.","Delete book",MessageBoxButtons.YesNo)!=DialogResult.Yes)return; Run(()=>DatabaseService.DeleteBook(selected.Id)); }
        private void Run(Action a){try{a();RefreshGrid();ClearForm();}catch(Exception ex){MessageBox.Show(ex.Message,"Book",MessageBoxButtons.OK,MessageBoxIcon.Error);}}
        private void RefreshGrid(){try{dgBooks.DataSource=DatabaseService.Books(txtSearchBy.Text.Trim()).Select(b=>new{b.Id,b.Title,b.Description,b.AuthorName,b.CategoryName,b.PublishDate,b.Copies,b.AvailableCopies}).ToList();}catch(Exception ex){MessageBox.Show(ex.Message,"Load books",MessageBoxButtons.OK,MessageBoxIcon.Error);}}
        private void ClearForm(){txtTittle.Clear();cmbCategory.SelectedIndex=-1;cmbAuthorName.SelectedIndex=-1;cmbDescription.Text="";dtpPublishDate.Value=DateTime.Today;numCopies.Value=1;selected=null;}
        private void dgBooks_CellClick(object sender,DataGridViewCellEventArgs e){if(e.RowIndex<0||e.RowIndex>=dgBooks.Rows.Count)return;var id=Convert.ToString(dgBooks.Rows[e.RowIndex].Cells["Id"].Value);selected=DatabaseService.Books().FirstOrDefault(b=>b.Id==id);if(selected==null)return;txtTittle.Text=selected.Title;cmbCategory.SelectedValue=selected.Category;cmbAuthorName.SelectedValue=selected.AuthorId;cmbDescription.Text=selected.Description;numCopies.Value=Math.Max(numCopies.Minimum,Math.Min(numCopies.Maximum,selected.Copies));DateTime date;if(DateTime.TryParse(selected.PublishDate,out date))dtpPublishDate.Value=date;}
        private void txtSearchBy_KeyDown(object sender,KeyEventArgs e){if(e.KeyCode==Keys.Enter){RefreshGrid();e.SuppressKeyPress=true;}}
        private void btnClearSearch_Click(object sender,EventArgs e){txtSearchBy.Clear();RefreshGrid();}
        private void textBox1_TextChanged(object sender,EventArgs e){}
        private void txtTittle_TextChanged(object sender,EventArgs e){}
        private void cmbAuthorName_SelectedIndexChanged(object sender,EventArgs e){}
    }
}
