// This file is part of Smuxi and is licensed under the terms of MIT/X11
//
// Copyright (c) 2005-2006 Mirco Bauer <meebey@meebey.net>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Text;
using System.Runtime.Remoting;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using SysTrace = System.Diagnostics.Trace;

namespace Smuxi.Common
{
    public sealed class Trace
    {
#if LOG4NET
        private static readonly log4net.ILog _Logger = log4net.LogManager.GetLogger("TRACE");
#else
        static Trace()
        {
            TextWriterTraceListener myWriter = new TextWriterTraceListener(Console.Out);
            SysTrace.Listeners.Add(myWriter);
        }
#endif

        public static MethodBase GetMethodBase()
        {
            return new StackTrace(new StackFrame(1)).GetFrame(0).GetMethod();
        }

        public static string GetStackTrace()
        {
            string line = null;
            StackTrace st = new StackTrace();
            for (int i = st.FrameCount - 1; i >= 1 ; i--) {
                StackFrame sf = new StackFrame();
                sf = st.GetFrame(i);
                MethodBase method = sf.GetMethod();
                string methodname = method.DeclaringType + "." + method.Name;
                line += methodname + "()" + Environment.NewLine;
            }

            return line;
        }

        [Conditional("TRACE")]
        public static void CallFull(params object[] args)
        {
            MethodBase mb = new StackTrace(new StackFrame(1)).GetFrame(0).GetMethod();
            string methodname = mb.DeclaringType.Name + "." + mb.Name;
            string line = GetStackTrace();
            line += methodname + "(" + _Parameterize(mb, args) + ")";
#if LOG4NET
            _Logger.Debug(line);
#else
            SysTrace.Write(line);
#endif
        }
        
        [Conditional("TRACE")]
        public static void Call(params object[] args)
        {
            MethodBase mb = new StackTrace(new StackFrame(1)).GetFrame(0).GetMethod();
            Call(mb, args);
        }

        [Conditional("TRACE")]
        public static void Call(MethodBase mb, params object[] args)
        {
            if (mb == null) {
                throw new ArgumentNullException("mb");
            }

            StringBuilder line = new StringBuilder();
            line.Append("[");
            line.Append(System.IO.Path.GetFileName(mb.DeclaringType.Assembly.Location));
            line.Append("] ");
            line.Append(mb.DeclaringType.Name);
            line.Append(".");
            line.Append(mb.Name);
            line.Append("(");
            line.Append(_Parameterize(mb, args));
            line.Append(")");
            
#if LOG4NET
            _Logger.Debug(line.ToString());
#else
            SysTrace.WriteLine(line.ToString());
#endif
        }
        
        private static string _Parameterize(MethodBase method, params object[] parameters)
        {
            ParameterInfo[] parameter_info = method.GetParameters();
            if (parameter_info.Length == 0) {
                return String.Empty;
            }
            StringBuilder res = new StringBuilder();
            for (int i = 0; i < parameter_info.Length; i++) {
                res.Append(parameter_info[i].Name).Append(" = ");
                if (parameters == null) {
                    res.Append(_ParameterizeQuote(null));
                } else if (parameters != null && parameters.Length > i) {
                    res.Append(_ParameterizeQuote(parameters[i]));
                } else {
                    // empty array
                    res.Append("[]");
                }
                res.Append(", ");
            }
            res.Remove(res.Length - 2, 2);
            return res.ToString();
        }

        private static string _ParameterizeQuote(object obj)
        {
            if (obj == null) {
                return "(null)";
            }

            // OPT: tracing over remote objects is too expensive!
            if (RemotingServices.IsTransparentProxy(obj)) {
                return obj.GetType().ToString();
            }

            StringBuilder line = new StringBuilder();
            if (obj is string) {
                line.Append("'").Append(obj).Append("'");
            } else if (obj is ITraceable) {
                line.AppendFormat("<{0}>", ((ITraceable) obj).ToTraceString());
            } else if (obj is IList) {
                line.Append("[");
                foreach (object val in (IList) obj) {
                    if (val is IList || val is IDictionary) {
                        line.Append(_ParameterizeQuote(val));
                        line.Append(", ");
                        continue;
                    }
                    line.Append((val == null ? "(null)" : val.ToString()));
                    line.Append(", ");
                }
                // remove last ", "
                if (line.Length > 1) {
                    line.Remove(line.Length - 2, 2);
                }
                line.Append("]");
            } else if (obj is IDictionary) {
                line.Append("{");
                foreach (DictionaryEntry de in (IDictionary) obj) {
                    if (de.Value is IList || de.Value is IDictionary) {
                        line.Append(de.Key.ToString());
                        line.Append("=");
                        line.Append(_ParameterizeQuote(de.Value));
                        line.Append(", ");
                        continue;
                    }
                    line.Append(de.Key.ToString());
                    line.Append("=");
                    line.Append((de.Value == null ? "(null)" : de.Value.ToString()));
                    line.Append(", ");
                }
                if (line.Length > 1) {
                    line.Remove(line.Length - 2, 2);
                }
                line.Append("}");
            } else {
                line.Append(obj.ToString());
            }

            return line.ToString();
        }

        /*
        private static string Dump(Hashtable ht)
        {
            string line = null;
            line += "{";
            foreach (DictionaryEntry de in (Hashtable)obj) {
                line += de.Key.ToString();
                line += "=";
                line += (de.Value == null ? "(null)" : de.Value.ToString());
                line += ", ";
            }
            line = line.Substring(0, line.Length - 2);
            line += "}";
            return line;
        }
        */
    }
}
