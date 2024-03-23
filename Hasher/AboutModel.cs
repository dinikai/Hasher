using HttpEngine.Core;

namespace Hasher
{
    internal class AboutModel : Model
    {
        public AboutModel()
        {
            Routes = new()
            {
                "/about"
            };
        }

        public override ModelResult OnRequest(ModelRequest request)
        {
            var data = File("pages/about.html", request);
            return new(data);
        }
    }
}
