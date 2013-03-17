using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = System.Windows.Point;

namespace PRCommon {
    public static class Util {
        public static Bitmap ConvertToBitmap(this int[][] img) {
            Bitmap b = new Bitmap(img.Length, img[0].Length);
            for (int i = 0; i < img.Length; i++) {
                for (int j = 0; j < img[0].Length; j++) {
                    int val = img[i][j];
                    if (val < 0) {
                        val = (int)Math.Abs(val);
                    }
                    b.SetPixel(i, j, Color.FromArgb((val / 20) % 255, (val / 10) % 255, val % 255));
                }
            }
            return b;
        }

        public static double Variance(this List<double> data) {
            int items = data.Count();
            int i;
            double variance;
            double[] deviation = new double[items];

            double mean = data.Average();

            for (i = 0; i < items; i++) {
                deviation[i] = Math.Abs((data[i] - mean));
            }
            variance = deviation.Average();

            return variance;
        }


        public static Dictionary<string, double> Normalize(this Dictionary<string, double> dict, double totalVal = int.MinValue) {
            if (totalVal == int.MinValue) {
                totalVal = dict.Sum(i => i.Value);
            }
            foreach (var a in dict.Keys.ToList()) {
                dict[a] /= totalVal;
            }
            return dict;
        }

        public static double Sqrd(this double val) {
            return Math.Pow(val, 2);
        }
        public static double Sqrd(this int val) {
            return Math.Pow(val, 2);
        }

        public static double Distance(this Point p1, Point p2) {
            return Math.Sqrt((p2.X - p1.X).Sqrd() + (p2.Y - p1.Y).Sqrd());
        }

        public static Point Midpoint(this Point p1, Point p2) {
            return new Point((p2.X - p1.X) / 2.0, (p2.Y - p1.Y) / 2.0);
        }

        public static Point Difference(this Point p1, Point p2) {
            return new Point(p2.X - p1.X, p2.Y - p1.Y);
        }
        public static double GetAngle(this Point p) {
            double slope = p.Y / (double)p.X;
            if (p.X >= 0 && p.Y >= 0) {
                return Math.Atan(slope);
            } else if (p.X < 0 && p.Y >= 0) {
                return Math.PI + Math.Atan(slope);
            } else if (p.X < 0 && p.Y < 0) {
                return Math.PI + Math.Atan(p.Y / (double)p.X);
            } else if (p.X >= 0 && p.Y < 0) {
                return 2 * Math.PI + Math.Atan(slope);
            }
            throw new Exception("Quadrant not handled");
        }

        public static int Round(this double val) {
            return (int)Math.Round(val);
        }

        public static string MaxLabel(this Dictionary<string, double> dict) {
            double maxCertainty = double.MinValue;
            string bestLabel = "";
            foreach (var a in dict) {
                if (a.Value > maxCertainty) {
                    maxCertainty = a.Value;
                    bestLabel = a.Key;
                }
            }
            return bestLabel;
        }

        public static IEnumerable<double> SubsetEval(this int[][] input, List<IntPoint> pts){
            foreach (var p in pts) {
                yield return input[p.X][p.Y];
            }
        }
    }

    public struct IntPoint {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
