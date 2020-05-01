using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Diagnostics;

namespace TestFormsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            _bitmap = new Bitmap(pictureBox1.Bounds.Width, pictureBox1.Bounds.Height);
            Console.WriteLine("" + pictureBox1.Bounds.Width + " | " + pictureBox1.Bounds.Height);
            _electricalField = new ElectricalField(_bitmap.Width, _bitmap.Height);
            _electricalField.placeElectricalCharge(_bitmap.Width / 2 - 35, _bitmap.Height / 2, 4);
            _electricalField.placeElectricalCharge(_bitmap.Width / 2 + 35, _bitmap.Height / 2, -3);
            checkBox1.Checked = true;
            drawElectricalField();
        }

        private int _selectedParticle = -1;
        private ElectricalField _electricalField;
        private Bitmap _bitmap;
        long _counter = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Initilize program\n");
            // backgroundWorker1.RunWorkerAsync();
        }

        private void drawElectricalField()
        {
            int[,] imageMatrix = _electricalField.getElectricalFieldAsRGB();

            unsafe
            {
                fixed (int* intPtr = &imageMatrix[0, 0])
                {
                    _bitmap = new Bitmap(imageMatrix.GetLength(0), imageMatrix.GetLength(1),
                        imageMatrix.GetLength(1) * 4,
                        PixelFormat.Format32bppArgb, new IntPtr(intPtr));
                }
            }

            pictureBox1.Image = _bitmap;
            pictureBox1.Update();
            _counter++;
        }

        private void backgroundWorker1_DoWork_1(object sender, DoWorkEventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (true)
            {
                drawElectricalField();
                if (sw.Elapsed.Seconds == 1)
                {
                    Console.WriteLine("FPS: " + _counter);
                    _counter = 0;
                    sw.Restart();
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouseEventArgs = (MouseEventArgs) e;
            int x = mouseEventArgs.Location.Y;
            int y = mouseEventArgs.Location.X;
            if (_selectedParticle == -1)
            {
                _selectedParticle = checkBox1.Checked ? 0 : 1;
                _electricalField.changePosition(_selectedParticle, x, y);
                drawElectricalField();
            }
            else
            {
                _electricalField.changePosition(_selectedParticle, x, y);
                _selectedParticle = -1;
                drawElectricalField();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            _electricalField.changeCharge(0, trackBar1.Value);
            label2.Text = trackBar1.Value + "e";
            label2.Update();
            drawElectricalField();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            _electricalField.changeCharge(1, trackBar3.Value);
            label4.Text = trackBar3.Value + "e";
            label4.Update();
            drawElectricalField();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                _selectedParticle = -1;
                checkBox2.Checked = false;
                checkBox2.Update();
            }

            checkBox1.Update();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                _selectedParticle = -1;
                checkBox1.Checked = false;
                checkBox1.Update();
            }

            checkBox2.Update();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_selectedParticle != -1)
            {
                _electricalField.changePosition(_selectedParticle, e.Location.Y, e.Location.X);
                drawElectricalField();
            }
        }
    }
}