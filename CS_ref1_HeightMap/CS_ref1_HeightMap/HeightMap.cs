using System;
using System.Drawing;
using System.IO;
using System.Text;


namespace CS_ref1_HeightMap
{
    class HeightMap
    {

        const int grid = 1201;
        int[,] HeightData;
        int elevationThreshold;

        public int ElevationThreshold { get => elevationThreshold; set => elevationThreshold = value; }

        public HeightMap()
        {
            HeightData = new int[grid, grid];
        }

        public static HeightMap Parse(string path) //reading bytes from .hgt file (BigEndian)
        {

            HeightMap hm = new HeightMap();
            FileStream fs;
            int[] buffer = new int[2];
            using (fs = new FileStream(path, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs, Encoding.BigEndianUnicode))
                {
                    for (int i = 0; i < grid; i++)
                    {
                        for (int j = 0; j < grid; j++)
                        {
                            buffer[0] = br.ReadByte();
                            buffer[1] = br.ReadByte();
                            hm.HeightData[i, j] = buffer[0] << 8 | buffer[1];

                            if (hm.HeightData[i, j] == 32768) //if no data
                            {
                                hm.HeightData[i, j] = 0;
                                Console.WriteLine("No data recorded on {0};{1} coordinate.", i, j);
                            }
                        }
                    }
                }
            }
            return hm;
        }

        private int[] GetExtremalElevation() //get maximum and minimum height 
        {
            int[] result = new int[2];
            int min = int.MaxValue;
            int max = int.MinValue;

            for (int i = 0; i < this.HeightData.GetLength(0); i++)
            {
                for (int j = 0; j < this.HeightData.GetLength(1); j++)
                {
                    if (this.HeightData[i, j] < min)
                        min = this.HeightData[i, j];
                    if (this.HeightData[i, j] > max)
                        max = this.HeightData[i, j];
                }
            }

            result[0] = min;
            result[1] = max;

            return result;
        }

        public void SaveToBitmap(string path) //save bitmap as output.bmp
        {
            Bitmap bmp = new Bitmap(grid, grid);
            Color c = Color.FromArgb(0, 0, 0);

            int[] ext = this.GetExtremalElevation();
            float interval = (float)(ext[1] - ext[0]);
            float divider = interval / 255;

            for (int i = 0; i < grid; i++)
            {
                for (int j = 0; j < grid; j++)
                {
                    if (HeightData[i, j] > this.ElevationThreshold)
                    {
                        c = Color.FromArgb(0, (int)(255 - (HeightData[i, j] / divider)), 0);
                    }
                    else
                        c = Color.FromArgb(10, 10, 200);

                    bmp.SetPixel(j, i, c);

                }
            }
            bmp.Save(path);
        }

    }
}
