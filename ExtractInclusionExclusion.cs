///////////////////////////////////////////////////////////
// Plugin PfizerClinicalTrials : file ExtractInclusionExclusion.cs
//

using System;
using System.Collections.Generic;
using System.Text;
using Sinequa.Common;
using Sinequa.Configuration;
using Sinequa.Plugins;
using Sinequa.Connectors;
using Sinequa.Indexer;
using Sinequa.Search;
//using Sinequa.Ml;

namespace Sinequa.Plugin
{
	
	public class ExtractInclusionExclusion : FunctionPlugin
	{

        public override string GetValue(IDocContext ctxt, params string[] values)
        {
            if (values.Length < 2) return Str.Empty;
            string txt = values[0];
            if (Str.IsEmpty(txt)) return Str.Empty;
            string type = values[1];
            if (Str.IsEmpty(type) || !Str.EQNCN(type, new string[] { "inclusion", "exclusion", "all" })) return Str.Empty;

            string[] arr = txt.Split(new Char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            bool isIC = false;
            bool isEC = false;

            ListStr lIC = new ListStr();
            ListStr lEC = new ListStr();

            foreach (string line in arr)
            {
                string cLine = Str.Trim(line);
                cLine = Str.Replace(cLine, ';', ',');

                if (Str.BeginWith(cLine, "inclusion criteria", true))
                {
                    isIC = true; isEC = false; continue;
                }
                if (Str.BeginWith(cLine, "exclusion criteria", true))
                {
                    isIC = false; isEC = true; continue;
                }

                if (isIC)
                {
                    if (Str.BeginWith(cLine, "- "))
                    {
                        lIC.Add(Str.BeginWithReplace(cLine, "- ", ""));
                    }
                    else
                    {
                        lIC.Set(lIC.Count - 1, lIC.Get(lIC.Count - 1) + " " + cLine);
                    }

                }

                if (isEC)
                {
                    if (Str.BeginWith(cLine, "- "))
                    {
                        lEC.Add(Str.BeginWithReplace(cLine, "- ", ""));
                    }
                    else
                    {
                        lEC.Set(lIC.Count - 1, lEC.Get(lIC.Count - 1) + " " + cLine);
                    }
                }
            }

            if (Str.EQNC(type, "inclusion")) return lIC.ToStr(';');
            if (Str.EQNC(type, "exclusion")) return lEC.ToStr(';');
            if (Str.EQNC(type, "all"))
            {
                ListStr l = new ListStr();
                l.Add(lIC);
                l.Add(lEC);
                return l.ToStr(';');
            }
            return Str.Empty;
        }

    }
	
}
