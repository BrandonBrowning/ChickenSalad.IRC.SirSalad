using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChickenSalad.IRC.Common;
using Lokad.Threading;
using Newtonsoft.Json;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace ChickenSalad.IRC.SirSalad
{
    public class RoslynExecutor
    {
        private static ScriptEngine ScriptEngine { get; set; }
        private static Assembly[] Assemblies { get; set; }
        private static string[] Namespaces { get; set; }

        static RoslynExecutor()
        {
            ScriptEngine = new ScriptEngine();

            Assemblies = new[]
            {
                typeof(Type).Assembly,
                typeof(ICollection).Assembly,
                typeof(ListDictionary).Assembly,
                typeof(Console).Assembly,
                typeof(ScriptEngine).Assembly,
                typeof(IEnumerable<>).Assembly,
                typeof(IQueryable).Assembly,
                typeof(IRCExtensions).Assembly,
            };

            Namespaces = new string[]
            {
                "System",
                "System.Collections",
                "System.Collections.Generic",
                "System.Linq",
                "ChickenSalad.IRC.Common",
            };

            foreach (var assembly in Assemblies)
            {
                ScriptEngine.AddReference(assembly);
            }
        }

        public RoslynExecutor()
        {

        }

        public Session SpawnSession()
        {
            var session = ScriptEngine.CreateSession();

            foreach (string @namespace in Namespaces)
            {
                session.ImportNamespace(@namespace);
            }

            return session;
        }

        public string PerformExecuteCode(string code)
        {
            try
            {
                var session = SpawnSession();
                var result = session.Execute(code);

                return result == null ? "" : JsonConvert.SerializeObject(result);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string ExecuteCode(string code)
        {
            try
            {
                string result = WaitFor<string>.Run(TimeSpan.FromSeconds(5), () => PerformExecuteCode(code));

                return result;
            }
            catch (TimeoutException e)
            {
                return "Exception: Time limit exceeded.";
            }
        }
    }
}
