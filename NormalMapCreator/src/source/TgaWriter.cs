#region File Description
//-------------------------------------------------------
// File : TgaWriter.cs
// Desc : TARGA File Writer
// Date : Jan. 02, 2010
// Author : Pocol
//-------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Drawing;
#endregion

namespace Asura
{
    public class TgaWriter
    {
        /// <summary>
        /// TGAファイル作成
        /// </summary>
        /// <param name="filename">出力ファイル名</param>
        /// <param name="srcImage">変換元データ</param>
        public static bool Save(string filename, Bitmap srcImage)
        {
            byte[] tgaHeader = { 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            byte[] tgaDataHeader = new byte[6];
            byte[] tgaFutter = { 0, 0, 0, 0, 0, 0, 0, 0 };
            Color color = new Color();

            BinaryWriter dataOut;

            //ファイルを開く
            try
            {
                dataOut = new BinaryWriter(new FileStream(filename, FileMode.Create));
            }
            catch (IOException exception)
            {
                Console.WriteLine(exception.ToString());
                return false;
            }

            //データを書き込み
            try
            {
                //データヘッダーに値を格納
                tgaDataHeader[0] = (byte)(srcImage.Width % 256);
                tgaDataHeader[1] = (byte)(srcImage.Width / 256);
                tgaDataHeader[2] = (byte)(srcImage.Height % 256);
                tgaDataHeader[3] = (byte)(srcImage.Height / 256);
                tgaDataHeader[4] = 32;
                tgaDataHeader[5] = 4;

                //ヘッダー書き込み
                dataOut.Write(tgaHeader);
                dataOut.Write(tgaDataHeader);

                //ピクセルデータ書き込み
                for (int y = srcImage.Height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < srcImage.Width; x++)
                    {
                        color = srcImage.GetPixel(x, y);
                        dataOut.Write(color.B);
                        dataOut.Write(color.G);
                        dataOut.Write(color.R);
                        dataOut.Write(color.A);
                    }
                }

                //フッター書き込み
                dataOut.Write(tgaFutter);
                dataOut.Write("TRUEVISION-TARGA");
                dataOut.Write((byte)0);

                //ファイルを閉じる
                dataOut.Close();
            }
            catch (IOException exception)
            {
                Console.WriteLine(exception.ToString());
                return false;
            }

            return true;
        }
    }
}