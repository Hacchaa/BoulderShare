using System;
using UnityEngine;

namespace SA.CrossPlatform.App
{
    [Serializable]
    abstract class UM_AbstractContact
    {
        [SerializeField]
        protected string m_name;
        [SerializeField]
        protected string m_phone;
        [SerializeField]
        protected string m_email;

        public string Name => m_name;

        public string Phone => m_phone;

        public string Email => m_email;
    }
}
