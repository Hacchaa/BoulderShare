using System;
using SA.Foundation.Patterns;

namespace SA.Foundation.Events
{
    public static class SA_MonoEvents
    {
        class SA_MonoEventsListner : SA_Singleton<SA_MonoEventsListner>
        {
            public SA_Event m_onApplicationQuit = new SA_Event();
            public SA_Event<bool> m_onApplicationFocus = new SA_Event<bool>();
            public SA_Event<bool> m_onApplicationPause = new SA_Event<bool>();

            public SA_Event m_onUpdate = new SA_Event();

            protected override void OnApplicationQuit()
            {
                base.OnApplicationQuit();
                m_onApplicationQuit.Invoke();
            }

            void OnApplicationFocus(bool focus)
            {
                m_onApplicationFocus.Invoke(focus);
            }

            void OnApplicationPause(bool pause)
            {
                m_onApplicationPause.Invoke(pause);
            }

            void Update()
            {
                m_onUpdate.Invoke();
            }
        }

        public static SA_iEvent OnApplicationQuit => SA_MonoEventsListner.Instance.m_onApplicationQuit;

        public static SA_iEvent<bool> OnApplicationFocus => SA_MonoEventsListner.Instance.m_onApplicationFocus;

        public static SA_iEvent<bool> OnApplicationPause => SA_MonoEventsListner.Instance.m_onApplicationPause;

        public static SA_iEvent OnUpdate => SA_MonoEventsListner.Instance.m_onUpdate;
    }
}
