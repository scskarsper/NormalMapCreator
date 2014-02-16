using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Asura
{
    /// <summary>
    /// カラーチャンネルです.
    /// </summary>
    public enum Channel : int
    {
        /// <summary>
        /// 赤.
        /// </summary>
        R = 0x01,

        /// <summary>
        /// 緑.
        /// </summary>
        G = 0x02,

        /// <summary>
        /// 青.
        /// </summary>
        B = 0x03,

        /// <summary>
        /// 透過度
        /// </summary>
        A = 0x04,
    }

    /// <summary>
    /// 法線マップへのコンバータです.
    /// </summary>
    public class Converter
    {
        /// <summary>
        /// コンストラクタです.
        /// </summary>
        public Converter()
        {
            /* DO_NOTHING */
        }

        /// <summary>
        /// 高さデータ.
        /// </summary>
        private double[,] HeightData { get; set; }

        /// <summary>
        /// 入力画像.
        /// </summary>
        private Bitmap Image { get; set; }

        /// <summary>
        /// 法線マップ.
        /// </summary>
        private Bitmap NormalMap { get; set; }

        /// <summary>
        /// ロードします.
        /// </summary>
        /// <param name="filename">ファイル名.</param>
        /// <param name="inputChannel">入力チャンネル</param>
        public void Load(string filename, int inputChannel)
        {
            Image = new Bitmap(filename);
            HeightData = new double[Image.Width, Image.Height];
            Color color = new Color();

            for (int x = 0; x < Image.Width; x++)
            {
                for (int y = 0; y < Image.Height; y++)
                {
                    color = Image.GetPixel(x, y);

                    switch (inputChannel)
                    {
                        case (int)Channel.R: { HeightData[x, y] = (double)color.R / 127.5; } break;
                        case (int)Channel.G: { HeightData[x, y] = (double)color.G / 127.5; } break;
                        case (int)Channel.B: { HeightData[x, y] = (double)color.B / 127.5; } break;
                        case (int)Channel.A: { HeightData[x, y] = (double)color.A / 127.5; } break;
                        default: { /* DO_NOTHING */ } break;
                    }
                }
            }
        }

        /// <summary>
        /// 法線マップを作成します.
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public Bitmap CreateNormalMap(double scale)
        {
            NormalMap = (Bitmap)Image.Clone();
            Color color = new Color();

            for (int x = 0; x < Image.Width; x++)
            {
                for (int y = 0; y < Image.Height; y++)
                {
                    int i = 1, j = 1;
                    if (x == 0 || x == Image.Width - 1)
                        i = 0;
                    if (y == 0 || y == Image.Height - 1)
                        j = 0;

                    //ベクトル作成
                    double[] vector = new double[3];
                    vector[0] = (-HeightData[x + i, y + 0] + HeightData[x - i, y + 0]) * scale;
                    vector[1] = (+HeightData[x + 0, y + j] - HeightData[x + 0, y - j]) * scale;
                    vector[2] = 1.0;

                    //正規化
                    double invlength = 1.0 / Math.Sqrt(vector[0] * vector[0] + vector[1] * vector[1] + vector[2] * vector[2]);
                    vector[0] *= invlength;
                    vector[1] *= invlength;
                    vector[2] *= invlength;

                    //範囲変更
                    vector[0] = (vector[0] + 1.0) * 127.5;
                    vector[1] = (vector[1] + 1.0) * 127.5;
                    vector[2] = (vector[2] + 1.0) * 127.5;

                    if (vector[0] < 0.0) vector[0] = 0;
                    if (vector[1] < 0.0) vector[1] = 0;
                    if (vector[2] < 0.0) vector[2] = 0;

                    if (vector[0] > 255.0) vector[0] = 255.0;
                    if (vector[1] > 255.0) vector[1] = 255.0;
                    if (vector[2] > 255.0) vector[2] = 255.0;

                    //法線データ格納
                    color = Color.FromArgb(
                        (byte)(HeightData[x, y] * 127.5),
                        (byte)vector[0],
                        (byte)vector[1],
                        (byte)vector[2]);
                    NormalMap.SetPixel(x, y, color);
                }
            }

            return NormalMap;
        }


    }
}