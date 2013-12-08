using MyLogger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PRCommon {
    public class Feature {
        public int CreationIndex { get; set; }

        private static int creationCounter = 0;

        private static Random rand = new Random();

        public enum FType { PixelEval, unknown, PixelDiff, PixelProjection, SymmetricalPixelDiff, PixelSum, PixelQuot, SymmetricalPixelQuot, PixelProd, SymmetricalPixelSum, PixelProjectionVarLength }

        public FType FeatureType { get; set; }

        public Feature() {
            this.FeatureType = FType.unknown;
            this.LastEval = null;
            this.PastEvals = new Dictionary<string, PastValues>();
            this.SuccessRate = new FeatureSuccess();
            this.CreationIndex = creationCounter++;
            if (Logger.Inst.GetString("compareValExponent") == "random") {
                this.compareValExponent = rand.Next(0, 2) + rand.NextDouble();
            } else {
                this.compareValExponent = Logger.Inst.GetDouble("compareValExponent");
            }
        }

        public void Serialize() {
            ///TODO: Test this function!!!
            XElement root = new XElement("Feature");
            root.Add(new XAttribute("Type", this.FeatureType.ToString()));
            foreach(var p in PastEvals){
                var label = new XElement("Label", p.Key);
                label.Add(p.Value.Serialize());
                root.Add(label);
            }
        }

        public FeatureSuccess SuccessRate { get; set; }

        double? LastEval { get; set; }

        /// <summary>
        /// Average success over plast 100 trials for each output label
        /// </summary>
        public double Interestingness {
            get {
                return 0;
                if (this.SuccessRate.LabelSuccess.Count() == 0) return 0;
                //return this.SuccessRate.SuccessRate.Select(i => i.Value.Average()).Max();
                //return this.SuccessRate.LabelSuccess.Select(i => i.Value.LastN()).Average();
                //return this.SuccessRate.Overall.LastN();
                var shortAve = this.SuccessRate.LabelSuccess.Select(i => i.Value.LastN(20)).Average();
                var longAve = this.SuccessRate.LabelSuccess.Select(i => i.Value.LastN()).Average();
                if (longAve == 0) return 0;
                return shortAve / (longAve) ;
            }
        }

        public int DataSeen {
            get {
                return this.SuccessRate.TrialCounter;
            }
        }

        public int NumberOfPoints {
            get {
                //return 1;
                return Projection.PointCount();
            }
        }

        public bool IsTrained {
            get {
                return Trained();
            }
        }

        public bool Trained(int thresholdVal = 5) {
            if (PastEvals.Count() == 0) return false;
            foreach (var a in PastEvals) {
                if (a.Value.Count < thresholdVal) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Eval variablility
        /// </summary>
        public double? Attractiveness {
            get {
                return 0;
                if (PastEvals.Count() == 0) return 0;
                //List<double> localVars = new List<double>();
                List<double> centers = new List<double>();
                foreach (var a in PastEvals) {
                    //var sd = a.Value.Variance();
                    //localVars.Add(sd);
                    centers.Add(a.Value.Average());
                }
                //return (centers.Variance()) / (localVars.Average() + 1e-6);
                //return centers.Select(i => Math.Abs(i)).Average();
                return centers.Variance();

                //return 0;

                ///Likelihood to mate. Unattractive features get purged.
                List<double> distanceBetweenNodes = new List<double>();
                List<double> characteristicDistances = new List<double>();
                List<string> allLabels = LabelCertainty.Keys.ToList();
                for (int i = 0; i < allLabels.Count(); i++) {
                    string l1 = allLabels[i];
                    for (int j = 0; j < allLabels.Count(); j++) {
                        if (i == j) continue;
                        string l2 = allLabels[j];
                        var dist = PastEvals[l1].Average() - PastEvals[l2].Average();

                        //.Distance(PastEvals[l2].Select(k => k.Average()).ToList());
                        distanceBetweenNodes.Add(dist);
                    }
                    characteristicDistances.Add(PastEvals[l1].Moment(1));
                }
                if (characteristicDistances.Count() == 0
                    || distanceBetweenNodes.Count() == 0) {
                    //throw new Exception();
                    return null;
                }

                var denom = characteristicDistances.Average();
                if (denom == 0) {
                    denom = .0001;
                }
                var attract = distanceBetweenNodes.Average() / denom;
                return attract;
            }
        }

        private double? compareValExponent = null;

        public IEvalFunc Projection { get; set; }
        public Dictionary<string, double> LabelCertainty { get; set; }
        public Dictionary<string, double> Test(int[][] input) {
            this.LastEval = Projection.Eval(input);
            double totalVal = 0;
            this.LabelCertainty = new Dictionary<string, double>();
            if (PastEvals == null) return null;
            foreach (var a in PastEvals) {
                var compareVal = a.Value.Compare(this.LastEval.Value);
                compareVal = Math.Pow(compareVal, compareValExponent.Value);
                ///Not working for some reason
                //if (SuccessRate.LabelSuccess.ContainsKey(a.Key)
                //    && SuccessRate.LabelSuccess[a.Key].Count() > 2) {
                //    compareVal *= (SuccessRate.LabelSuccess[a.Key].LastN() * 10 + 1);
                //}
                //if (SuccessRate.SuccessRate.ContainsKey(a.Key)) {
                //    compareVal *= (SuccessRate.SuccessRate[a.Key].Average() + .10) *  (SuccessRate.LabelSuccess[a.Key].LastN() + .1) * 10;
                //}
                /*
                if (SuccessRate.LabelSuccess.ContainsKey(a.Key) && SuccessRate.LabelSuccess[a.Key].Count() > 1) {
                    compareVal *= SuccessRate.LabelSuccess[a.Key].LastN2 * 10;
                }*/

                LabelCertainty[a.Key] = compareVal;
                totalVal += compareVal;

            }
            if (totalVal == 0) return null;
            var normalized = LabelCertainty.Normalize(totalVal);
            return normalized;
        }

        private Dictionary<string, double> BoosterTest(int[][] input, double? lastEval) {
            this.LastEval = lastEval;
            this.LabelCertainty = new Dictionary<string, double>();
            if (PastEvals == null) return null;
            foreach (var a in PastEvals) {
                var compareVal = a.Value.Compare(this.LastEval.Value);
                LabelCertainty[a.Key] = compareVal;
            }
            return LabelCertainty;
        }

        public Dictionary<string, PastValues> PastEvals { get; set; }

        public void Train(string label, int[][] input) {
            if (LastEval == null) {
                return;
            }

            string guess = "";
            if (LabelCertainty.ContainsKey(label)) {
                guess = LabelCertainty.BestGuess();
                this.SuccessRate.Trial(label, LabelCertainty, guess);
            }

            if (guess != label) {
                if (PastEvals.ContainsKey(label) && PastEvals[label].Count < 10) {
                    PastEvals[label].Add(LastEval.Value);
                } else if(!PastEvals.ContainsKey(label)) {
                    PastEvals[label] = new PastValues(LastEval.Value);
                }
            } else {

            }
        }

        private void Train(string label, double? lastEval) {
            this.LastEval = lastEval;
            if (LastEval == null) {
                return;
            }
            if (PastEvals.ContainsKey(label)) {
                PastEvals[label].Add(LastEval.Value);
            } else {
                PastEvals[label] = new PastValues(LastEval.Value);
            }
        }
    }
}

