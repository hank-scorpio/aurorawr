using System;


public static class Is<T> where T : IEquatable<T>
{
    public static Predicate<T> Eq(IEquatable<T> k)
        => k.Equals;


    public static Predicate<T> Default
        => default(T).Equals;
}


#region ===[ Struct   ]===
public static class Struct
{
    //	/// <summary>
    //	/// converts byte[] to struct
    //	/// </summary>
    //	public static T GetBytes<T>(byte[] rawData, int position)
    //	{
    //		int rawsize = Marshal.SizeOf(typeof(T));
    //		if (rawsize > rawData.Length - position)
    //			throw new ArgumentException("Not enough data to fill struct. Array length from position: " + (rawData.Length - position) + ", Struct length: " + rawsize);
    //		IntPtr buffer = Marshal.AllocHGlobal(rawsize);
    //		Marshal.Copy(rawData, position, buffer, rawsize);
    //		T retobj = (T)Marshal.PtrToStructure(buffer, typeof(T));
    //		Marshal.FreeHGlobal(buffer);
    //		return retobj;
    //	}

    /// <summary>
    /// converts a struct to byte[]
    /// </summary>
    public static byte[] GetBytes<T>(T obj)
    {
        int sz = Marshal.SizeOf<T>(obj);
        byte[] data = new byte[sz];
        IntPtr ptr = Marshal.AllocHGlobal(sz);

        Marshal.StructureToPtr(obj, ptr, false);
        Marshal.Copy(ptr, data, 0, sz);
        Marshal.FreeHGlobal(ptr);
        return data;
    }


}


#endregion




#region ===[ Seq   ]===

public static class Seq
{
    //:: Empty ::// 
    ///:	-->| x 					|--> { }	
    public static IEnumerable<T> Empty<T>()
        => Array.Empty<T>();

    //:: Items ::// 			
    ///:	-->| x1, x2, x3, ...	|--> { x1, x2, x3, ... }
    ///:	-->| seq				|--> seq
    ///:	-->| NULL				|--> { }	
    public static IEnumerable<T> Items<T>(params T[] items)
        => items ?? Empty<T>();

    //:: Single ::// 
    ///:	-->| x 					|--> { x }	
    ///:	-->| DEFAULT 			|--> { NULL }
    public static IEnumerable<T> Single<T>(T item)
        => Items(item);

    //:: SingleOrEmpty ::// 
    ///:	-->| x 					|--> Single(x)
    ///:	-->| DEFAULT 			|--> Empty
    public static IEnumerable<T> SingleOrEmpty<T>(T item, Predicate<T> emptyWhen = null)
        => default(T).Equals(item) ? Single(item) : Empty<T>();

