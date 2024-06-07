namespace Pocky.WASM.Models
{
    public class AuthResult
    {
        public bool Successed { get; set; }
        public string[] ErrorList { get; set; } = [];
    }
}
