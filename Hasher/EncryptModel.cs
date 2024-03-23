using HttpEngine.Core;

namespace Hasher
{
    internal class EncryptModel : Model
    {
        public EncryptModel()
        {
            Routes = new()
            {
                "/encryption"
            };
        }

        public override ModelResult OnRequest(ModelRequest request)
        {
            var data = File("pages/encrypt.html", request);
            return new(data);
        }
    }
}
