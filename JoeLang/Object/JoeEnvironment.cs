namespace JoeLang.Object;

public class JoeEnvironment
{
    private Dictionary<string, IJoeObject> store;
    private JoeEnvironment? outer;

    public JoeEnvironment()
    {
        store = new Dictionary<string, IJoeObject>();
    }

    public JoeEnvironment(JoeEnvironment environment)
    {
        store = new Dictionary<string, IJoeObject>();
        outer = environment;
    }

    public JoeEnvironment? Outer { get =>  outer; }

    public IJoeObject? Get(string key) 
    {
        IJoeObject? result;
        if (store.TryGetValue(key,out result))
            return result;
        if (outer != null)
            return outer.Get(key);
        return null;
    }

    public void Set(string name, IJoeObject value) 
    {
        store[name] = value;
    }
}
