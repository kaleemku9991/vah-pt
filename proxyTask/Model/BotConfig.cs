namespace proxyTask.Model
{
    public class EndpointParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class BotConfig
    {
        public string WebhookUrl { get; set; }
        public object AuthorizationHeader { get; set; }
        public object[] CustomHeaders { get; set; }
        public EndpointParameter[] EndpointParameters { get; set; }
        public string SchemaVersion { get; set; }
        public int Timeout { get; set; }
        public object[] WebhookClientCertificates { get; set; }

        public string GetEndpointParameter(string name)
        {
            var parameter = EndpointParameters.FirstOrDefault(p => p.Name == name);
            return parameter?.Value;
        }
    }
}
