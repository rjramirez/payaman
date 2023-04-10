namespace Common.DataTransferObjects.AppSettings
{
    public class SecurityGroup
    {
        //TODO: Customize role using securitygroup based on project requirments 
        public string[] ApplicationSupport { get; set; }

        public string[] AllowedGroups {
            get
            {
                //TODO: For multiple security group setup, use array.Concat(arr).ToArray()
                return ApplicationSupport;
            }
        }
    }
}
