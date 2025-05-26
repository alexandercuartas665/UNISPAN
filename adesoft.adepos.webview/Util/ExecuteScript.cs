//using Microsoft.CSharp;
//using System;
//using System.CodeDom.Compiler;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Threading.Tasks;

//namespace adesoft.adepos.webview.Util
//{
//    public class ExecuteScript
//    {
//        public static double EvaluateExpression(string expression)
//        {
//            string code = string.Format  // Note: Use "{{" to denote a single "{"  
//            (
//                "public static class Func{{ public static double func(){{ return {0};}}}}",
//                expression
//            );

//            CompilerResults compilerResults = CompileScript(code);

//            if (compilerResults.Errors.HasErrors)
//            {
//                throw new InvalidOperationException("Expression has a syntax error.");
//            }

//            Assembly assembly = compilerResults.CompiledAssembly;
//            MethodInfo method = assembly.GetType("Func").GetMethod("func");

//            return (double)method.Invoke(null, null);
//        }
//        public static CompilerResults CompileScript(string source)
//        {
//            CompilerParameters parms = new CompilerParameters();

//            parms.GenerateExecutable = false;
//            parms.GenerateInMemory = true;
//            parms.IncludeDebugInformation = false;

//            CodeDomProvider compiler = CSharpCodeProvider.CreateProvider("CSharp");

//            return compiler.CompileAssemblyFromSource(parms, source);
//        }
//    }
//}
