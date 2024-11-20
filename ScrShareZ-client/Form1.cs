using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace ScrShareZ_Client
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private BinaryFormatter binFormatter;

        public Form1()
        {
            InitializeComponent();
        }

        // Chụp ảnh toàn màn hình
        private Image CaptureScreen()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds; // Lấy kích thước màn hình chính
            Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height);  // Tạo bitmap với kích thước màn hình
            using (Graphics g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size); // Sao chép màn hình vào bitmap
            }
            return screenshot;
        }

        // Gửi ảnh màn hình
        private void SendScreen()
        {
            try
            {
                if (client.Connected)
                {
                    stream = client.GetStream(); // Lấy dòng dữ liệu của client
                    binFormatter = new BinaryFormatter();
                    Image screenImage = CaptureScreen(); // Lấy ảnh màn hình
                    binFormatter.Serialize(stream, screenImage); // Serialize và gửi ảnh qua stream
                }
                else
                {
                    MessageBox.Show("Client is not connected.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending image: {ex.Message}");
            }
        }

        // Kết nối client tới server
        private void btnConnect_Click(object sender, EventArgs e)
        {
            string serverIp = txtIP.Text;
            int port = int.Parse(txtPort.Text);

            try
            {
                client = new TcpClient(serverIp, port); // Kết nối tới server
                MessageBox.Show("Connected to server.");
                btnShare.Enabled = true; // Bật nút chia sẻ ảnh
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}");
            }
        }

        // Bắt đầu chia sẻ màn hình
        private void btnShare_Click(object sender, EventArgs e)
        {
            if (btnShare.Text == "Start Sharing")
            {
                // Bắt đầu chia sẻ màn hình liên tục
                timer1.Start();
                btnShare.Text = "Stop Sharing";
            }
            else
            {
                // Dừng chia sẻ màn hình
                timer1.Stop();
                btnShare.Text = "Start Sharing";
            }
        }

        // Gửi ảnh mỗi giây khi đang chia sẻ
        private void timer1_Tick(object sender, EventArgs e)
        {
            SendScreen();
        }

        // Hủy kết nối khi form đóng
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (client.Connected)
            {
                client.Close();
            }
        }
    }
}
