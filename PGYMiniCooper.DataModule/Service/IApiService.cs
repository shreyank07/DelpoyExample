using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Prodigy.ApiAdapter.ProtoService;
using Prodigy.Interfaces;
using Prodigy.Interfaces.I3C;
using Prodigy.Interfaces.WaveformTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Service
{
    public interface IApiService
    {
        bool IsInitialized { get; }

        event Action<Google.Protobuf.IMessage> OnEventReceived;

        void Connect(string userName);
        Task Disconnect();
        void Initialize();
        Task<TResponse> RequestMessageAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellation)
            where TRequest : Google.Protobuf.IMessage
            where TResponse : Google.Protobuf.IMessage;

        TResponse RequestMessage<TRequest, TResponse>(TRequest request, CancellationToken cancellation)
            where TRequest : Google.Protobuf.IMessage
            where TResponse : Google.Protobuf.IMessage;

        void SendMessage(Google.Protobuf.IMessage message);
    }

    internal class ApiService : IApiService
    {
        private readonly FileDescriptor[] fileDescriptors = new[]
        {
               CaptureServiceReflection.Descriptor,
                CommonTypesReflection.Descriptor,
                CaptureTypesReflection.Descriptor,
                DecoderTypesReflection.Descriptor,
                I3CFrameReflection.Descriptor,
                WaveformTypesReflection.Descriptor,
                
        };
        private readonly TypeRegistry? typeRegistry;
        private ProtoService.ProtoServiceClient? service;
        private CancellationTokenSource cancellationTokenSource;

        private bool isInitialized = false;
        public bool IsInitialized { get => isInitialized; private set => isInitialized = value; }

        public event Action<Google.Protobuf.IMessage> OnEventReceived;

        public ApiService() 
        {
            typeRegistry = TypeRegistry.FromFiles(fileDescriptors);
        }

        public void Initialize()
        {
            try
            {
                if (IsInitialized) return;

                string apiServiceUrl = System.Configuration.ConfigurationManager.AppSettings["ApiServiceUrl"];
                var channel = GrpcChannel.ForAddress(apiServiceUrl);
                
                service = new Prodigy.ApiAdapter.ProtoService.ProtoService.ProtoServiceClient(channel);

                IsInitialized = true;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private Task runLookTask = Task.CompletedTask;
        public async Task RunEventLoop(CancellationToken cancellation)
        {
            Empty request = new Empty();
            MessageWrapper response = null;
            while (cancellation.IsCancellationRequested == false)
            {
                try
                {
                    response = await service.RequestEventAsync(request, new Grpc.Core.CallOptions(cancellationToken: cancellation));

                    if (response.Message == null || response.Message.TryUnpack<Empty>(out _))
                        await Task.Delay(500, cancellation);
                    else
                        OnEventReceived?.Invoke(response.Message.Unpack(typeRegistry));
                }
                catch (TaskCanceledException)
                {
                    // Expected error when we have cancelled the operation
                    break;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    await Task.Delay(500, cancellation);
                }
            }
        }

        public void Connect(string userName)
        {
            cancellationTokenSource = new CancellationTokenSource();

            runLookTask = RunEventLoop(cancellationTokenSource.Token);

            var appServerAddress = System.Configuration.ConfigurationManager.AppSettings["AppServerAddress"];

            service.Connect(new Prodigy.ApiAdapter.ProtoService.ConnectMessage { UserName = userName, AppServerAddress = appServerAddress, IsAnalyzer = true });
        }

        public async Task Disconnect()
        {
            try
            {
                cancellationTokenSource.Cancel();

                await runLookTask;

                await service.DisconnectAsync(new Empty());
            }
            catch (Grpc.Core.RpcException) { /* Task cancelled rpc exception -> can be ignored*/ }
        }

        public void SendMessage(Google.Protobuf.IMessage message)
        {
            service.SendMessage(new Prodigy.ApiAdapter.ProtoService.MessageWrapper { Message = Any.Pack(message) });
        }

        public async Task<TResponse> RequestMessageAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellation)
            where TRequest : Google.Protobuf.IMessage
            where TResponse : Google.Protobuf.IMessage
        {
            try
            {
                var responseMessage = await service.RequestAsync(new Prodigy.ApiAdapter.ProtoService.MessageWrapper { Message = Any.Pack(request) },
                    new Grpc.Core.CallOptions(cancellationToken: cancellation));

                // Error invalid result -> may happen due to timeout
                if (responseMessage.ResponseMessage.Is(Empty.Descriptor))
                    return default;

                return (TResponse)responseMessage.ResponseMessage.Unpack(typeRegistry);
            }
            catch
            {
                throw;
            }
        }

        public TResponse RequestMessage<TRequest, TResponse>(TRequest request, CancellationToken cancellation)
            where TRequest : Google.Protobuf.IMessage
            where TResponse : Google.Protobuf.IMessage
        {
            try
            {
                var responseMessage = service.Request(new Prodigy.ApiAdapter.ProtoService.MessageWrapper { Message = Any.Pack(request) },
                    new Grpc.Core.CallOptions(cancellationToken: cancellation));

                // Error invalid result -> may happen due to timeout
                if (responseMessage.ResponseMessage.Is(Empty.Descriptor))
                    return default;

                return (TResponse)responseMessage.ResponseMessage.Unpack(typeRegistry);
            }
            catch
            {
                throw;
            }
        }
    }
}