    //:: OrEmpty ::// 
    ///:	
    ///			-->| <dflt>		|--> Empty  -- when seq i
    ///:		-->| **			|--  **		-- otherwise:
    public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> seq)
        => seq ?? Empty<T>();


    //
    // Numerics
    //


    //:: For ::// 			
    ///:	-->| 5				|--> { 5, 6, 7, 8, ... }
    ///:	-->| 10,  end: 20	|--> { 10, 11, 12, .., 19, 20 }
    ///:	-->| 500, step: 100	|--> { 500, 600, 700, .800, ... }
    public static IEnumerable<int> For(int start = 0, int end = int.MaxValue, int step = 1)
    {
        do { yield return start; }
        while ((start += step) <= end);
    }

    //:: N ::// 			
    ///:	-->| 5				|--> { 0, 1, 2, 3, 4, ... }
    ///:	-->| -2,			|--> { -2, -1, 0, 1, 2, ... }
    ///:	-->| 44, step: 2	|--> { 44, 46, 48, 50, ... }
    public static IEnumerable<int> N
        => For(0, step: 1);


    //:: Indices ::// 			
    ///:	-->| { ... | n: 50 }  |--> { 0..49 }
    public static IEnumerable<int> Indices<T>(this IEnumerable<T> seq)
        => Seq.N.Take(seq.Count());





    public static IEnumerable<T> Yield<T>(this Func<T, T> next, T seed = default(T))
    {
        while (true) yield return seed = next(seed);
    }
    public static IEnumerable<T> Yield<T>(this Func<T, T> next, Func<T> fst)
        => fst().Apply(seed => Seq.Single(seed).Concat(Yield<T>(next, seed)));

    public static IEnumerable<T> Yield<T>(this Func<T> next)
        => Yield<T>(_ => next());



    //
    // Set Operators
    //



    public static IEnumerable<T> Concat<T>(this IEnumerable<T> head, params T[] tail)
        => Concat(head, Items(tail));

    public static IEnumerable<T> Concat<T>(this IEnumerable<T> head, params IEnumerable<T>[] tails)
        => tails.Aggregate(head.OrEmpty(), Enumerable.Concat);

    public static IEnumerable<T> ConcatRev<T>(this IEnumerable<T> tail, params T[] head)
        => Concat(head, tail);



    public static IEnumerable<Y> ZipNext<X, Y>(this X[] arr, Func<X, X, Y> selector)
    {
        Y[] ret = new Y[arr.Length - 1];
        for (int i = 0; i < ret.Length; i++)
        {
            ret[i] = selector(arr[i], arr[i + 1]);
        }
        return ret;
    }



    public static IEnumerable<Y> Select<X, Y>(this IEnumerable<X> seq, Func<X, int, Y> selector)
    {
        int i = 0;
        return seq.Select(x => selector(x, i++));
    }
    public static IEnumerable<Y> Select<X, Y>(this X[] arr, Func<X, int, Y> selector)
    {
        int i = 0;
        return arr.Select(x => selector(arr[i], i++));
    }
    public static IEnumerable<Y> Window<X, Y>(this X[] arr, int n, Func<ArraySegment<X>, Y> selector)
    {
        for (int i = 0; i < arr.Length - n; i++)
            yield return selector(new ArraySegment<X>(arr, i, n));
    }

    public static IEnumerable<T> Map<T>(this T[] arr, IEnumerable<int> idx)
        => from i in idx select arr[i];

    public static IEnumerable<T> Map<T>(this IEnumerable<T> seq, IEnumerable<int> idx)
        => seq.ToArray().Map(idx);

    public static IEnumerable<Y> ZipNext<X, Y>(this IEnumerable<X> seq, Func<X, X, Y> selector)
        => seq.ToArray().ZipNext(selector);

    public static IOrderedEnumerable<T> Order<T>(this IEnumerable<T> seq, IComparer<T> comparer = null)
        => seq.OrderBy(x => x, comparer ?? Comparer<T>.Default);
}

#endregion

#region ===[ Obj   ]===
public static class Obj
{

    public static X Do<X>(this X x, Action body) { body(); return x; }
    public static X Do<X>(this X x, Action<X> body) { body(x); return x; }

    public static Y Apply<X, Y>(this X x, Func<X, Y> fn)
        => fn(x);

    public static IEnumerable<X> Seq<X>(this X x)
        => new X[] { x };


    public static T New<T>(params object[] args)
        => (T)Activator.CreateInstance(typeof(T), args);

    public static IEnumerable<NV> Properties(this object obj)
        => obj.Properties(NV.Create);

    public static IEnumerable<T> Properties<T>(this object obj, Func<string, object, T> selector)
        => obj.GetType().GetProperties().Select(p => selector(p.Name, p.GetValue(obj)));

    public static IEnumerable<NV> Fields(this object obj)
        => obj.Fields(NV.Create);

    public static IEnumerable<T> Fields<T>(this object obj, Func<string, object, T> selector)
        => obj.GetType().GetFields().Select(p => selector(p.Name, p.GetValue(obj)));

}
#endregion

#region ===[ NV    ]===

public abstract class NV
{
    public virtual string Name { get; protected set; }
    public virtual object Value { get; protected set; }

    public string ToString(string delim) => $"{Name}{delim}{Value}";

    public static NV Create(string name, object val) => new NamedValue(name, val);
}

public class NamedValue : NV
{
    public NamedValue(string name, object value)
    {
        Name = name;
        Value = value;
    }
}

public static class NVExt
{
    public static string ToQueryString(this IEnumerable<NV> vals)
        => vals.Select(x => $"{x.Name.ToLower()}={HttpUtility.UrlEncode(x.Value?.ToString())}").Join("&");
}
#endregion

#region ===[ Num    ]===

public static partial class VarInt
{

    public static void WriteVarInt(this BinaryWriter writer, UInt32 value)
        => writer.WriteVarInt((Int32)value);

    public static void WriteVarInt(this BinaryWriter writer, Int32 value)
    {
        while (value >= 0x80)
        {
            writer.Write((Byte)(value | 0x80));
            value >>= 7;
        }
        writer.Write((Byte)value);
    }



