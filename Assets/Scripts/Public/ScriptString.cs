using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace String
{

    public class ScriptString : MonoBehaviour
    {
        static public string RemoveSlash(string str)
        {
            string s = str;
            //if (s.IndexOf("//") >= 0)
            //{
            //    s = s.Substring(0, s.IndexOf("//"));
            //}
            return s;
        }

        static public string RemoveSpace(string str)
        {
            string s = str;
            s = s.Replace(" ", "");
            return s;
        }

        static public string RemoveTab(string str)
        {
            string s = str;
            s = s.Replace("\t", "");
            return s;
        }

        static public string RemoveEnter(string str)
        {
            string s = str;
            s = s.Replace("\n\n", "\n");
            s = s.Replace("\n\n", "\n");
            s = s.Replace("\n\n", "\n");
            return s;
        }

        static public string SetShapeToBR(string str)
        {
            string s = str;
            s = s.Replace("#", "\n");
            s = s.Replace("#", "\n");
            return s;
        }

    }
}