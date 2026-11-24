using Admin.WinForms.DTOs.Attendance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Admin.WinForms.Controls.Attendance
{
    public partial class AttendanceApprovalControl : UserControl
    {
        private class ApprovalItemModel
        {
            public Guid Id { get; set; }
            public string Category { get; set; } = "Explanation"; // "Explanation" or "OT"
            public Guid EmployeeId { get; set; }
            public DateOnly Date { get; set; }
            public string Type { get; set; } = string.Empty;
            public string Reason { get; set; } = string.Empty;
            public string StartTime { get; set; } = "--:--";
            public string EndTime { get; set; } = "--:--";
            public string Status { get; set; } = string.Empty;
            public string ApprovedBy { get; set; } = "--";
            public string ApprovedAt { get; set; } = "--";
            public string Extra { get; set; } = "--";
        }

        private List<ApprovalItemModel> _allApprovals = new();
        private string _statusFilter = "Tất cả";

        public AttendanceApprovalControl()
        {
            InitializeComponent();

            comboBox_TypeFilter.Items.AddRange(new object[] { "Tất cả loại đơn", "Giải trình chấm công", "Đơn làm thêm giờ (OT)" });
            comboBox_TypeFilter.SelectedIndex = 0;
            comboBox_TypeFilter.SelectedIndexChanged += (s, e) => ApplyFilters();

            button_Reload.Click += async (s, e) => await LoadApprovalsAsync();
            button_Filter.Click += Button_Filter_Click;
            button_Approve.Click += async (s, e) => await HandleApproveRejectAsync(isApprove: true);
            button_Reject.Click += async (s, e) => await HandleApproveRejectAsync(isApprove: false);

            Load += AttendanceApprovalControl_Load;
        }

        private async void AttendanceApprovalControl_Load(object? sender, EventArgs e)
        {
            await LoadApprovalsAsync();
        }

        private async Task LoadApprovalsAsync()
        {
            try
            {
                _allApprovals.Clear();

                // 1. Tải danh sách đơn giải trình (Explanations)
                try
                {
                    var explanations = await Program.AttendanceApi.GetExplanationsAsync();
                    foreach (var item in explanations)
                    {
                        _allApprovals.Add(new ApprovalItemModel
                        {
                            Id = item.Id,
                            Category = "Explanation",
                            EmployeeId = item.EmployeeId,
                            Date = item.Date,
                            Type = $"[Giải trình] {item.ExplanationType}",
                            Reason = item.Reason,
                            StartTime = item.RequestedTimeIn?.ToString("HH:mm") ?? "--:--",
                            EndTime = item.RequestedTimeOut?.ToString("HH:mm") ?? "--:--",
                            Status = item.Status,
                            ApprovedBy = item.ApprovedBy ?? "--",
                            ApprovedAt = item.ApprovedAt?.ToString("dd/MM/yyyy HH:mm") ?? "--",
                            Extra = !string.IsNullOrWhiteSpace(item.AttachmentUrl) ? item.AttachmentUrl : "--"
                        });
                    }
                }
                catch
                {
                }

                // 2. Tải danh sách đơn làm thêm giờ (OTRequests)
                try
                {
                    var otRequests = await Program.AttendanceApi.GetOTRequestsAsync();
                    foreach (var item in otRequests)
                    {
                        _allApprovals.Add(new ApprovalItemModel
                        {
                            Id = item.Id,
                            Category = "OT",
                            EmployeeId = item.EmployeeId,
                            Date = item.Date,
                            Type = $"[Tăng ca OT] {(item.IsUnplanned ? "Đột xuất" : "Kế hoạch")}",
                            Reason = item.Reason ?? $"OT {item.PlannedHours}h",
                            StartTime = item.StartTime.ToString("HH:mm"),
                            EndTime = item.EndTime.ToString("HH:mm"),
                            Status = item.Status,
                            ApprovedBy = item.ApprovedBy ?? "--",
                            ApprovedAt = item.ApprovedAt?.ToString("dd/MM/yyyy HH:mm") ?? "--",
                            Extra = $"Hệ số: {item.Multiplier}x | Dự kiến: {item.PlannedHours}h"
                        });
                    }
                }
                catch
                {
                }

                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể tải danh sách phê duyệt: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            var filtered = _allApprovals.AsEnumerable();

            // Lọc theo Loại đơn
            var typeSelection = comboBox_TypeFilter.SelectedIndex;
            if (typeSelection == 1) // Giải trình
            {
                filtered = filtered.Where(x => x.Category == "Explanation");
            }
            else if (typeSelection == 2) // OT
            {
                filtered = filtered.Where(x => x.Category == "OT");
            }

            // Lọc theo Trạng thái
            if (!string.IsNullOrWhiteSpace(_statusFilter) && _statusFilter != "Tất cả")
            {
                filtered = filtered.Where(x => x.Status.Equals(_statusFilter, StringComparison.OrdinalIgnoreCase));
            }

            DisplayApprovals(filtered.OrderByDescending(x => x.Date).ToList());
        }

        private void DisplayApprovals(List<ApprovalItemModel> items)
        {
            listView_Approvals.BeginUpdate();
            listView_Approvals.Items.Clear();

            foreach (var item in items)
            {
                // 1. Mã NV
                var row = new ListViewItem(item.EmployeeId.ToString()[..8]);

                // 2. Ngày
                row.SubItems.Add(item.Date.ToString("dd/MM/yyyy"));

                // 3. Loại đơn
                row.SubItems.Add(item.Type);

                // 4. Lý do
                row.SubItems.Add(item.Reason);

                // 5. Bắt đầu
                row.SubItems.Add(item.StartTime);

                // 6. Kết thúc
                row.SubItems.Add(item.EndTime);

                // 7. Trạng thái
                row.SubItems.Add(item.Status);

                // 8. Người duyệt
                row.SubItems.Add(item.ApprovedBy);

                // 9. Thời điểm duyệt
                row.SubItems.Add(item.ApprovedAt);

                // 10. Minh chứng / Ghi chú
                row.SubItems.Add(item.Extra);

                // Đổi màu dòng theo trạng thái
                if (item.Status.Equals("APPROVED", StringComparison.OrdinalIgnoreCase) || item.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                {
                    row.ForeColor = Color.FromArgb(40, 167, 69);
                }
                else if (item.Status.Equals("REJECTED", StringComparison.OrdinalIgnoreCase) || item.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                {
                    row.ForeColor = Color.FromArgb(220, 53, 69);
                }
                else
                {
                    row.ForeColor = Color.FromArgb(217, 119, 6);
                }

                row.Tag = item;
                listView_Approvals.Items.Add(row);
            }

            listView_Approvals.EndUpdate();
        }

        private async Task HandleApproveRejectAsync(bool isApprove)
        {
            if (listView_Approvals.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một đơn trong danh sách để xử lý.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedItem = listView_Approvals.SelectedItems[0].Tag as ApprovalItemModel;
            if (selectedItem == null) return;

            string actionText = isApprove ? "DUYỆT" : "TỪ CHỐI";
            string targetStatus = isApprove ? "Approved" : "Rejected";
            string rejectionReason = string.Empty;

            if (!isApprove)
            {
                // Cho nhập lý do từ chối
                using var inputForm = new Form
                {
                    Text = "Nhập lý do từ chối",
                    Width = 400,
                    Height = 200,
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false
                };

                var lbl = new Label { Text = "Lý do từ chối đơn:", Location = new Point(20, 15), AutoSize = true };
                var txt = new TextBox { Location = new Point(20, 45), Width = 340, Multiline = true, Height = 50 };
                var btnConfirm = new Button { Text = "Xác nhận", DialogResult = DialogResult.OK, Location = new Point(240, 105), Width = 120, Height = 35 };

                inputForm.Controls.Add(lbl);
                inputForm.Controls.Add(txt);
                inputForm.Controls.Add(btnConfirm);
                inputForm.AcceptButton = btnConfirm;

                if (inputForm.ShowDialog(this) != DialogResult.OK) return;
                rejectionReason = string.IsNullOrWhiteSpace(txt.Text) ? "Không đạt yêu cầu" : txt.Text.Trim();
            }

            var confirmResult = MessageBox.Show(
                $"Bạn có chắc chắn muốn {actionText} đơn của nhân viên {selectedItem.EmployeeId.ToString()[..8]} ({selectedItem.Type})?",
                "Xác nhận phê duyệt",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmResult != DialogResult.Yes) return;

            try
            {
                bool success = false;
                string currentApprover = "HR Manager";

                if (selectedItem.Category == "Explanation")
                {
                    var dto = new UpdateAttendanceExplanationDto
                    {
                        Status = targetStatus,
                        ApprovedBy = currentApprover,
                        ApprovedAt = DateTime.UtcNow,
                        RejectionReason = isApprove ? null : rejectionReason,
                        UpdatedAt = DateTime.UtcNow
                    };
                    success = await Program.AttendanceApi.UpdateExplanationAsync(selectedItem.Id, dto);
                }
                else if (selectedItem.Category == "OT")
                {
                    var dto = new UpdateOTRequestDto
                    {
                        Status = targetStatus,
                        ApprovedBy = currentApprover,
                        ApprovedAt = DateTime.UtcNow,
                        RejectionReason = isApprove ? null : rejectionReason,
                        UpdatedAt = DateTime.UtcNow
                    };
                    success = await Program.AttendanceApi.UpdateOTRequestAsync(selectedItem.Id, dto);
                }

                if (success)
                {
                    MessageBox.Show($"Đã {actionText.ToLower()} đơn thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadApprovalsAsync();
                }
                else
                {
                    MessageBox.Show($"Xử lý đơn thất bại qua API.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thực hiện: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button_Filter_Click(object? sender, EventArgs e)
        {
            using var form = new Form
            {
                Text = "Lọc trạng thái phê duyệt",
                Width = 320,
                Height = 180,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var cmb = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(20, 25),
                Width = 260
            };
            cmb.Items.AddRange(new object[] { "Tất cả", "PENDING", "APPROVED", "REJECTED" });
            cmb.SelectedItem = _statusFilter;
            if (cmb.SelectedIndex < 0) cmb.SelectedIndex = 0;

            var btnOk = new Button
            {
                Text = "Áp dụng",
                DialogResult = DialogResult.OK,
                Location = new Point(180, 75),
                Width = 100
            };

            form.Controls.Add(cmb);
            form.Controls.Add(btnOk);
            form.AcceptButton = btnOk;

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                _statusFilter = cmb.SelectedItem?.ToString() ?? "Tất cả";
                ApplyFilters();
            }
        }
    }
}
