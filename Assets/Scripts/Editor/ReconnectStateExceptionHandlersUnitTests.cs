using System;
using PubNubMessaging.Core;
using NUnit.Framework;
using System.Text;

namespace PubNubMessaging.Tests
{
	[TestFixture]
	public class ReconnectStateExceptionHandlersUnitTests
	{
		#if DEBUG
		string ExceptionMessage ="";
		string ExceptionChannel = "";
		int ExceptionStatusCode = 0;

		ResponseType CRequestType;
		string[] Channels;
		bool ResumeOnReconnect;

		void UserCallback (string result)
		{
			UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result));
		}

		void UserCallback (object result)
		{
			UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result.ToString()));
		}

		void ErrorCallback (PubnubClientError result)
		{
			UnityEngine.Debug.Log (string.Format ("DisplayErrorMessage LOG: {0}", result));
		}

		void DisconnectCallback (string result)
		{
			UnityEngine.Debug.Log (string.Format ("Disconnect CALLBACK LOG: {0}", result));
		}

		void ConnectCallback (string result)
		{
			UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result));
		}

		void ConnectCallback (object result)
		{
			UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result.ToString()));
		}

		[Test]
		public void TestBuildRequestStateHistoryObj ()
		{
			string[] channels = new string[] { "test" };
			TestBuildRequestStateCommon<object> (channels, CurrentRequestType.NonSubscribe , ResponseType.DetailedHistory, 
				false, UserCallback, ConnectCallback, ErrorCallback, DateTime.Now.Ticks, false, 0, null
			);
		}

		[Test]
		public void TestBuildRequestStateHereNowObj ()
		{
			string[] channels = new string[] { "test" };
			TestBuildRequestStateCommon<object> (channels, CurrentRequestType.NonSubscribe , ResponseType.HereNow, 
				false, UserCallback, ConnectCallback, ErrorCallback, DateTime.Now.Ticks, false, 0, null
			);
		}

		[Test]
		public void TestBuildRequestStateSubscribeObj ()
		{
			string[] channels = new string[] { "test" };
			TestBuildRequestStateCommon<object> (channels, CurrentRequestType.Subscribe, ResponseType.Subscribe, 
				false, UserCallback, ConnectCallback, ErrorCallback, DateTime.Now.Ticks, false, 0, null
			);
		}

		[Test]
		public void TestBuildRequestStateSubscribeReconnectObj ()
		{
			string[] channels = new string[] { "test" };
			TestBuildRequestStateCommon<object> (channels, CurrentRequestType.Subscribe, ResponseType.Subscribe, 
				true, UserCallback, ConnectCallback, ErrorCallback, DateTime.Now.Ticks, false, 0, null
			);
		}

		[Test]
		public void TestBuildRequestStateSubscribeTimeoutObj ()
		{
			string[] channels = new string[] { "test" };
			TestBuildRequestStateCommon<object> (channels, CurrentRequestType.Subscribe, ResponseType.Subscribe, 
				true, UserCallback, ConnectCallback, ErrorCallback, DateTime.Now.Ticks, false, 0, null
			);
		}

		[Test]
		public void TestBuildRequestStateSubscribeTimetokenObj ()
		{
			string[] channels = new string[] { "test" };
			TestBuildRequestStateCommon<object> (channels, CurrentRequestType.Subscribe, ResponseType.Subscribe, 
				true, UserCallback, ConnectCallback, ErrorCallback, DateTime.Now.Ticks, false, 14498416434364941, null
			);
		}

		[Test]
		public void TestBuildRequestStateHistory ()
		{
			string[] channels = new string[] { "test" };
			TestBuildRequestStateCommon<string> (channels, CurrentRequestType.NonSubscribe , ResponseType.DetailedHistory, 
				false, UserCallback, ConnectCallback, ErrorCallback, DateTime.Now.Ticks, false, 0, null
			);
		}

		[Test]
		public void TestBuildRequestStateHereNow ()
		{
			string[] channels = new string[] { "test" };
			TestBuildRequestStateCommon<string> (channels, CurrentRequestType.NonSubscribe , ResponseType.HereNow, 
				false, UserCallback, ConnectCallback, ErrorCallback, DateTime.Now.Ticks, false, 0, null
			);
		}

		[Test]
		public void TestBuildRequestStateSubscribe ()
		{
			string[] channels = new string[] { "test" };
			TestBuildRequestStateCommon<string> (channels, CurrentRequestType.Subscribe, ResponseType.Subscribe, 
				false, UserCallback, ConnectCallback, ErrorCallback, DateTime.Now.Ticks, false, 0, null
			);
		}

		[Test]
		public void TestBuildRequestStateSubscribeReconnect ()
		{
			string[] channels = new string[] { "test" };
			TestBuildRequestStateCommon<string> (channels, CurrentRequestType.Subscribe, ResponseType.Subscribe, 
				true, UserCallback, ConnectCallback, ErrorCallback, DateTime.Now.Ticks, false, 0, null
			);
		}

		[Test]
		public void TestBuildRequestStateSubscribeTimeout ()
		{
			string[] channels = new string[] { "test" };
			TestBuildRequestStateCommon<string> (channels, CurrentRequestType.Subscribe, ResponseType.Subscribe, 
				true, UserCallback, ConnectCallback, ErrorCallback, DateTime.Now.Ticks, false, 0, null
			);
		}

		[Test]
		public void TestBuildRequestStateSubscribeTimetoken ()
		{
			string[] channels = new string[] { "test" };
			TestBuildRequestStateCommon<string> (channels, CurrentRequestType.Subscribe, ResponseType.Subscribe, 
				true, UserCallback, ConnectCallback, ErrorCallback, DateTime.Now.Ticks, false, 14498416434364941, null
			);
		}

		[Test]
		public void TestBuildRequestStateSubscribeTimetokenId ()
		{
			string[] channels = new string[] { "test" };
			TestBuildRequestStateCommon<string> (channels, CurrentRequestType.Subscribe, ResponseType.Subscribe, 
				true, UserCallback, ConnectCallback, ErrorCallback, 0, false, 14498416434364941, null
			);
		}

		[Test]
		public void TestBuildRequestStateSubscribeTimetokenObjId ()
		{
			string[] channels = new string[] { "test" };
			TestBuildRequestStateCommon<object> (channels, CurrentRequestType.Subscribe, ResponseType.Subscribe, 
				true, UserCallback, ConnectCallback, ErrorCallback, 0, false, 14498416434364941, null
			);
		}

		public void TestBuildRequestStateCommon<T>(string[] channels, CurrentRequestType requestType, ResponseType responseType, 
			bool reconnect, Action<T> userCallback,
			Action<T> connectCallback, Action<PubnubClientError> errorCallback,
			long id, bool timeout, long timetoken, Type typeParam
		){
			RequestState<T> requestState = BuildRequests.BuildRequestState<T> (channels, responseType, 
				reconnect, userCallback, connectCallback, errorCallback, id, timeout, timetoken, typeof(T));

			StoredRequestState.Instance.SetRequestState (requestType, requestState);
			RequestState<T> reqState = StoredRequestState.Instance.GetStoredRequestState(requestType) as RequestState<T>;
			Assert.IsTrue (reqState.Equals (requestState) && (reqState.ID.Equals(id)));
		}	

		[Test]
		public void TestCommonExceptionHandlerTimeoutSubscribe ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.Subscribe, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerNonTimeoutSubscribe ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.Subscribe, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerDetailedHistory ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.DetailedHistoryOperationTimeout;
			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.DetailedHistory, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerNonTimeoutDetailedHistory ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.DetailedHistory, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerHereNow ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.HereNowOperationTimeout;
			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.HereNow, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerNonTimeoutHereNow ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.HereNow, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerGlobalHereNow ()
		{
			ExceptionStatusCode = (int)PubnubErrorCode.GlobalHereNowOperationTimeout;
			TestCommonExceptionHandlerCommon<object> ("test message", "",
				ResponseType.GlobalHereNow, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerNonTimeoutGlobalHereNow ()
		{

			TestCommonExceptionHandlerCommon<object> ("test message", "",
				ResponseType.GlobalHereNow, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerWhereNow ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.WhereNowOperationTimeout;
			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.WhereNow, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerNonTimeoutWhereNow ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.WhereNow, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerAuditAccess ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.PAMAccessOperationTimeout;
			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.AuditAccess, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerNonTimeoutAuditAccess ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.AuditAccess, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerGrantAccess ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.PAMAccessOperationTimeout;
			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.GrantAccess, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerNonTimeoutGrantAccess ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.GrantAccess, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerRevokeAccess ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.PAMAccessOperationTimeout;
			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.RevokeAccess, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerNonTimeoutRevokeAccess ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.RevokeAccess, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerGetUserState ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.GetUserStateTimeout;
			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.GetUserState, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerNonTimeoutGetUserState ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.GetUserState, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerSetUserState ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.SetUserStateTimeout;
			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.SetUserState, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerNonTimeoutSetUserState ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.SetUserState, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerPublish ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.PublishOperationTimeout;
			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.Publish, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerNonTimeoutPublish ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.Publish, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerTime ()
		{
			ExceptionStatusCode = (int)PubnubErrorCode.TimeOperationTimeout;
			TestCommonExceptionHandlerCommon<object> ("test message", "",
				ResponseType.Time, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerNonTimeoutTime ()
		{
			TestCommonExceptionHandlerCommon<object> ("test message", "",
				ResponseType.Time, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerLeave ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.Leave, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerNonTimeoutLeave ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.Leave, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerUnsubscribe ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.Unsubscribe, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerNonTimeoutUnsubscribe ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.Unsubscribe, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}


		[Test]
		public void TestCommonExceptionHandlerPresence ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.Presence, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerNonTimeoutPresence ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.Presence, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerPresenceUnsubscribe ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.PresenceUnsubscribe, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerNonTimeoutPresenceUnsubscribe ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.PresenceUnsubscribe, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerHeartbeat ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.Heartbeat, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerNonTimeoutHeartbeat ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<object> ("test message", channel,
				ResponseType.Heartbeat, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringTimeoutSubscribe ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.Subscribe, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringNonTimeoutSubscribe ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.Subscribe, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringDetailedHistory ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.DetailedHistoryOperationTimeout;
			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.DetailedHistory, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringNonTimeoutDetailedHistory ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.DetailedHistory, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringHereNow ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.HereNowOperationTimeout;
			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.HereNow, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringNonTimeoutHereNow ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.HereNow, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringGlobalHereNow ()
		{
			ExceptionStatusCode = (int)PubnubErrorCode.GlobalHereNowOperationTimeout;
			TestCommonExceptionHandlerCommon<string> ("test message", "",
				ResponseType.GlobalHereNow, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringNonTimeoutGlobalHereNow ()
		{

			TestCommonExceptionHandlerCommon<string> ("test message", "",
				ResponseType.GlobalHereNow, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringWhereNow ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.WhereNowOperationTimeout;
			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.WhereNow, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringNonTimeoutWhereNow ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.WhereNow, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringAuditAccess ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.PAMAccessOperationTimeout;
			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.AuditAccess, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringNonTimeoutAuditAccess ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.AuditAccess, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringGrantAccess ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.PAMAccessOperationTimeout;
			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.GrantAccess, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringNonTimeoutGrantAccess ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.GrantAccess, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringRevokeAccess ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.PAMAccessOperationTimeout;
			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.RevokeAccess, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringNonTimeoutRevokeAccess ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.RevokeAccess, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringGetUserState ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.GetUserStateTimeout;
			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.GetUserState, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringNonTimeoutGetUserState ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.GetUserState, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringSetUserState ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.SetUserStateTimeout;
			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.SetUserState, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringNonTimeoutSetUserState ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.SetUserState, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringPublish ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.PublishOperationTimeout;
			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.Publish, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringNonTimeoutPublish ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.Publish, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringTime ()
		{
			ExceptionStatusCode = (int)PubnubErrorCode.TimeOperationTimeout;
			TestCommonExceptionHandlerCommon<string> ("test message", "",
				ResponseType.Time, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringNonTimeoutTime ()
		{
			TestCommonExceptionHandlerCommon<string> ("test message", "",
				ResponseType.Time, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringLeave ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.Leave, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringNonTimeoutLeave ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.Leave, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringUnsubscribe ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.Unsubscribe, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringNonTimeoutUnsubscribe ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.Unsubscribe, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}


		[Test]
		public void TestCommonExceptionHandlerStringPresence ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.Presence, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringNonTimeoutPresence ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.Presence, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringPresenceUnsubscribe ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.PresenceUnsubscribe, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringNonTimeoutPresenceUnsubscribe ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.PresenceUnsubscribe, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringHeartbeat ()
		{
			string channel = "test";
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.Heartbeat, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestCommonExceptionHandlerStringNonTimeoutHeartbeat ()
		{
			string channel = "test";

			TestCommonExceptionHandlerCommon<string> ("test message", channel,
				ResponseType.Heartbeat, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		public void TestCommonExceptionHandlerCommon<T>(string message, string channel,
			ResponseType responseType, Action<PubnubClientError> errorCallback,
			bool timeout, PubnubErrorFilter.Level errorLevel
		){
			ExceptionMessage = message;

			if (timeout) { 
				ExceptionMessage = "Operation Timeout";
			} else {
				ExceptionStatusCode = (int)PubnubErrorCode.None;
			}

			ExceptionChannel = channel;
			ExceptionHandlers.CommonExceptionHandler<T> (message, channel, timeout, errorCallback, errorLevel, responseType);
		}	

		void ErrorCallbackCommonExceptionHandler (PubnubClientError result)
		{
			UnityEngine.Debug.Log (string.Format ("DisplayErrorMessage LOG: {0} {1} {2} {3} {4} {5} {6}",
				result, result.Message.Equals (ExceptionMessage),
				result.Channel.Equals (ExceptionChannel),
				result.StatusCode.Equals(ExceptionStatusCode), ExceptionMessage, ExceptionChannel, ExceptionStatusCode
			));
			Assert.True (result.Message.Equals (ExceptionMessage) 
				&& result.Channel.Equals (ExceptionChannel)
				&& result.StatusCode.Equals(ExceptionStatusCode)
			);
		}

		void UserCallbackCommonExceptionHandler (string result)
		{
			UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result));
		}

		void UserCallbackCommonExceptionHandler (object result)
		{
			UnityEngine.Debug.Log (string.Format ("REGULAR CALLBACK LOG: {0}", result.ToString()));
		}

		void DisconnectCallbackCommonExceptionHandler (string result)
		{
			UnityEngine.Debug.Log (string.Format ("Disconnect CALLBACK LOG: {0}", result));
		}

		void ConnectCallbackCommonExceptionHandler (string result)
		{
			UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result));
		}

		void ConnectCallbackCommonExceptionHandler (object result)
		{
			UnityEngine.Debug.Log (string.Format ("CONNECT CALLBACK LOG: {0}", result.ToString()));
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerTimeoutSubscribeReconnect ()
		{
			string[] channel = {"test"}; 
			Channels = channel;
			ResumeOnReconnect = true;
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, true,
				ResponseType.Subscribe, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerNonTimeoutSubscribeReconnect ()
		{
			string[] channel = {"test"}; 
			Channels = channel;
			ResumeOnReconnect = true;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, true,
				ResponseType.Subscribe, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerTimeoutSubscribe ()
		{
			string[] channel = {"test"}; 
			Channels = channel;
			ResumeOnReconnect = false;
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.Subscribe, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerNonTimeoutSubscribe ()
		{
			string[] channel = {"test"}; 
			Channels = channel;
			ResumeOnReconnect = false;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.Subscribe, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerTimeoutPresenceReconnect ()
		{
			string[] channel = {"test"}; 
			Channels = channel;
			ResumeOnReconnect = true;
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, true,
				ResponseType.Presence, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerNonTimeoutPresenceReconnect ()
		{
			string[] channel = {"test"}; 
			Channels = channel;
			ResumeOnReconnect = true;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, true,
				ResponseType.Presence, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerTimeoutPresence ()
		{
			string[] channel = {"test"}; 
			Channels = channel;
			ResumeOnReconnect = false;
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.Presence, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerNonTimeoutPresence ()
		{
			string[] channel = {"test"}; 
			Channels = channel;
			ResumeOnReconnect = false;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.Presence, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerDetailedHistory ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.DetailedHistoryOperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.DetailedHistory, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerNonTimeoutDetailedHistory ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.DetailedHistory, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerHereNow ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.HereNowOperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.HereNow, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerNonTimeoutHereNow ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.HereNow, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerGlobalHereNow ()
		{
			string[] channel = {""}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.GlobalHereNowOperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.GlobalHereNow, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerNonTimeoutGlobalHereNow ()
		{
			string[] channel = {""}; ExceptionChannel = channel[0];
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.GlobalHereNow, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerWhereNow ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.WhereNowOperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.WhereNow, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerNonTimeoutWhereNow ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.WhereNow, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerAuditAccess ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.PAMAccessOperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.AuditAccess, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerNonTimeoutAuditAccess ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.AuditAccess, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerGrantAccess ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.PAMAccessOperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.GrantAccess, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerNonTimeoutGrantAccess ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.GrantAccess, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerRevokeAccess ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.PAMAccessOperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.RevokeAccess, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerNonTimeoutRevokeAccess ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.RevokeAccess, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerGetUserState ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.GetUserStateTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.GetUserState, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerNonTimeoutGetUserState ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.GetUserState, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerSetUserState ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.SetUserStateTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.SetUserState, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerNonTimeoutSetUserState ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.SetUserState, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerPublish ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.PublishOperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.Publish, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerNonTimeoutPublish ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.Publish, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerTime ()
		{
			string[] channel = {""}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.TimeOperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.Time, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerNonTimeoutTime ()
		{
			string[] channel = {""}; ExceptionChannel = channel[0];
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.Time, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerLeave ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.Leave, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerNonTimeoutLeave ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.Leave, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerUnsubscribe ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.Unsubscribe, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerNonTimeoutUnsubscribe ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.Unsubscribe, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerPresenceUnsubscribe ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.PresenceUnsubscribe, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerNonTimeoutPresenceUnsubscribe ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.PresenceUnsubscribe, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerHeartbeat ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<object> ("test message", channel, false,
				ResponseType.Heartbeat, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringTimeoutSubscribeReconnect ()
		{
			string[] channel = {"test"}; 
			Channels = channel;
			ResumeOnReconnect = true;
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			ExceptionHandlers.MultiplexException += HandleMultiplexException<string>;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, true,
				ResponseType.Subscribe, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutSubscribeReconnect ()
		{
			string[] channel = {"test"}; 
			Channels = channel;
			ResumeOnReconnect = true;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, true,
				ResponseType.Subscribe, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringTimeoutSubscribe ()
		{
			string[] channel = {"test"}; 
			Channels = channel;
			ResumeOnReconnect = false;
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.Subscribe, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutSubscribe ()
		{
			string[] channel = {"test"}; 
			Channels = channel;
			ResumeOnReconnect = false;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.Subscribe, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringTimeoutPresenceReconnect ()
		{
			string[] channel = {"test"}; 
			Channels = channel;
			ResumeOnReconnect = true;
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, true,
				ResponseType.Presence, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutPresenceReconnect ()
		{
			string[] channel = {"test"}; 
			Channels = channel;
			ResumeOnReconnect = true;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, true,
				ResponseType.Presence, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringTimeoutPresence ()
		{
			string[] channel = {"test"}; 
			Channels = channel;
			ResumeOnReconnect = false;
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.Presence, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutPresence ()
		{
			string[] channel = {"test"}; 
			Channels = channel;
			ResumeOnReconnect = false;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.Presence, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringDetailedHistory ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.DetailedHistoryOperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.DetailedHistory, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutDetailedHistory ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.DetailedHistory, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringHereNow ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.HereNowOperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.HereNow, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutHereNow ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.HereNow, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringGlobalHereNow ()
		{
			string[] channel = {""}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.GlobalHereNowOperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.GlobalHereNow, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutGlobalHereNow ()
		{
			string[] channel = {""}; ExceptionChannel = channel[0];
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.GlobalHereNow, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringWhereNow ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.WhereNowOperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.WhereNow, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutWhereNow ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.WhereNow, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringAuditAccess ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.PAMAccessOperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.AuditAccess, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutAuditAccess ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.AuditAccess, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringGrantAccess ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.PAMAccessOperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.GrantAccess, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutGrantAccess ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.GrantAccess, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringRevokeAccess ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.PAMAccessOperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.RevokeAccess, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutRevokeAccess ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.RevokeAccess, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringGetUserState ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.GetUserStateTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.GetUserState, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutGetUserState ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.GetUserState, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringSetUserState ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.SetUserStateTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.SetUserState, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutSetUserState ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.SetUserState, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringPublish ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.PublishOperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.Publish, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutPublish ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.Publish, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringTime ()
		{
			string[] channel = {""}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.TimeOperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.Time, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutTime ()
		{
			string[] channel = {""}; ExceptionChannel = channel[0];
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.Time, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringLeave ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.Leave, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutLeave ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.Leave, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringUnsubscribe ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.Unsubscribe, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutUnsubscribe ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.Unsubscribe, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringPresenceUnsubscribe ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.PresenceUnsubscribe, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutPresenceUnsubscribe ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.PresenceUnsubscribe, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringHeartbeat ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];
			ExceptionStatusCode = (int)PubnubErrorCode.OperationTimeout;
			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.Heartbeat, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, true, PubnubErrorFilter.Level.Critical
			);
		}

		[Test]
		public void TestUrlRequestCommonExceptionHandlerStringNonTimeoutHeartbeat ()
		{
			string[] channel = {"test"}; ExceptionChannel = channel[0];

			TestUrlRequestCommonExceptionHandlerCommon<string> ("test message", channel, false,
				ResponseType.Heartbeat, UserCallbackCommonExceptionHandler, ConnectCallbackCommonExceptionHandler, ErrorCallbackCommonExceptionHandler, false, PubnubErrorFilter.Level.Critical
			);
		}

		public void TestUrlRequestCommonExceptionHandlerCommon<T>(string message, string[] channels,
			bool resumeOnReconnect, ResponseType responseType, Action<T> userCallback,
			Action<T> connectCallback, Action<PubnubClientError> errorCallback,
			bool timeout, PubnubErrorFilter.Level errorLevel
		){
			ExceptionMessage = message;

			if (timeout) { 
				ExceptionMessage = "Operation Timeout";
			} else {
				ExceptionStatusCode = (int)PubnubErrorCode.None;
			}

			ExceptionHandlers.UrlRequestCommonExceptionHandler<T> (message, responseType, channels, 
				timeout, userCallback, connectCallback, errorCallback, resumeOnReconnect, errorLevel );
		}	

		private void HandleMultiplexException<T> (object sender, EventArgs ea)
		{
			MultiplexExceptionEventArgs<T> mea = ea as MultiplexExceptionEventArgs<T>;
			UnityEngine.Debug.Log (string.Format ("HandleMultiplexException LOG: {0} {1} {2}",
				mea.requestType.Equals (CRequestType) ,
				mea.channels.Equals (Channels),
				mea.resumeOnReconnect.Equals(ResumeOnReconnect)
			));

			Assert.True (mea.requestType.Equals (CRequestType) 
				&& mea.channels.Equals (Channels)
				&& mea.resumeOnReconnect.Equals(ResumeOnReconnect)
			);
		}


		#endif
	}
}
