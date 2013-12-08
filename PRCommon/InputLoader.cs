using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRCommon   {
    public class InputLoader {
        private string filename;

        public string Filename {
            get { return filename; }
            set { filename = value; }
        }

        private string[] lines;

        public string[] Lines {
            get { return lines; }
            set { lines = value; }
        }

        public int Width {
            get {
                return 28;
            }
        }

        public int Height {
            get {
                return 28;
            }
        }

        public void LoadFile(string filename) {
            this.Filename = filename;
            this.Lines = File.ReadAllLines(filename);
        }

        public int[][] AccessElement(int at, out string label) {
            var a = Lines.ElementAt(at);
            label = a.Split(',').First();
            var data = a.Split(',').Skip(1).Select(v => int.Parse(v));
            int[][] inputData = new int[28][];
            for (int i = 0; i < 28; i++) {
                inputData[i] = new int[28];
            }

            for (int i = data.Count() - 1; i >= 0; i--) {
                inputData[i % 28][i / 28] = data.ElementAt(i);
            }
            return inputData;
        }

        public IEnumerable<Tuple<int[][], string>> AllElements() {
            int i = 0;
            string l;
            while (true) {
                i = i % 25000;
                if (i == 0) i++;
                var a = AccessElement(i++, out l);
                yield return new Tuple<int[][], string>(a, l);

            }
        }
    }
}
