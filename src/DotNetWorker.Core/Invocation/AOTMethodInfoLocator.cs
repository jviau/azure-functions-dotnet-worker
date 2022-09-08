using System;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
#if NET5_0_OR_GREATER
#endif
using Microsoft.Azure.Functions.Worker.Http;

namespace Microsoft.Azure.Functions.Worker.Invocation
{

    internal class AOTMethodInfoLocator : IMethodInfoLocator
    {


        private delegate HttpResponseData HelloDelegate(HttpRequestData msg);
        public MethodInfo GetMethod(string assemblyName, string entryPoint)
        {
           
            //Func<HttpRequestData, HttpResponseData> func = new Func<HttpRequestData, HttpResponseData>((HttpRequestData msg) =>
            //{
               
            //    var res= msg?.CreateResponse();
            //    res?.WriteString("Hello from AOT");
            //    return res;
            //});

            //var mi  = func.GetMethodInfo();
            //return mi;

            // https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.dynamicmethod?view=net-6.0
            Type[] helloArgs = { typeof(HttpRequestData) };

            DynamicMethod method = new DynamicMethod("Run",
                typeof(HttpResponseData),
                helloArgs,
                typeof(string).Module);

            Type[] createResponseArgs = {  };
            MethodInfo createResponse = typeof(HttpRequestData).GetMethod("CreateResponse", createResponseArgs);

            ILGenerator il = method.GetILGenerator(256);
            // Load the first argument, which is a HttpRequestData instance, onto the stack.
            il.Emit(OpCodes.Ldarg_0);
            // Call CreateResponse
            il.EmitCall(OpCodes.Call, createResponse, null);


            // return httpresponsedata
            il.Emit(OpCodes.Ret);

            //HelloDelegate hi =
            //(HelloDelegate)method.CreateDelegate(typeof(HelloDelegate));

            return method;
        }
    }
}
