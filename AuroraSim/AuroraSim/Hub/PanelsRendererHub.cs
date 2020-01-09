using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading;

namespace AuroraSim
{
    [HubName("rendererHub")]
    public class PanelsRendererHub : Hub<IPanelsRenderer>
    {
    }
}