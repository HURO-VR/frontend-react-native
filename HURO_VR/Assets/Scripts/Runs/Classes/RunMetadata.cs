public partial class RunMetadata
{
    public string uid { get; set; }
    public string dateCreated { get; set; }
    public string status { get; set; }
    public string simID { get; set; }
    public bool starred { get; set; }
    public string runID { get; set; }
    public string name { get; set; }

    public int serverHits = 0;

    public string errorMessage { get; set; }
    public RunData data { get; set; }

}