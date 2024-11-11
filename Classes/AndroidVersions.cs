namespace com.companyname.navigationgraph9net9.Classes
{
    public class AndroidVersions
    {
        public string AndroidName { get; set; }
        public string AndroidBuildVersion { get; set; }
        public string AndroidCodeName { get; set; }
        public string AndroidApiNumber { get; set; }

        public AndroidVersions(string name, string buildVersion, string codename, string apiNumber)
        {
            AndroidName = name;
            AndroidBuildVersion = buildVersion;
            AndroidCodeName = codename;
            AndroidApiNumber = apiNumber;
        }
    }
}