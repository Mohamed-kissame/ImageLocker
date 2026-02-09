using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageLocker
{
    public partial class Form1 : Form
    {

        private string _originalFile;
        private readonly string _encryptedFile = @"C:\Image\encrypted.dat";
        private readonly string _decryptedFile = @"C:\Image\decrypted.jpg";


        public Form1()
        {
            InitializeComponent();
        }

        private void LoadImageToPictureBox(string path)
        {
            if (!File.Exists(path)) return;

            if (picInspect.Image != null)
            {
                picInspect.Image.Dispose();
                picInspect.Image = null;
            }

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                picInspect.Image = Image.FromStream(fs);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg; *.png)|*.jpg;*.png|Encrypted Files (*.dat)|*.dat",
                Title = "Select an Image or Encrypted File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _originalFile = openFileDialog.FileName;

                FileInfo fileInfo = new FileInfo(_originalFile);
                const long maxFileSize = 25 * 1024 * 1024;

                if (fileInfo.Length > maxFileSize)
                {
                    MessageBox.Show("Selected file is too large. Maximum size is 25MB.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string extension = Path.GetExtension(_originalFile).ToLower();

                if (extension == ".jpg" || extension == ".png")
                {
                   
                    LoadImageToPictureBox(_originalFile);
                }
                else
                {
                    
                    picInspect.Image?.Dispose();
                    picInspect.Image = null;

                    MessageBox.Show(
                        "Encrypted file selected.\nPreview is not available.\nUse Decrypt to view the image.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_originalFile))
            {
                MessageBox.Show("Please select an image first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string password = txtSecurityKey.Text;
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Password is required.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Directory.CreateDirectory(@"C:\Image");

            ImageCryptoService.EncryptFile(_originalFile, _encryptedFile, password);

            MessageBox.Show($"Image encrypted successfully you will find it in {_encryptedFile}.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            string password = txtSecurityKey.Text;
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Password is required." , "Warning" , MessageBoxButtons.OK , MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(_encryptedFile))
            {
                MessageBox.Show("Encrypted file not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ImageCryptoService.DecryptFile(_encryptedFile, _decryptedFile, password);

            LoadImageToPictureBox(_decryptedFile);

            MessageBox.Show($"Image decrypted successfully you will find it in {_decryptedFile}." , "Information" , MessageBoxButtons.OK , MessageBoxIcon.Information);
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
