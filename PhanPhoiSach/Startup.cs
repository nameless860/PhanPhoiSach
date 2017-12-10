using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PhanPhoiSach.Startup))]
namespace PhanPhoiSach
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