    public static UInt32 ReadVarUInt32(this BinaryReader reader)
        => (UInt32)reader.ReadVarUInt32();


    public static Int32 ReadVarInt32(this BinaryReader reader)
    {
        Int32 value = 0;
        Int32 shift = 0;

        Byte b;

        do
        {
            b = reader.ReadByte();
            value |= (b & 0x7F) << shift;
            shift += 7;

        } while ((b & 0x80) != 0);

        return value;
    }
}






public static partial class Num
{

    //public static double Sqrt(this double x) => Math.Sqr

    public static double Sq(this double x) => x * x;
    //public static double Sqrt(this double x) => Math.Sqr

    public static double Hypotenuse2(double a)
        => Hypotenuse2(a, a);
    public static double Hypotenuse(double a)
        => Hypotenuse(a, a);

    public static double Hypotenuse2(double a, double b)
        => a * a + b * b;
    public static double Hypotenuse(double a, double b)
        => Sqrt(Hypotenuse2(a, b));

    public static double Cathetus2(double c, double a)
        => c * c - a * a;
    public static double Cathetus(double c, double a)
        => Sqrt(Cathetus2(c, a));

    public static double Cathetus2(double c)
        => c * c / 2;
    public static double Cathetus(double c)
        => Sqrt(Cathetus2(c));
}

#endregion

// Text

#region ===[ Chr   ]===
public static class Chr
{
    public static IReadOnlyCollection<char> Digit { get; } = Range('0', '9').ToArray();
    public static IReadOnlyCollection<char> Upper { get; } = Range('a', 'z').ToArray();
    public static IReadOnlyCollection<char> Lower { get; } = Range('A', 'Z').ToArray();
    public static IReadOnlyCollection<char> Letter { get; } = Upper.Concat(Lower).ToArray();
    public static IReadOnlyCollection<char> Base64 { get; } = Seq.Concat(Upper, Lower, Digit, "+/").ToArray();
    public static IReadOnlyCollection<char> Printable { get; } = Range(0x20, 0x7F).ToArray();


    public static IEnumerable<char> Range(char start, char end, int step = 1)
        => Range((byte)start, (byte)end, step);
    public static IEnumerable<char> Range(byte start, byte end, int step = 1)
        => Seq.For(start, end, step).Select(c => (char)c);
    public static string String(this IEnumerable<char> chars)
        => new String(chars.ToArray());
    public static string Repeat(this char c, int n)
        => new String(c, n);
}

#endregion

#region ===[ Str   ]===

public static class StrExt
{
    public static T AsJson<T>(this string str, string path)
           => str.AsJson(path).ToObject<T>();
    public static JToken AsJson(this string str)
           => JToken.Parse(str);
    public static JToken AsJson(this string str, string path)
        => str.AsJson().SelectToken(path);

    public static String ToJson(this object obj)
        => JToken.FromObject(obj).ToString();

    public static String ToBase64(this string str, Encoding enc = null)
        => Convert.ToBase64String(str.Bytes(enc));

    const string EolRegex = @"(\r\n?)|(\n)";
    const string EowRegex = @"([\s\t]+)|(\r\n?)|\n";

    public static string[] Lines(this String str) => Regex.Split(str, EolRegex, RegexOptions.Compiled | RegexOptions.Multiline);
    public static string[] Words(this String str) => Regex.Split(str, EowRegex, RegexOptions.Compiled | RegexOptions.Multiline);

    public static string Prepend(this String str, string prefix) => str == null ? null : (prefix + str);
    public static string Append(this String str, string suffix) => str == null ? null : (str + suffix);
    public static string Surround(this String str, string prefix, string suffix) => str == null ? null : (prefix + str + suffix);
    public static string Surround(this String str, string fix) => str.Surround(fix, fix);

    public static string Join(this IEnumerable<String> strings, string sep) => strings == null ? null : (String.Join(sep, strings));

    public static bool IsEmpty(this String str) => str == String.Empty;

    public static string NullIfEmpty(this String str) => str.IsEmpty() ? null : str;

    public static string First(this String str, int n = 1) => n <= 0 ? null : str.Substring(0, n);
    public static string Last(this String str, int n = 1) => n <= 0 ? null : str.Substring(str.Length - 1 - n);

