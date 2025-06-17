namespace JoeLang.Object;

public class JoeEnvironment
{
    private Dictionary<string, IJoeObject> store;

    public JoeEnvironment()
    {
        store = new Dictionary<string, IJoeObject>();
    }

    public JoeEnvironment(JoeEnvironment environment)
    {
        store = new Dictionary<string, IJoeObject>();
        Outer = environment;
    }

    public JoeEnvironment? Outer { get; }

    public IJoeObject? Get(string key) 
    {
        if (store.TryGetValue(key,out var result))
            return result;
        return Outer is not null  ? Outer!.Get(key) : null;
    }

    public void Set(string name, IJoeObject value) 
    {
        store[name] = value;
    }
}
