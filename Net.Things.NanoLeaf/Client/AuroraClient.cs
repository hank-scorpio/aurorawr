using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Ext;
using Infodev.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KeyValue = System.Collections.Generic.Dictionary<string, object>;

namespace Net.Things.NanoLeaf
{

    public class AuroraClient : IDisposable
    {
        internal AuroraClient(AuroraEndPoint ep)
              => EndPoint = ep;

        public AuroraEndPoint EndPoint { get; }

        HttpClient http;
        HttpClient Http
            => http ?? (http = new HttpClient() { BaseAddress = EndPoint.Uri });

        AuroraUdpStreamer udpStreamer;
        public AuroraUdpStreamer UdpStreamer
            => udpStreamer ?? (udpStreamer = new AuroraUdpStreamer(this)); 

        public AuroraClient WithAuthToken(string token)
        {
            Http.BaseAddress = new Uri(EndPoint.Uri, $"./{token}/");
            return this;
        }

        public async Task<string> GetAuthToken()
        {
            var r = (await Http.Post("new", ""));
            if (!r.IsSuccessStatusCode) return null;
            string token = JToken.Parse(await r.Content.ReadAsStringAsync())["auth_token"].Value<string>();
            return token;
        }



        async void PutState(Level level, string name, int value)
        {
            var json = new KeyValue
            {
                [name.ToLower()] = new { value = level.Clamp(value) }
            };

            var r = await Put("state", json);

            if (r.StatusCode != HttpStatusCode.NoContent)
            {
                throw new HttpRequestException($"Unexpected status code: {r.StatusCode}");
            }
            state = null;
        }



        static string ToJson(object obj, Formatting formatting = Formatting.None)
            => JObject.FromObject(obj).ToString(formatting);

        async Task<T> Get<T>(string route, string jsonPath = "")
            => await Http.Get(route).Json<T>(jsonPath);

        async Task<HttpResponseMessage> Put(string route, object body, string jsonPath = "")
            => await Http.Put(route, ToJson(body));

        async Task<T> Put<T>(string route, object body, string jsonPath = "")
        {
            var msg = await Http.Put(route, ToJson(body));

            using (var stream = new JsonTextReader(new StreamReader(await msg.Content.ReadAsStreamAsync())))
            {
                var token = await JToken.ReadFromAsync(stream);
                var select = token.SelectToken(jsonPath);
                return select.ToObject<T>();
            }
        }



        public IEnumerable<Tile> Tiles
            => Layout.Tiles;

        public async Task<Layout> GetLayout()
            => layout = await Get<Layout>("panelLayout/layout");

        public async Task<State> GetState()
            => state = await Get<State>("state");

        //new { write = new { command = "display", version = "1.0", animType = "extControl"} }

        Layout layout;
        public Layout Layout => layout ?? GetLayout().Result;

        State state;
        public State State => state ?? GetState().Result;


        public int Brightness
        {
            get => State.Brightness;
            set => PutState(State.Brightness, nameof(Brightness), value);
        }
        public int Hue
        {
            get => State.Hue;
            set => PutState(State.Hue, nameof(Hue), value);
        }
        public int Saturation
        {
            get => State.Sat;
            set => PutState(State.Sat, nameof(State.Sat), value);
        }

        public async Task<bool> SetExternalControlMode()
        {
            var r = await Put<dynamic>("effects", new
            {
                write = new
                {
                    command = "display",
                    animType = "extControl"
                }
            });

            if (r.streamControlProtocol != "udp")
            { 
                return false;
            }
            int port = r.streamControlPort;
            UdpStreamer.Port = port;
            return true;
        }

        public void Dispose()
        {
            http?.Dispose();
            udpStreamer?.Dispose();
        }
    }
}
