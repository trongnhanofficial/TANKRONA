using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace CrazyTank
{
    public class GameImages
    {
        public static List<Bitmap>[] tankPic = new List<Bitmap>[16];
        public static Bitmap coronaPic;
        public static Bitmap orangePic;
        public static List<Bitmap> bulletPic = new List<Bitmap>();
        public static Bitmap explodePic;
        public static List<Bitmap> tilePic = new List<Bitmap>();
        private Size ts = Tank.bmpSize;
        private Size cs = Corona.bmpSize;
        private Size bs = Bullet.bmpSize;
        private Size es = Explode.bmpSize;
        private Size tis = Tile.bmpSize;
        private string picPath = Application.StartupPath + "//Res//";

        public GameImages()
        {
            int i, j;
            Rectangle tmpRec;
            Bitmap tmpBmp;
            
            Bitmap bmpTank1 = new Bitmap(picPath + "tank1.bmp");
            bmpTank1.MakeTransparent(Color.Black);
            System.Drawing.Imaging.PixelFormat format = bmpTank1.PixelFormat;
            for (i = 0; i < 16; i++)
            {
                tankPic[i] = new List<Bitmap>();
            }
            for (i = 0; i < 8; i++)
            {
                for (j = 0; j < 4; j++)
                {
                    tmpRec = new Rectangle(i * ts.Width, j * ts.Height, ts.Width, ts.Height);
                    tmpBmp = bmpTank1.Clone(tmpRec, format);
                    tankPic[i].Add(tmpBmp);
                }
            }

            coronaPic = new Bitmap(picPath + "covid.bmp");
            coronaPic.MakeTransparent(Color.Black);

            orangePic = new Bitmap(picPath + "orange.bmp");/////////////////////
            orangePic.MakeTransparent(Color.Black);

            Bitmap bmpBullet = new Bitmap(picPath + "bullet.bmp");
            bmpBullet.MakeTransparent(Color.Black);
            for (i = 0; i < 4; i++)
            {
                tmpRec = new Rectangle(i * bs.Width, 0, bs.Width, bs.Height);
                tmpBmp = bmpBullet.Clone(tmpRec, format);
                bulletPic.Add(tmpBmp);
            }
            
            explodePic = new Bitmap(picPath + "bomb1.bmp");
            explodePic.MakeTransparent(Color.Black);
            
            Bitmap bmpTile = new Bitmap(picPath + "tile.bmp");
            bmpTile.MakeTransparent(Color.Black);
            for (i = 0; i < 4; i++)
            {
                tmpRec = new Rectangle(i * tis.Width * 2, 0, tis.Width, tis.Height);
                tmpBmp = bmpTile.Clone(tmpRec, format);
                tilePic.Add(tmpBmp);
            }
        }

        public static List<Bitmap> Recoloring(List<Bitmap> bmps, float[] color)
        {
            List<Bitmap> imgs = new List<Bitmap>();
            float[][] colorMatrixElements = { new float[] {1,  0,  0,  0, 0},
                                              new float[] {0,  1,  0,  0, 0},
                                              new float[] {0,  0,  1,  0, 0},
                                              new float[] {0,  0,  0,  1, 0},
                                              new float[] {color[0], color[1], color[2], 0, 0}};
            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(
               colorMatrix,
               ColorMatrixFlag.Default,
               ColorAdjustType.Bitmap);
            for (int i = 0; i < bmps.Count; i++)
            {
                Bitmap tmp = new Bitmap(bmps[i].Width, bmps[i].Height);
                Graphics bmpGraphics = Graphics.FromImage(tmp);
                Rectangle rect = new Rectangle(0, 0, bmps[i].Width, bmps[i].Height);
                bmpGraphics.DrawImage(bmps[i], rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, imageAttributes);
                imgs.Add(tmp);
            }
            return imgs;
        }

        public static Bitmap Recoloring(Bitmap bmp, float[] color)
        {
            Bitmap img = new Bitmap(bmp.Width, bmp.Height);
            float[][] colorMatrixElements = { new float[] {1,  0,  0,  0, 0},
                                                new float[] {0,  1,  0,  0, 0},
                                                new float[] {0,  0,  1,  0, 0},
                                                new float[] {0,  0,  0,  1, 0},
                                                new float[] {color[0], color[1], color[2], 0, 0}};
            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(
               colorMatrix,
               ColorMatrixFlag.Default,
               ColorAdjustType.Bitmap);
            Graphics bmpGraphics = Graphics.FromImage(img);
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            bmpGraphics.DrawImage(bmp, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, imageAttributes);
            return img;
        }
    }
}
