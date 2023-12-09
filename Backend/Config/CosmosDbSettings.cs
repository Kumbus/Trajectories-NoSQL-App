namespace Backend.Config
{
    public class CosmosDbSettings
    {
        public string ConnectionString { get; set; }
        public string Key { get; set; }
        public string DatabaseName { get; set; }
        public string ContainerName { get; set; }
    }
}
