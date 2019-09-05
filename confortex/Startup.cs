using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Confortex.Startup))]
namespace Confortex
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
