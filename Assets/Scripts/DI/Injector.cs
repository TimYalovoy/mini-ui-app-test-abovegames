using System.Collections.Generic;
using System.Linq;

namespace DI
{
    public class Injector
    {
        private List<IService> _services = new List<IService>();
        private List<IClient> _clients = new List<IClient>();

        public void UpdateServices(IEnumerable<IService> services)
        {
            _services = services.ToList();
        }

        public void UpdateClients(IEnumerable<IClient> clients)
        {
            _clients = clients.ToList();
        }

        public void AddService(IService service)
        {
            _services.Add(service);
        }

        public void AddClient(IClient client)
        {
            _clients.Add(client);
        }

        public void InjectAll()
        {
            foreach (var client in _clients)
            {
                var servicesToInject = _services.Where(service => !ReferenceEquals(service, client));

                foreach (var service in servicesToInject)
                {
                    client.Inject(service);
                }
            }
        }
    }
}