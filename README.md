# ProcessPipeline
ProcessPipeline is an easy to use assembly line framework. It allows you to build complex workflows with reusable assembly parts.

## Example

An example using custom pipe classes and inline pipe declaration to build a console logger.
```csharp
public class ConsoleLogger
{
    private readonly ProcessPipe<Object, String> _logPipe;

    public ConsoleLogger(LogTypes type) : base(type)
    {
        _logPipe = new ConditionalPipe<Object, Object>(o => null == o // validate 1
                , new EmptyPipe<Object>(() => throw new ProcessAbortedException())
                , new EmptyPipe<Object>()).Connect(new ConditionalPipe<Object, String>(
                o => o is Exception // object -> string
                , new CasterPipe<Object, Exception>().Connect(new ExceptionStringifierPipe())
                , new ProcessPipe<Object, String>(o => o?.ToString())).Connect(
                new ProcessPipe<String, String>(s => s?.Trim())))
            .Connect(new ConditionalPipe<String, String>(String.IsNullOrWhiteSpace // validate 2
                , new EmptyPipe<String>(() => throw new ProcessAbortedException())
                , new EmptyPipe<String>()))
            .Connect(new LogFlagPipe(type, DefaultLogFlags)) // process
            .Connect(new ConditionalPipe<String, String>(o => LogTypes.Error == type || LogTypes.Fatal == type // write
                , new WriterPipe(Console.Error)
                , new WriterPipe(Console.Out)));
    }

    public void Log(Object o)
    {
        _logPipe.Process(o);
    }
}
```
### The custom pipe classes

##### ExceptionStringifierPipe
```csharp
public class ExceptionStringifierPipe : ProcessPipe<Exception, String>
{
    public ExceptionStringifierPipe() : base(exception =>
    {
        if (null == exception)
        {
            return null;
        }

        StringBuilder sb = new StringBuilder();

        new List<(String Title, String Value)>
            {
                ("Type      ", exception.GetType().Name)
                , ("Message   ", exception.Message)
                , ("Source    ", exception.Source)
                , ("HelpLink  ", exception.HelpLink)
                , ("HResult   ", exception.HResult.ToString())
                , ("TargetSite", exception.TargetSite?.Name)
                , ("Stacktrace", $"{Environment.NewLine}{exception.StackTrace}")
            }.Where(x => !String.IsNullOrWhiteSpace(x.Value))
            .Select(x => $"{x.Title} : {x.Value.Trim().Replace(Environment.NewLine, "")}{Environment.NewLine}")
            .ToList()
            .ForEach(x => sb.Append(x));

        if (exception.Data?.Count > 0)
        {
            sb.Append($"Data :{Environment.NewLine}");
            foreach (Object key in exception.Data.Keys)
            {
                sb.Append($" -> [{key}, {exception.Data[key]}]{Environment.NewLine}");
            }
        }

        if (null == exception.InnerException)
        {
            return sb.ToString();
        }

        String innerEx = new ExceptionStringifierPipe().Process(exception.InnerException);

        sb.Append($"----- INNER EXCEPTION -----{Environment.NewLine}");
        sb.Append(innerEx);

        return sb.ToString();
    }) { }
}
```

##### WriterPipe

```csharp
public class WriterPipe : ProcessPipe<String, String>, IDisposable
{
    private readonly TextWriter _stream;

    public WriterPipe(TextWriter stream) : base(o =>
    {
        stream?.Write(o ?? String.Empty);
        return o;
    })
    {
        _stream = stream;
    }

    public void Dispose()
    {
        try
        {
            _stream?.Dispose();
        }
        catch (Exception)
        {
            // ignore
        }
    }

    ~WriterPipe()
    {
        Dispose();
    }
}
```


##### LogFlagPipe

```csharp
public LogFlagPipe(LogTypes type, LogFlags flags) : base(o =>
{
    ProcessPipe<String, String> pipe = new EmptyPipe<String>();
    if (flags.HasFlag(LogFlags.PrefixLoggerType))
    {
        pipe = pipe.Connect(new ProcessPipe<String, String>(s => $"[{type}] {s}"));
    }
    if (flags.HasFlag(LogFlags.PrefixTimeStamp))
    {
        pipe = pipe.Connect(new ProcessPipe<String, String>(s => $"[{DateTime.Now:s}] {s}"));
    }
    if (flags.HasFlag(LogFlags.SuffixNewLine))
    {
        pipe = pipe.Connect(new ProcessPipe<String, String>(s => $"{s}{Environment.NewLine}"));
    }
    return pipe.Process(o);
}) { }

public enum LogTypes
{
    Trace
    , Debug
    , Info
    , Warning
    , Error
    , Fatal
}
```

.. you get the idea.

Of course you can decide for yourself what abstraction level your pipe processes should have. 
