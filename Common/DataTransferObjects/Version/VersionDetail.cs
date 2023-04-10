namespace Common.DataTransferObjects.Version
{
    public static class VersionDetail
    {
        public static string DisplayVersion()
        {
            System.Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
