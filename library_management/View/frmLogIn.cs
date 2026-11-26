using System;
using System.Windows.Forms;
using library_management.model;
using library_management.VIew;
using PosSystem;

namespace library_management.View
{
    public partial class frmLogIn : Form
    {
        public frmLogIn()
        {
            InitializeComponent();
            label2.Cursor = Cursors.Hand; label2.Click += register_Click;
            btncancel.Click += (s,e) => Close();
            AcceptButton = btnLogIn;
        }
        private void register_Click(object sender, EventArgs e) { using(var form=new RegisterForm()) form.ShowDialog(this); }
        private void btnLogIn_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrWhiteSpace(txtUsername.Text)||String.IsNullOrEmpty(txtPassword.Text)){MessageBox.Show("Enter your username and password.");return;}
            try
            {
                var user=DatabaseService.Login(txtUsername.Text,txtPassword.Text);
                if(user==null){MessageBox.Show("Invalid username or password.");return;}
                Hide();
                using (var main = new LogInForm(user)) main.ShowDialog();
                txtPassword.Clear(); Show(); Activate();
            }
            catch(Exception ex){MessageBox.Show("Could not connect to PostgreSQL. Check the connection settings and try again.\n\n"+ex.Message,"Database connection",MessageBoxButtons.OK,MessageBoxIcon.Error);}
        }
    }
}
