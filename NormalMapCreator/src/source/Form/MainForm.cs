#region File Description
//-------------------------------------------------------
// File : Form1.cs
// Desc : Normal Map Creator Main Form
// Date : Jan. 02, 2010
// Author : Pocol
//-------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
#endregion

namespace Asura
{
    public partial class MainForm : Form
    {
        #region Field
        private Image image = null;
        private ImageFormat format = ImageFormat.Bmp;
        private string inputFileName = null;
        private string outputFileName = null;
        private double scale = 1.0;
        private int inputChannel = (int)Asura.Channel.R;
        private Asura.Converter converter = null;
        private bool customFormat = false;
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            converter = new Asura.Converter();
            comboBox1.SelectedIndex = 0;
        }

        /// <summary>
        /// 入力ファイル
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = Properties.Resources.FileFilterBMP
                + Properties.Resources.FileFilterJPG
                + Properties.Resources.FileFilterPNG
                + Properties.Resources.FileFilterGIF
                + Properties.Resources.FileFilterTIF
                + Properties.Resources.FileFilterEMF
                + Properties.Resources.FileFilterAll;
            dialog.FilterIndex = 1;
            dialog.Title = Properties.Resources.OpenFileDialogTitle;
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                inputFileName = dialog.FileName;
                textBox1.Text = inputFileName;
                image = Image.FromFile(inputFileName);
                pictureBox2.Image = image;

                string[] splitTable = 
                {
                    ".bmp",
                    ".BMP",
                    ".jpg",
                    ".JPG",
                    ".png",
                    ".PNG",
                    ".emf",
                    ".EMF",
                    ".tif",
                    ".TIF"
                };

                string[] splitedString = inputFileName.Split(splitTable, StringSplitOptions.RemoveEmptyEntries);
                outputFileName = splitedString[0] + "_normal.bmp";
                textBox2.Text = outputFileName;
            }
        }

        /// <summary>
        /// 出力ファイル
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = Properties.Resources.FileFilterBMP
                + Properties.Resources.FileFilterJPG
                + Properties.Resources.FileFilterPNG
                + Properties.Resources.FileFilterGIF
                + Properties.Resources.FileFilterTIF
                + Properties.Resources.FileFilterEMF
                + Properties.Resources.FileFilterAll;
            dialog.FilterIndex = 1;
            dialog.Title = Properties.Resources.SaveFileDialogTitle;
            dialog.CheckFileExists = false;
            dialog.CheckPathExists = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                outputFileName = dialog.FileName;
                textBox2.Text = outputFileName;
            }
        }

        /// <summary>
        /// 入力チャンネル
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            inputChannel = comboBox1.SelectedIndex + 1;
        }

        /// <summary>
        /// 入力イメージの描画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            if (image != null)
                e.Graphics.DrawImage(image, 0, 0, pictureBox2.Width, pictureBox2.Height);
        }

        /// <summary>
        /// 入力ファイルの文字列入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            inputFileName = textBox1.Text;
        }

        /// <summary>
        /// 出力ファイルの文字列入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            outputFileName = textBox2.Text;
            format = CheckImageFormat(outputFileName);
        }

        /// <summary>
        /// スケールの入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                scale = Double.Parse(textBox3.Text);
            }
            catch (FormatException exception)
            {
                MessageBox.Show(
                    "Warning: スケールの値が不正です",
                    "スケールの値が不正です",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                scale = 1.0;
                textBox3.Text = "1.0";
                Console.WriteLine(exception.ToString());
            }
        }

        /// <summary>
        /// 変換実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            converter.Load(inputFileName, inputChannel);
            format = CheckImageFormat(outputFileName);

            if (!customFormat)
            {
                image = converter.CreateNormalMap(scale);
                image.Save(outputFileName, format);
            }
            else
            {
                Asura.TgaWriter.Save(outputFileName, converter.CreateNormalMap(scale));
            }

            MessageBox.Show("変換に成功しました", "変換終了", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 入力ファイルのドラッグアンドドロップ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            inputFileName = textBox1.Text;
        }

        /// <summary>
        /// 出力ファイルのドラッグアンドドロップ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox2_DragDrop(object sender, DragEventArgs e)
        {
            outputFileName = textBox2.Text;
            format = CheckImageFormat(outputFileName);
        }

        /// <summary>
        /// イメージフォーマットのチェック
        /// </summary>
        /// <param name="filename">出力ファイル名</param>
        /// <returns>拡張子から判別したイメージフォーマット</returns>
        public ImageFormat CheckImageFormat(string filename)
        {
            ImageFormat result = ImageFormat.Bmp;

            customFormat = false;

            if (filename.Contains(".bmp") || filename.Contains(".BMP"))
            {
                result = ImageFormat.Bmp;
            }
            else if (filename.Contains(".jpg") || filename.Contains(".JPG"))
            {
                result = ImageFormat.Jpeg;
            }
            else if (filename.Contains(".png") || filename.Contains(".PNG"))
            {
                result = ImageFormat.Png;
            }
            else if (filename.Contains(".gif") || filename.Contains(".GIF"))
            {
                result = ImageFormat.Gif;
            }
            else if (filename.Contains(".emf") || filename.Contains(".EMF"))
            {
                result = ImageFormat.Emf;
            }
            else if (filename.Contains(".tga") || filename.Contains(".TGA"))
            {
                customFormat = true;
            }
            else
            {
                /* DO_NOTHING */
            }

            return result;
        }
    }
}
