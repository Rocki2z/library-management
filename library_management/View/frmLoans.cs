using System;
using System.Linq;
using System.Windows.Forms;
using library_management.model;

namespace PosSystem
{
    /// <summary>Member registration and circulation screen.</summary>
    public sealed class frmLoans : Form
    {
        private readonly ComboBox books = new ComboBox();
        private readonly ComboBox members = new ComboBox();
        private readonly TextBox name = new TextBox();
        private readonly TextBox email = new TextBox();
        private readonly TextBox phone = new TextBox();
        private readonly DateTimePicker due = new DateTimePicker();
        private readonly DataGridView grid = new DataGridView();
        public frmLoans()
        {
            Text="Loans and members"; BackColor=System.Drawing.Color.White; AutoScroll=true;
            books.FormattingEnabled=true;
            books.Format += (s,e) => { var b=e.ListItem as Book; if(b!=null)e.Value=b.Title+" ("+b.AvailableCopies+" available)"; };
            AddLabel("Loans and members",20,15,300,30,14);
            AddLabel("New member",20,62,120,24,10); AddLabel("Name",20,96,70,24,9); name.SetBounds(95,95,180,25); Controls.Add(name);
            AddLabel("Email",290,96,60,24,9); email.SetBounds(345,95,200,25); Controls.Add(email);
            AddLabel("Phone",555,96,55,24,9); phone.SetBounds(610,95,160,25); Controls.Add(phone);
            AddButton("Add member",785,92,120,32,AddMember);
            AddLabel("Book",20,147,70,24,9); books.SetBounds(95,145,300,27); books.DropDownStyle=ComboBoxStyle.DropDownList; Controls.Add(books);
            AddLabel("Member",415,147,70,24,9); members.SetBounds(490,145,280,27); members.DropDownStyle=ComboBoxStyle.DropDownList; Controls.Add(members);
            AddLabel("Due date",20,190,70,24,9); due.SetBounds(95,188,160,27); due.Value=DateTime.Today.AddDays(14); Controls.Add(due);
            AddButton("Check out",280,185,120,34,Checkout); AddButton("Return selected",415,185,150,34,Return);
            AddButton("Refresh",580,185,100,34,RefreshButton);
            grid.SetBounds(20,235,900,370); grid.ReadOnly=true; grid.AllowUserToAddRows=false; grid.SelectionMode=DataGridViewSelectionMode.FullRowSelect; grid.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill; Controls.Add(grid);
            Load += (s,e)=>RefreshData();
        }
        private void AddLabel(string text,int x,int y,int w,int h,float size){var l=new Label{Text=text,Left=x,Top=y,Width=w,Height=h,Font=new System.Drawing.Font("Segoe UI",size)};Controls.Add(l);}
        private void AddButton(string text,int x,int y,int w,int h,EventHandler click){var b=new Button{Text=text,Left=x,Top=y,Width=w,Height=h};b.Click+=click;Controls.Add(b);}
        private void RefreshButton(object sender,EventArgs e){RefreshData();}
        private void AddMember(object sender,EventArgs e)
        {
            if(String.IsNullOrWhiteSpace(name.Text)||String.IsNullOrWhiteSpace(email.Text)){MessageBox.Show("Member name and email are required.");return;}
            Run(()=>DatabaseService.SaveMember(new Member{Name=name.Text.Trim(),Email=email.Text.Trim(),Phone=phone.Text.Trim()}));
            name.Clear();email.Clear();phone.Clear();
        }
        private void Checkout(object sender,EventArgs e)
        {
            var book=books.SelectedItem as Book; var member=members.SelectedItem as Member;
            if(book==null||member==null){MessageBox.Show("Choose a book and a member.");return;}
            if(due.Value.Date<DateTime.Today){MessageBox.Show("The due date must be today or later.");return;}
            Run(()=>DatabaseService.Checkout(book.Id,member.Id,due.Value.Date));
        }
        private void Return(object sender,EventArgs e)
        {
            if(grid.CurrentRow==null){MessageBox.Show("Select a loan first.");return;}
            var returned=grid.CurrentRow.Cells["ReturnedAt"].Value;
            if(returned!=null&&returned!=DBNull.Value&&!String.IsNullOrEmpty(Convert.ToString(returned))){MessageBox.Show("That book has already been returned.");return;}
            var id=Convert.ToString(grid.CurrentRow.Cells["Id"].Value); Run(()=>DatabaseService.ReturnBook(id));
        }
        private void Run(Action action){try{action();RefreshData();}catch(Exception ex){MessageBox.Show(ex.Message,"Library",MessageBoxButtons.OK,MessageBoxIcon.Error);}}
        private void RefreshData()
        {
            try
            {
                var allBooks=DatabaseService.Books(); books.DisplayMember="Title";books.ValueMember="Id";books.DataSource=allBooks;
                var allMembers=DatabaseService.Members();members.DisplayMember="Name";members.ValueMember="Id";members.DataSource=allMembers;
                grid.DataSource=DatabaseService.Loans().Select(l=>new{l.Id,l.BookTitle,l.MemberName,BorrowedAt=l.BorrowedAt.ToString("yyyy-MM-dd"),DueAt=l.DueAt.ToString("yyyy-MM-dd"),ReturnedAt=l.ReturnedAt.HasValue?l.ReturnedAt.Value.ToString("yyyy-MM-dd"):""}).ToList();
                if(grid.Columns["Id"]!=null)grid.Columns["Id"].Visible=false;
            }
            catch(Exception ex){MessageBox.Show(ex.Message,"Load loans",MessageBoxButtons.OK,MessageBoxIcon.Error);}
        }
    }
}
