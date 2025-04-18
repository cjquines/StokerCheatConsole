namespace Stoker.Base.Data
{
    public class HandlerArgs
    {
        public Dictionary<string, object?> Arguments { get; set; } = [];
        public Dictionary<string, object?> Options { get; set; } = [];
        public string[] UnparsedArgs { get; set; } = [];
        public string? SubCommand { get; set; } = null;
    }
}