    public static string Skip(this String str, int n = 1) => (n <= 0 || n >= str.Length) ? null : str.Substring(n);
    public static string SkipLast(this String str, int n = 1) => (n <= 0 || n >= str.Length) ? null : str.Substring(0, str.Length - 1 - n);


    public static string Before(this String str, string sub, int start = 0)
        => str.First(str.IndexOf(sub, start));

    public static string After(this String str, string sub, int start = 0)
        => str.Skip(str.IndexOf(sub, start))?.Skip(sub.Length);

    public static bool Like(this String str, string expr) => str.Matches($"^{expr.Replace("*", "(.*)")}$");

    //public static bool StartMatches(this String str, string regex) => Regex.IsMatch(str, regex);

    public static bool Matches(this String str, string regex) => Regex.IsMatch(str, regex);

}
public static class StringBytesConverter
{
    public static Byte[] Bytes(this string str, Encoding enc = null)
        => (enc ?? Encoding.Default).GetBytes(str);

    public static String String(this IEnumerable<byte> bytes, Encoding enc = null)
        => (enc ?? Encoding.Default).GetString(bytes.ToArray());
}

#endregion

#region ===[ Lines ]===
public static class Lines
{
    public static IEnumerable<string> WriteLines(this IEnumerable<string> lines, string file)
        => lines.Do(x => File.WriteAllLines(file, lines.ToArray()));


}
#endregion



#region ===[ Rnd   ]===

public static partial class RndE
{

    public static T ElementAtRandom<T>(this ICollection<T> coll)
    => Rnd.Item(coll);

    public static T RemoveRandom<T>(this IList<T> list)
    => Rnd.Draw(list);
}
public static partial class Rnd
{

    /// Random Service

    internal static class Static
    {

        static int seed
            = Environment.TickCount;

        internal static ThreadLocal<Random> ThreadLocal { get; }
            = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));
    }

    public static Random Gen
        => Static.ThreadLocal.Value;


    /// Value Generators


    public static double Probability()
        => Gen.NextDouble();

    public static bool Binary(double p = 0.5d)
        => Probability() > p;

    public static int Ordinal(int min = 0, int max = int.MaxValue)
        => Gen.Next(max);

    public static int Integer()
        => Gen.Next(int.MinValue);

    public static int Integer(int min, int max = int.MaxValue)
        => Gen.Next(min, max);

    public static uint Integer(uint min = uint.MinValue, uint max = uint.MaxValue)
        => unchecked((UInt32)Integer((Int32)min, (Int32)max));


    public static T Item<T>(this IEnumerable<T> coll)
        => coll.ElementAt(Ordinal(coll.Count() - 1));

    public static T Draw<T>(this IList<T> coll)
        => coll.ElementAt(Ordinal(coll.Count() - 1));


    /// Seq Generators


    public static IEnumerable<int> Integers(int min = int.MinValue, int max = int.MaxValue)
        => Seq.Yield((() => Integer(min, max)));

    public static IEnumerable<double> Probabilities
        => Seq.Yield(Probability);

    public static IEnumerable<T> Items<T>(this IEnumerable<T> set)
        => Seq.Yield(() => Item(set));


    /// Seq Operators




    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> lst)
        => lst.ToArray();

    public static IEnumerable<T> Ordinals<T>(this IEnumerable<T> lst)
        => lst.ToArray();


    public static IEnumerable<T> Shuffle<T>(this ICollection<T> lst)
        => lst.OrderBy(x => x);

    public static void Shuffle<T>(this T[] arr)
        => Array.Sort(arr, Comparer);


    public static IComparer Comparer = new RandomComparer();



    public class RandomComparer : IComparer
    {
        public int Compare(object x, object y)
            => Rnd.Integer();
    }



}

#endregion

#region ===[ Timed ]===

public static class TaskExt
{
    public static Task<Timed<T>> Timed<T>(this Task<T> t)
    {
        var sw = Stopwatch.StartNew();
        return t.ContinueWith(x => new Timed<T>(sw.Elapsed, x.Result));
    }
}

public class Timed<R>
{
    public TimeSpan Duration { get; }
    public R Result { get; }

    public Timed(TimeSpan duration, R result)
    {
        Duration = duration;
        Result = result;
    }
    public long Count => (Result as ICollection)?.Count ?? 1;
    public double PerSecond => Count / Duration.TotalSeconds;
}

#endregion


#region ===[ Files ]===

public static class Local
{
    public static DirectoryInfo Dir(string path)
        => new DirectoryInfo(path);

