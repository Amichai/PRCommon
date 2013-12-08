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

        public static string BestGuess(this Dictionary<string, double> prob) {
            double bestEval = 0;
            string bestGuess = null;
            if (prob == null) return null;
            foreach (var p in prob) {
                if (p.Value > bestEval) {
                    bestEval = p.Value;
                    bestGuess = p.Key;
                }
            }
            return bestGuess;
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


        public static Dictionary<string, double> Normalize(this Dictionary<string, double> dict, double? totalVal) {
            if (totalVal == null) {
                totalVal = dict.Sum(i => i.Value);
            }
            if (totalVal == 0) return null;
            foreach (var a in dict.Keys.ToList()) {
                dict[a] /= totalVal.Value;
            }
            return dict;
        }

        public static Dictionary<string, double> Normalize(this Dictionary<string, int> dict, double totalVal = int.MinValue) {
            var output = new Dictionary<string, double>();
            if (totalVal == int.MinValue) {
                totalVal = dict.Sum(i => i.Value);
            }
            foreach (var a in dict.Keys.ToList()) {
                output[a] = dict[a] / (double)totalVal;
            }
            return output;
        }

        public static string MaxLabel(this Dictionary<string, int> dict) {
            string maxLabel = "";
            int maxVal = int.MinValue;
            foreach (var a in dict) {
                if (a.Value > maxVal) {
                    maxVal = a.Value;
                    maxLabel = a.Key;
                }
            }
            return maxLabel;
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

        public static int MaxIndex<T>(this IEnumerable<T> sequence)
    where T : IComparable<T> {
            int maxIndex = -1;
            T maxValue = default(T); // Immediately overwritten anyway

            int index = 0;
            foreach (T value in sequence) {
                if (value.CompareTo(maxValue) > 0 || maxIndex == -1) {
                    maxIndex = index;
                    maxValue = value;
                }
                index++;
            }
            return maxIndex;
        }
    }

    public struct IntPoint {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
