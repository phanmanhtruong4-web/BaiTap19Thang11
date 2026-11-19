using System;
using System.Drawing;
using System.Windows.Forms;
using ProjectManagement.BUS;
using ProjectManagement.DTO;

namespace ProjectManagement.GUI
{
    public class FormDuAn : Form
    {
        private readonly DuAnBUS _duAnBUS = new DuAnBUS();
        private DataGridView dgvDuAn = null!;
        private TextBox txtMaDuAn = null!;
        private TextBox txtTenDuAn = null!;
        private DateTimePicker dtpNgayBatDau = null!;
        private DateTimePicker dtpNgayKetThuc = null!;
        private ComboBox cboTrangThai = null!;
        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;
        private Button btnLamMoi = null!;

        public FormDuAn()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý Dự án (3-Tier Layered)";
            this.Size = new Size(1050, 620);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            // Header Panel
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(41, 128, 185)
            };
            var lblTitle = new Label
            {
                Text = "QUẢN LÝ DANH SÁCH DỰ ÁN",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlHeader.Controls.Add(lblTitle);

            // Left Input Panel
            var pnlInput = new GroupBox
            {
                Text = "Thông tin dự án",
                Dock = DockStyle.Left,
                Width = 380,
                Padding = new Padding(15)
            };

            int y = 30;
            var lblMa = new Label { Text = "Mã dự án (Tự động):", Location = new Point(15, y), AutoSize = true };
            txtMaDuAn = new TextBox { Location = new Point(15, y + 25), Width = 340, ReadOnly = true, BackColor = Color.LightGray };
            y += 65;

            var lblTen = new Label { Text = "Tên dự án (*):", Location = new Point(15, y), AutoSize = true };
            txtTenDuAn = new TextBox { Location = new Point(15, y + 25), Width = 340 };
            y += 65;

            var lblBD = new Label { Text = "Ngày bắt đầu:", Location = new Point(15, y), AutoSize = true };
            dtpNgayBatDau = new DateTimePicker { Location = new Point(15, y + 25), Width = 340, Format = DateTimePickerFormat.Short };
            y += 65;

            var lblKT = new Label { Text = "Ngày kết thúc:", Location = new Point(15, y), AutoSize = true };
            dtpNgayKetThuc = new DateTimePicker { Location = new Point(15, y + 25), Width = 340, Format = DateTimePickerFormat.Short };
            y += 65;

            var lblTT = new Label { Text = "Trạng thái:", Location = new Point(15, y), AutoSize = true };
            cboTrangThai = new ComboBox { Location = new Point(15, y + 25), Width = 340, DropDownStyle = ComboBoxStyle.DropDownList };
            cboTrangThai.Items.AddRange(new object[] { "Đang chạy", "Hoàn thành", "Tạm dừng" });
            cboTrangThai.SelectedIndex = 0;
            y += 70;

            // Buttons
            btnThem = new Button { Text = "Thêm mới", Location = new Point(15, y), Size = new Size(82, 35), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            btnSua = new Button { Text = "Cập nhật", Location = new Point(102, y), Size = new Size(82, 35), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            btnXoa = new Button { Text = "Xóa", Location = new Point(189, y), Size = new Size(75, 35), BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            btnLamMoi = new Button { Text = "Làm mới", Location = new Point(269, y), Size = new Size(85, 35), BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };

            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;
            btnLamMoi.Click += (s, e) => ResetFields();

            pnlInput.Controls.AddRange(new Control[] {
                lblMa, txtMaDuAn,
                lblTen, txtTenDuAn,
                lblBD, dtpNgayBatDau,
                lblKT, dtpNgayKetThuc,
                lblTT, cboTrangThai,
                btnThem, btnSua, btnXoa, btnLamMoi
            });

            // Right Grid Panel
            var pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            dgvDuAn = new DataGridView
            {
                Dock = DockStyle.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                RowTemplate = { Height = 28 },
                ColumnHeadersHeight = 32
            };
            dgvDuAn.CellClick += DgvDuAn_CellClick;
            pnlGrid.Controls.Add(dgvDuAn);

            this.Controls.Add(pnlGrid);
            this.Controls.Add(pnlInput);
            this.Controls.Add(pnlHeader);
        }

        private void LoadData()
        {
            try
            {
                dgvDuAn.DataSource = null;
                dgvDuAn.DataSource = _duAnBUS.GetAll();

                if (dgvDuAn.Columns["MaDuAn"] != null)
                {
                    dgvDuAn.Columns["MaDuAn"].HeaderText = "Mã Dự Án";
                    dgvDuAn.Columns["MaDuAn"].Width = 90;
                }
                if (dgvDuAn.Columns["TenDuAn"] != null)
                {
                    dgvDuAn.Columns["TenDuAn"].HeaderText = "Tên Dự Án";
                    dgvDuAn.Columns["TenDuAn"].Width = 250;
                }
                if (dgvDuAn.Columns["NgayBatDau"] != null)
                {
                    dgvDuAn.Columns["NgayBatDau"].HeaderText = "Ngày Bắt Đầu";
                    dgvDuAn.Columns["NgayBatDau"].Width = 130;
                    dgvDuAn.Columns["NgayBatDau"].DefaultCellStyle.Format = "dd/MM/yyyy";
                }
                if (dgvDuAn.Columns["NgayKetThuc"] != null)
                {
                    dgvDuAn.Columns["NgayKetThuc"].HeaderText = "Ngày Kết Thúc";
                    dgvDuAn.Columns["NgayKetThuc"].Width = 130;
                    dgvDuAn.Columns["NgayKetThuc"].DefaultCellStyle.Format = "dd/MM/yyyy";
                }
                if (dgvDuAn.Columns["TrangThai"] != null)
                {
                    dgvDuAn.Columns["TrangThai"].HeaderText = "Trạng Thái";
                    dgvDuAn.Columns["TrangThai"].Width = 120;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvDuAn_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvDuAn.Rows[e.RowIndex].DataBoundItem is DuAnDTO da)
            {
                txtMaDuAn.Text = da.MaDuAn.ToString();
                txtTenDuAn.Text = da.TenDuAn;
                if (da.NgayBatDau.HasValue) dtpNgayBatDau.Value = da.NgayBatDau.Value;
                if (da.NgayKetThuc.HasValue) dtpNgayKetThuc.Value = da.NgayKetThuc.Value;
                cboTrangThai.SelectedItem = da.TrangThai;
            }
        }

        private void BtnThem_Click(object? sender, EventArgs e)
        {
            var da = new DuAnDTO
            {
                TenDuAn = txtTenDuAn.Text.Trim(),
                NgayBatDau = dtpNgayBatDau.Value,
                NgayKetThuc = dtpNgayKetThuc.Value,
                TrangThai = cboTrangThai.SelectedItem?.ToString() ?? "Đang chạy"
            };

            string error = _duAnBUS.Add(da);
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Thêm dự án thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadData();
            ResetFields();
        }

        private void BtnSua_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtMaDuAn.Text, out int ma))
            {
                MessageBox.Show("Vui lòng chọn dự án cần sửa từ danh sách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var da = new DuAnDTO
            {
                MaDuAn = ma,
                TenDuAn = txtTenDuAn.Text.Trim(),
                NgayBatDau = dtpNgayBatDau.Value,
                NgayKetThuc = dtpNgayKetThuc.Value,
                TrangThai = cboTrangThai.SelectedItem?.ToString() ?? "Đang chạy"
            };

            string error = _duAnBUS.Update(da);
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Cập nhật dự án thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadData();
            ResetFields();
        }

        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtMaDuAn.Text, out int ma))
            {
                MessageBox.Show("Vui lòng chọn dự án cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa dự án này? (Các công việc liên quan cũng sẽ bị xóa)", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string error = _duAnBUS.Delete(ma);
                if (!string.IsNullOrEmpty(error))
                {
                    MessageBox.Show(error, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                MessageBox.Show("Xóa dự án thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ResetFields();
            }
        }

        private void ResetFields()
        {
            txtMaDuAn.Clear();
            txtTenDuAn.Clear();
            dtpNgayBatDau.Value = DateTime.Now;
            dtpNgayKetThuc.Value = DateTime.Now.AddMonths(1);
            cboTrangThai.SelectedIndex = 0;
        }
    }
}