    public static DirectoryInfo Dir(Environment.SpecialFolder sf)
        => Dir(Environment.GetFolderPath(sf));

    public static DirectoryInfo MyDocuments
        => Dir(Environment.SpecialFolder.MyDocuments);
}

public static class FileSystemInfoExt
{
    public static IEnumerable<FileInfo> Files(this DirectoryInfo di, string pattern = "*.*")
        => di.EnumerateFiles(pattern);

    public static FileInfo File(this DirectoryInfo di, string pattern = "*.*")
        => di.Files(pattern).First();

    public static IEnumerable<string> Lines(this FileInfo fi)
        => fi.FullName.Apply(System.IO.File.ReadLines);

    public static byte[] Bytes(this FileInfo fi)
        => fi.FullName.Apply(System.IO.File.ReadAllBytes);

}

#endregion

#region ===[ Tcp   ]===

public static class Tcp
{
    public async static Task<TcpClient> Open(string host, int port)
    {
        TcpClient c = new TcpClient();
        await c.ConnectAsync(host, port);
        return c;
    }
    public async static Task<Stream> OpenStream(string host, int port)
        => (await Open(host, port)).GetStream();

    public async static Task<ConsoleClient> OpenConsole(string host, int port)
        => new ConsoleClient(await OpenStream(host, port));
}

public class ConsoleClient : IDisposable
{
    public StreamReader Reader { get; }
    public StreamWriter Writer { get; }

    public Stream Stream { get; }

    public ConsoleClient(Stream stream)
    {
        Stream = stream;
        Reader = new StreamReader(Stream);
        Writer = new StreamWriter(Stream);
        Writer.AutoFlush = true;
    }

    public async Task<string> Run(string cmd, bool throwException = false)
    {
        try
        {
            await Writer.WriteLineAsync(cmd);
            return await Reader.ReadLineAsync();
        }
        catch
        {
            return null;
        }
    }


    public void Dispose()
    {
        Writer?.Flush();
        Writer?.Dispose();
        Reader?.Dispose();
        Stream?.Dispose();
    }
}

#endregion

#region ===[ Http  ]===

public static class Http
{
    public static HttpClient New => new HttpClient();



    public static HttpClient At(string baseAddress)
        => At(New, baseAddress);

    public static HttpClient At(this HttpClient c, string baseAddress)
        => c.Do(x => x.BaseAddress = new Uri(baseAddress));

    public static HttpClient WithHeader(this HttpClient c, string name, object value)
        => c.Do(x => x.DefaultRequestHeaders.Add(name, value.ToString()));

    public static HttpClient WithAuth(this HttpClient c, string user, string pass)
        => c.WithHeader("Authorization", $"Basic {$"{user}:{pass}".ToBase64()}");


    public static HttpClient WithUserAgent(this HttpClient c, string userAgent = null)
        => c.WithHeader("User-Agent", userAgent ?? UserAgents.Default);

    static class UserAgents
    {
        public static String Default => Chrome();

        public static String Chrome
        (
            string os = "Windows NT 10.0; Win64; x64",
            string chrome = "55.0.2883.87",
            string webkit = "537.36"
        )
        => $@"Mozilla/5.0 ({os}) AppleWebKit/{webkit} (KHTML, like Gecko) Chrome/{chrome} Safari/{webkit}";
    }

    public static Task<HttpContent> Get(string url)
        => Http.New.Get(url);

    public static async Task<HttpContent> Get(this HttpClient c, string url)
        => (await c.GetAsync(url)).Content;

    public async static Task<HttpResponseMessage> Put(this HttpClient c, string url, HttpContent content)
    {
        var t = c.PutAsync(url, content);

        return (await t).EnsureSuccessStatusCode();
    }

    public static Task<HttpResponseMessage> Put(this HttpClient c, string url, object content, string type = text_plain)
        => c.Put(url, Content(content.ToString(), type));

    public static Task<HttpResponseMessage> Put(this HttpClient c, string url, JToken json)
        => c.Put(url, json, application_json);

    public static async Task<HttpContent> Post(this HttpClient c, string url, HttpContent content)
        => (await c.PostAsync(url, content)).Content;

    public static Task<HttpContent> Post(this HttpClient c, string url, object content, string type = text_plain)
        => c.Post(url, Content(content.ToString(), type));

    public static Task<HttpContent> Post(this HttpClient c, string url, JToken json)
        => c.Post(url, json, application_json);

