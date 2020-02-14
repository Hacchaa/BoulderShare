using System;
using SA.Foundation.Templates;
using SA.Foundation.Events;
using SA.CrossPlatform.Analytics;

namespace SA.CrossPlatform.GameServices
{
    internal abstract class UM_AbstractSignInClient 
    {
        private UM_PlayerInfo m_CurrentPlayerInfo = new UM_PlayerInfo(UM_PlayerState.SignedOut, null);
        private SA_Event m_OnPlayerChanged = new SA_Event();
        private SA_Event<SA_Result> m_SingInCallback = new SA_Event<SA_Result>();
        private bool m_SingInFlowInProgress;
        
        //--------------------------------------
        // Abstract Methods
        //--------------------------------------

        protected abstract void StartSingInFlow(Action<SA_Result> callback);
        
        //--------------------------------------
        // Public Methods
        //--------------------------------------

        public void SingIn(Action<SA_Result> callback) 
        {
            m_SingInCallback.AddListener(callback);

            //Preventing double sing in
            if (m_SingInFlowInProgress) { return;}

            m_SingInFlowInProgress = true;
            StartSingInFlow(result => 
            {
                m_SingInFlowInProgress = false;
                m_SingInCallback.Invoke(result);

                m_SingInCallback.RemoveAllListeners();
            });
        }

        //--------------------------------------
        // Get / Set
        //--------------------------------------

        public SA_iEvent OnPlayerUpdated 
        {
            get { return m_OnPlayerChanged; }
        }
        
        public UM_PlayerInfo PlayerInfo 
        {
            get { return m_CurrentPlayerInfo; }
        }

        public bool IsSingInFlowInProgress 
        {
            get { return m_SingInFlowInProgress; }
        }

        //--------------------------------------
        // Protected Methods 
        //--------------------------------------

        protected void UpdateSignedPlayer(UM_PlayerInfo info) 
        {
            m_CurrentPlayerInfo = info;
            m_OnPlayerChanged.Invoke();

            UM_AnalyticsInternal.OnPlayerUpdated(info);
        }
    }
}