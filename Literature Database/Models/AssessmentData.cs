namespace Literature_Database.Models
{
    public class AssessmentData
    {
        public string Industry { get; set; }
        public string Size { get; set; }
        public string SustainabilityAwareness { get; set; }
        public string WasteManagement { get; set; }
        public string Mobility { get; set; }
        public string TrainingDevelopment { get; set; }
        public string AIModel { get; set; }
        public string KeyStrengths { get; set; }
        public string AreasForImprovement { get; set; }
        public string RecommendedActions { get; set; }
        public int SustainabilityScore { get; set; }
        public string PressureOfAction { get; set; }
        public CompanyProfile CompanyProfile { get; set; }
        public SustainabilityAnalysisResult SustainabilityAnalysis { get; set; }

    }

    public class CompanyProfile
    {
        public string Industry { get; set; }
        public string NumberOfEmployees { get; set; }
        public string SustainabilityAwarenessAmongEmployees { get; set; }
        public string WasteManagementPractices { get; set; }
        public string EcoFriendlyTransportationPromotion { get; set; }
        public string EmployeeTrainingAndDevelopment { get; set; }
    }

    public class SustainabilityAnalysisResult
    {
        public List<string> KeyStrengths { get; set; }
        public List<string> AreasForImprovement { get; set; }
        public Dictionary<string, string> RecommendedActions { get; set; }
        public double SustainabilityScore { get; set; }
        public string PressureOfActionRating { get; set; }
    }
}
