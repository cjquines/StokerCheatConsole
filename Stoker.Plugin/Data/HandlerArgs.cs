namespace Stoker.Plugin.Data
{
    public class HandlerArgs
    {
        public List<object?> Arguments { get; set; } = [];
        public Dictionary<string, object?> Options { get; set; } = [];
        public string[] UnparsedArgs { get; set; } = [];
        public string? SubCommand { get; set; } = null;
    }
}
