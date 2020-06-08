using System;
using System.Collections.Generic;
using SA.iOS.GameKit;
using SA.iOS.Foundation;
using SA.Foundation.Templates;

namespace SA.CrossPlatform.GameServices
{
    class UM_IOSLeaderboardsClient : UM_AbstractLeaderboardsClient, UM_iLeaderboardsClient
    {
        public void ShowUI(Action<SA_Result> callback)
        {
            var viewController = new ISN_GKGameCenterViewController();
            viewController.ViewState = ISN_GKGameCenterViewControllerState.Leaderboards;
            viewController.Show(() =>
            {
                callback.Invoke(new SA_Result());
            });
        }

        public void ShowUI(string leaderboardId, Action<SA_Result> callback)
        {
            ShowUI(leaderboardId, UM_LeaderboardTimeSpan.AllTime, callback);
        }

        public void ShowUI(string leaderboardId, UM_LeaderboardTimeSpan timeSpan, Action<SA_Result> callback)
        {
            var viewController = new ISN_GKGameCenterViewController();
            viewController.ViewState = ISN_GKGameCenterViewControllerState.Leaderboards;
            viewController.LeaderboardIdentifier = leaderboardId;
            var scope = ToIOSSpan(timeSpan);
            viewController.LeaderboardTimeScope = scope;
            viewController.Show(() =>
            {
                callback.Invoke(new SA_Result());
            });
        }

        public void SubmitScore(string leaderboardId, long score, int context, Action<SA_Result> callback)
        {
            var scoreReporter = new ISN_GKScore(leaderboardId);
            scoreReporter.Value = score;
            scoreReporter.Context = context;
            scoreReporter.Report(result =>
            {
                ReportScoreSubmited(leaderboardId, score, result);
                callback.Invoke(result);
            });
        }

        public void LoadLeaderboardsMetadata(Action<UM_LoadLeaderboardsMetaResult> callback)
        {
            ISN_GKLeaderboard.LoadLeaderboards(result =>
            {
                UM_LoadLeaderboardsMetaResult um_result;
                if (result.IsSucceeded)
                {
                    var um_leaderboards = new List<UM_iLeaderboard>();
                    foreach (var leaderboards in result.Leaderboards)
                    {
                        var um_leaderboardMeta = new UM_LeaderboardMeta(leaderboards.Identifier, leaderboards.Title);
                        um_leaderboards.Add(um_leaderboardMeta);
                    }

                    um_result = new UM_LoadLeaderboardsMetaResult(um_leaderboards);
                }
                else
                {
                    um_result = new UM_LoadLeaderboardsMetaResult(result.Error);
                }

                callback.Invoke(um_result);
            });
        }

        ISN_GKLeaderboardPlayerScope ToIOSCollection(UM_LeaderboardCollection collection)
        {
            var isn_collection = ISN_GKLeaderboardPlayerScope.Global;
            switch (collection)
            {
                case UM_LeaderboardCollection.Public:
                    isn_collection = ISN_GKLeaderboardPlayerScope.Global;
                    break;
                case UM_LeaderboardCollection.Social:
                    isn_collection = ISN_GKLeaderboardPlayerScope.FriendsOnly;
                    break;
            }

            return isn_collection;
        }

        ISN_GKLeaderboardTimeScope ToIOSSpan(UM_LeaderboardTimeSpan span)
        {
            var isn_timeSpan = ISN_GKLeaderboardTimeScope.AllTime;
            switch (span)
            {
                case UM_LeaderboardTimeSpan.AllTime:
                    isn_timeSpan = ISN_GKLeaderboardTimeScope.AllTime;
                    break;
                case UM_LeaderboardTimeSpan.Weekly:
                    isn_timeSpan = ISN_GKLeaderboardTimeScope.Week;
                    break;
                case UM_LeaderboardTimeSpan.Daily:
                    isn_timeSpan = ISN_GKLeaderboardTimeScope.Today;
                    break;
            }

            return isn_timeSpan;
        }

        public void LoadCurrentPlayerScore(string leaderboardId, UM_LeaderboardTimeSpan span, UM_LeaderboardCollection collection, Action<UM_ScoreLoadResult> callback)
        {
            var leaderboardRequest = new ISN_GKLeaderboard();
            leaderboardRequest.Identifier = leaderboardId;
            var isn_timeSpan = ToIOSSpan(span);
            var isn_collection = ToIOSCollection(collection);
            leaderboardRequest.PlayerScope = isn_collection;
            leaderboardRequest.TimeScope = isn_timeSpan;
            leaderboardRequest.Range = new ISN_NSRange(1, 1);
            leaderboardRequest.LoadScores(result =>
            {
                UM_ScoreLoadResult um_result;
                if (result.IsSucceeded)
                {
                    var score = new UM_Score(leaderboardRequest.LocalPlayerScore.Value,
                        leaderboardRequest.LocalPlayerScore.Rank,
                        (int)leaderboardRequest.LocalPlayerScore.Context,
                        leaderboardRequest.LocalPlayerScore.DateUnix
                    );
                    um_result = new UM_ScoreLoadResult(score);
                }
                else
                {
                    um_result = new UM_ScoreLoadResult(result.Error);
                }

                callback.Invoke(um_result);
            });
        }
    }
}