    static StringContent Content(string content, string mediaType = application_json)
        => new StringContent(content, Encoding.UTF8, mediaType);

    const string text_plain = "text/plain";
    const string application_json = "application/json";

    public static Task<string> Text(this Task<HttpContent> c)
        => c.Result.ReadAsStringAsync();
    public static Task<byte[]> Bytes(this Task<HttpContent> c)
        => c.Result.ReadAsByteArrayAsync();
    public static Task<Stream> Stream(this Task<HttpContent> c)
        => c.Result.ReadAsStreamAsync();

    public static async Task<JsonReader> JsonReader(this Task<HttpContent> c)
        => new JsonTextReader(new StreamReader(await c.Stream()));

    public static Task<JArray> JsonArray(this Task<HttpContent> c)
        => c.Json<JArray>();

    public static async Task<JToken> Json(this Task<HttpContent> c, string jsonPath = "")
        => JToken.Load(await c.JsonReader()).SelectToken(jsonPath);

    public static async Task<T> Json<T>(this Task<HttpContent> c, string jsonPath = "")
        => (await c.Json(jsonPath)).ToObject<T>();


}
#region Url

// Define other methods and classes here
public class Url
{
    public class QS : Dictionary<string, object>
    {
        Url Url => url & this;
        Url url;
        public QS(Url url)
        {
            this.url = url;
        }
    }
    public enum Scheme
    {
        Http = 80,
        Https = 443,
    }


    public const string SchemeSuffix = "://";
    bool IsSchemeUrl => Enum.GetValues(typeof(Scheme)).Cast<Scheme>().Any(s => Value == SchemeUrl(s));
    public static Url SchemeUrl(Scheme s = Scheme.Http)
    => s.ToString().ToLower() + SchemeSuffix;

    public static Url Empty = String.Empty;

    readonly string Value;


    public const char PathSeparator = '/';
    public const char QueryPrefix = '?';
    public const char QueryArgSeparator = '&';
    public const char QueryKeyValueSeparator = '=';
    public const char HostPortSeparator = ':';

    public Url(string url)
    {
        Value = url;
    }

    public static implicit operator Url(string url) => new Url(url);
    public static implicit operator string(Url url) => url.Value;
    public static implicit operator Uri(Url url) => url.ToUri();
    public static Url operator /(Url url, string str) => url.AppendPath(str);
    public static Url operator &(Url url, string str) => url.AppendQuery(str);
    public static Url operator &(Url url, object obj) => url.AppendQuery(obj);
    public static Url operator &(Url url, QS qs) => url.AppendQuery(qs);
    public Uri ToUri() => new Uri(Value);
    public override string ToString() => Value;
    public static string Encode(string url) => HttpUtility.UrlEncode(url);

    public static Url Build(string hostname, Scheme scheme = Scheme.Http, int? port = null)
    => $"{SchemeUrl(scheme)}{hostname}{port::#}";

    public static Url Http
    => SchemeUrl(Scheme.Http);

    public static Url Https
    => SchemeUrl(Scheme.Https);

    public Url Combine(string l, string r, char sep)
    => $"{l.TrimEnd(sep)}{sep}{r.TrimStart(sep)}";

    Url Combine(string str, char sep)
    => Combine(Value, str, sep);

    public Url AppendPath(Url url)
    => Combine(url, PathSeparator);

    public Url AppendQuery(string kv)
    => Combine(kv, Value.Contains(QueryPrefix) ? QueryArgSeparator : QueryPrefix);

    public Url AppendQuery(IEnumerable<string> kv)
    => Combine(String.Join(QueryArgSeparator.ToString(), kv), Value.Contains(QueryPrefix) ? QueryArgSeparator : QueryPrefix);

    public static string FormatArg(string k, object v)
    => $"{k}{QueryKeyValueSeparator}{Encode(v.ToString())}";

    public static string FormatArg(PropertyInfo prop, object obj)
    => FormatArg(prop.Name, prop.GetValue(obj));

    public Url AppendQuery(string k, object v)
    => AppendQuery(FormatArg(k, v));

    public Url AppendQuery(IEnumerable<string> keys, IEnumerable<object> values)
    => AppendQuery(keys.Zip(values, FormatArg));

    public Url AppendQuery(object obj)
    => AppendQuery(obj.GetType().GetProperties().Select(p => FormatArg(p.Name, p.GetValue(obj))));
}
#endregion
#endregion