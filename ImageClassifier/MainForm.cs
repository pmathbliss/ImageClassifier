using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing.Drawing2D;
using System.IO;

namespace ImageClassifier
{
    public partial class MainForm : Form
    {
        private FilterInfoCollection VideoCaptureDevices;
        private VideoCaptureDevice FinalVideo;
        private PictureBox pictureBox;

        public MainForm()
        {
            InitializeComponent();

            VideoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo VideoCaptureDevice in VideoCaptureDevices)
            {
                comboBox1.Items.Add(VideoCaptureDevice);
            }
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "MonikerString";
            comboBox1.SelectedIndex = 0;

            videoSourcePlayer1.NewFrame += VideoSourcePlayer1_NewFrame;
            pictureBox = new PictureBox();
        }

        private void VideoSourcePlayer1_NewFrame(object sender, ref Bitmap image)
        {
            pictureBox.Image = image;
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            var captureDevice = comboBox1.SelectedItem as FilterInfo;
            videoSourcePlayer1.VideoSource = new VideoCaptureDevice(captureDevice.MonikerString);
            videoSourcePlayer1.Start();
            //videoSourcePlayer1.VideoSource = captureDevice.
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SavePicture();
        }

        private void textBoxScore_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                SavePicture();
            }
        }

        /// <summary>
        /// Save the current picture.
        /// </summary>
        private void SavePicture()
        {
            if(!videoSourcePlayer1.VideoSource.IsRunning)
            {
                MessageBox.Show("Select a video source and make sure it is running.");
                return;
            }


            string score = textBoxScore.Text;

            if (string.IsNullOrWhiteSpace(score))
            {
                MessageBox.Show("Enter a Value");
                return;
            }

            //pause the videoSource
            videoSourcePlayer1.VideoSource.Stop();

            string modelName = comboBoxModel.Text;
            string modelPath = Path.Combine("Models", modelName);

            DateTime foo = DateTime.UtcNow;
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();


            string fileName = $"{unixTime}-{score}.jpg";

            pictureBox.Image.Save(Path.Combine(modelPath, fileName));
            videoSourcePlayer1.VideoSource.Start();

        }

        private void btnSaveModel_Click(object sender, EventArgs e)
        {
            CreateModelDirectory();
        }

        /// <summary>
        /// Create the model directory and add the current selected model.
        /// </summary>
        private void CreateModelDirectory()
        {
            if (!Directory.Exists("Models"))
            {
                Directory.CreateDirectory("Models");
            }

            string modelName = comboBoxModel.Text;

            if(string.IsNullOrWhiteSpace(modelName))
            {
                MessageBox.Show("Enter a model name");
                return;
            }

            if (!Directory.Exists(Path.Combine("Models", modelName)))
            {
                Directory.CreateDirectory(Path.Combine("Models", modelName));
            }

            if(!comboBoxModel.Items.Contains(modelName))
            {
                comboBoxModel.Items.Add(modelName);
            }
        }

        private void comboBoxModel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CreateModelDirectory();
            }
        }
    }
}
