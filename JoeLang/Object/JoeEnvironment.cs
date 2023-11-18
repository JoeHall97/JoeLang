using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        return store.GetValueOrDefault(key);
    }

    public void Set(string name, IJoeObject value) 
    {
        store[name] = value;
    }
}
