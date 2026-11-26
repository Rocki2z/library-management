using System;
using System.Windows.Forms;
using library_management.model;
using library_management.View;

namespace PosSystem
{
    public partial class LogInForm : Form
    {
        private readonly User currentUser;
        public LogInForm() : this(null) { }
        public LogInForm(User user)
        {
            InitializeComponent(); currentUser=user;
            if(user!=null){lblName.Text=user.Username;lblRole.Text=user.Role;}
            var loans = new Button { Text="Loans / Members", ForeColor=System.Drawing.Color.White, FlatStyle=FlatStyle.Flat, Width=234, Height=35, Left=3, Top=289, BackColor=System.Drawing.Color.FromArgb(37,38,38) };
            loans.Click += (s,e)=>OpenChild(new frmLoans()); panel2.Controls.Add(loans); loans.BringToFront();
        }
        private void Form1_Load(object sender, EventArgs e) { }
        private void OpenChild(Form child) { while(panel3.Controls.Count>0){var old=panel3.Controls[0];panel3.Controls.RemoveAt(0);old.Dispose();} child.TopLevel=false; child.FormBorderStyle=FormBorderStyle.None; child.Dock=DockStyle.Fill; panel3.Controls.Add(child); child.BringToFront(); child.Show(); }
        private void button4_Click(object sender, EventArgs e) { OpenChild(new frmCategory()); }
        private void btnProduct_Click(object sender, EventArgs e) { OpenChild(new frmBook()); }
        private void btnVendor_Click(object sender, EventArgs e) { OpenChild(new frmAuthor()); }
        private void btnClose_Click(object sender, EventArgs e) { Close(); }
        private void button9_Click(object sender, EventArgs e) { Close(); }
        private void lblRole_Click(object sender, EventArgs e) { }
        private void lblName_Click(object sender, EventArgs e) { }
        private void panel3_Paint(object sender, PaintEventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void btnBrand_Click(object sender, EventArgs e) { }
        private void btnStock_Click(object sender, EventArgs e) { }
        private void button8_Click(object sender, EventArgs e) { }
        private void btnSalesHistory_Click(object sender, EventArgs e) { }
        private void button6_Click(object sender, EventArgs e) { }
        private void button7_Click(object sender, EventArgs e) { }
        private void button2_Click(object sender, EventArgs e) { }
    }
}
