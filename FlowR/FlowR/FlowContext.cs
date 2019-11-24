using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowR
{
    public class FlowContext
    {
        #region Member declarations

        private IDictionary<(Type, string), Func<IFlowStepRequest, object>> MockActivityHandlers { get; } =
            new Dictionary<(Type, string), Func<IFlowStepRequest, object>>();

        #endregion

        #region Constructors

        public FlowContext()
        {
        }

        public FlowContext(string correlationId, string requestId)
        {
            CorrelationId = correlationId;
            RequestId = requestId;
        }

        internal FlowContext(Type flowRequestType, FlowContext parentContext)
        {
            FlowInstanceId = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 22);
            FlowName = flowRequestType.Name;
            FlowStepName = "Flow";

            ParentContext = parentContext;
            CorrelationId = parentContext?.CorrelationId ?? Guid.NewGuid().ToString();
            RequestId = parentContext?.RequestId ?? Guid.NewGuid().ToString();
            FlowRootInstanceId = parentContext?.FlowRootInstanceId ?? FlowInstanceId;
            MockActivityHandlers = parentContext?.MockActivityHandlers ?? MockActivityHandlers;
        }

        private FlowContext(FlowContext flowContext, string flowStepName)
        {
            FlowRootInstanceId = flowContext.FlowRootInstanceId;
            FlowName = flowContext.FlowName;
            FlowStepName = flowStepName;

            ParentContext = flowContext;
            CorrelationId = flowContext.CorrelationId;
            RequestId = flowContext.RequestId;
            FlowInstanceId = flowContext.FlowInstanceId;
            MockActivityHandlers = flowContext?.MockActivityHandlers ?? MockActivityHandlers;
        }

        #endregion

        #region Properties

        public string CorrelationId { get; }

        public string RequestId { get; }

        public string FlowRootInstanceId { get; }

        public string FlowInstanceId { get; }

        public string FlowName { get; }

        public string FlowStepName { get; }

        public FlowContext ParentContext { get; }

        public bool IsRootFlow => FlowInstanceId == FlowRootInstanceId;

        #endregion

        #region Public methods

        public FlowContext MockActivity<TReq, TRes>(Func<TReq, TRes> mockHandler) where TReq : FlowActivityRequest<TRes>
        {
            return MockActivity(null, mockHandler);
        }

        public FlowContext MockActivity<TReq, TRes>(string stepName, Func<TReq, TRes> mockHandler)
            where TReq : FlowActivityRequest<TRes>
        {
            MockActivityHandlers.Add((typeof(TReq), stepName), request => mockHandler((TReq)request));
            return this;
        }

        #endregion

        #region Internal methods

        internal FlowContext GetStepContext(string stepName)
        {
            return new FlowContext(this, stepName);
        }

        internal Func<IFlowStepRequest, object> GetMockActivityHandler(Type requestType, string stepName)
        {
            if (!this.IsRootFlow)
            {
                return null;
            }

            if (MockActivityHandlers.TryGetValue((requestType, stepName), out var instanceMockHandler))
            {
                return instanceMockHandler;
            }

            if (MockActivityHandlers.TryGetValue((requestType, null), out var typeMockHandler))
            {
                return typeMockHandler;
            }

            return null;
        }

        #endregion
    }
}
