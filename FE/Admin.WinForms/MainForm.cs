using System;
using System.Drawing;
using System.Windows.Forms;

namespace Admin.WinForms
{
    public partial class MainForm : Form
    {
        // Biến lưu trữ nút đang được chọn (Active) hiện tại
        private Button currentActiveButton = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Mặc định khi mở app sẽ chọn sẵn nút Dashboard
            ActivateButton(button_Dashboard);
        }

        // Hàm xử lý đổi màu Active cho nút được chọn
        private void ActivateButton(Button selectedButton)
        {
            // 1. Trả nút cũ về trạng thái ban đầu (Transparent)
            if (currentActiveButton != null)
            {
                currentActiveButton.BackColor = Color.Transparent;
                currentActiveButton.ForeColor = Color.White;
            }

            // 2. Gán nút hiện tại thành nút active mới
            currentActiveButton = selectedButton;

            // 3. Đổi màu nút được chọn sang màu xanh dương hiện đại (Active)
            currentActiveButton.BackColor = Color.FromArgb(35, 83, 166); // Mã màu xanh dương chuẩn Admin
            currentActiveButton.ForeColor = Color.White;
        }

        // Sự kiện Click cho từng nút trên Sidebar
        private void button_Dashboard_Click(object sender, EventArgs e)
        {
            ActivateButton((Button)sender);
            // TODO: Code gọi Load UserControl Dashboard ở đây
        }

        private void button_Employee_Click(object sender, EventArgs e)
        {
            ActivateButton((Button)sender);
            // TODO: Code gọi Load UserControl Employee ở đây
        }

        private void button_Contract_Work_Click(object sender, EventArgs e)
        {
            ActivateButton((Button)sender);
            // TODO: Code gọi Load UserControl Contracts ở đây
        }

        private void button_Organization_Click(object sender, EventArgs e)
        {
            ActivateButton((Button)sender);
            // TODO: Code gọi Load UserControl Organization ở đây
        }

        private void button_Attendance_Click(object sender, EventArgs e)
        {
            ActivateButton((Button)sender);
            // TODO: Code gọi Load UserControl Attendance ở đây
        }

        private void button_Payroll_Click(object sender, EventArgs e)
        {
            ActivateButton((Button)sender);
            // TODO: Code gọi Load UserControl Payroll ở đây
        }

        private void button_Project_Click(object sender, EventArgs e)
        {
            ActivateButton((Button)sender);
            // TODO: Code gọi Load UserControl Project ở đây
        }

        private void button_System_Click(object sender, EventArgs e)
        {
            ActivateButton((Button)sender);
            // TODO: Code gọi Load UserControl System ở đây
        }
    }
}