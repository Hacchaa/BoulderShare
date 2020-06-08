using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Foundation.Patterns
{
    public static class SA_SingletonService
    {
        static Transform s_services = null;

        public static Transform Parent
        {
            get
            {
                if (s_services == null)
                {
                    s_services = new GameObject("SA_Singletons").transform;
                    Object.DontDestroyOnLoad(s_services);
                }

                return s_services;
            }
        }
    }
}
