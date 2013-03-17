using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRCommon {
    public class Feature {
        public int CreationIndex { get; set; }

        private static int creationCounter = 0;

        public Feature() {
            this.PastEvals = new Dictionary<string, PastValuesVec>();
            this.SuccessRate = new FeatureSuccess();
            this.CreationIndex = creationCounter++;
        }

        public FeatureSuccess SuccessRate { get; set; }

        List<double> LastEval { get; set; }

        public double Interestingness {
            get {
                //return this.SuccessRate.SuccessRate.Select(i => i.Value.Average()).Max();
                return this.SuccessRate.LabelSuccess.Select(i => i.Value.LastN()).Average();
                //return this.SuccessRate.Overall.LastN();
            }
        }

        public int DataSeen {
            get {
                return this.SuccessRate.TrialCounter;
            }
        }

        public int NumberOfPoints {
            get {
                return 1;
                //return Func.GetPoints().Count();
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
                if (a.Value[0].Count < thresholdVal) {
                    return false;
                }
            }
            return true;
        }

        public double? Attractiveness {
            get {
                //List<double> localVars = new List<double>();
                List<double> centers = new List<double>();
                foreach (var a in PastEvals) {
                    //var sd = a.Value.Variance();
                    //localVars.Add(sd);
                    centers.Add(a.Value.AveragePosition().Average());
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
                        var dist = PastEvals[l1].Distance(PastEvals[l2].Select(k => k.Average()).ToList());
                        distanceBetweenNodes.Add(dist);
                    }
                    characteristicDistances.Add(PastEvals[l1].Variance());
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
        
        public IEvalFunc Func { get; set; }
        public Dictionary<string, double> LabelCertainty { get; set; }
        public Dictionary<string, double> Test(int[][] input) {
            this.LastEval = Func.Eval(input);
            double totalVal = 0;
            this.LabelCertainty = new Dictionary<string, double>();
            if (PastEvals == null) return null;
            foreach (var a in PastEvals) {
                var compareVal = a.Value.Compare(this.LastEval);
                ///Not working for some reason
                //if (SuccessRate.LabelSuccess.ContainsKey(a.Key)
                //    && SuccessRate.LabelSuccess[a.Key].Count() > 2) {
                //    compareVal *= (SuccessRate.LabelSuccess[a.Key].LastN() * 10);
                //}
                LabelCertainty[a.Key] = compareVal;
                totalVal += compareVal;
            }
            if (totalVal == 0) return null;
            return LabelCertainty.Normalize(totalVal);
        }


        public Dictionary<string, PastValuesVec> PastEvals { get; set; }

        public void Train(string label) {
            if (PastEvals.ContainsKey(label)) {
                PastEvals[label].Add(LastEval);
            } else {
                PastEvals[label] = new PastValuesVec(LastEval);
            }
            if (!LabelCertainty.ContainsKey(label)) return;
            this.SuccessRate.Trial(label, LabelCertainty);
        }
    }
}

