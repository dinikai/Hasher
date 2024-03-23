using HttpEngine.Core;

namespace Hasher
{
    internal class Error404Model : Model
    {
        public override ModelResult OnRequest(ModelRequest request) => new ModelResult(File("pages/404.html", request));
    }
}
