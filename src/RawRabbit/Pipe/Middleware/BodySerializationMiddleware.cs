﻿using System;
using System.Threading;
using System.Threading.Tasks;
using RawRabbit.Serialization;

namespace RawRabbit.Pipe.Middleware
{
	public class MessageSerializationOptions
	{
		public Func<IPipeContext, object> MessageFunc { get; set; }
		public string SerializedMessageKey { get; set; }
	}

	public class BodySerializationMiddleware : Middleware
	{
		private readonly ISerializer _serializer;
		private readonly Func<IPipeContext, object> _msgFunc;
		private readonly string _serializedMessageKey;

		public BodySerializationMiddleware(ISerializer serializer, MessageSerializationOptions options = null)
		{
			_serializer = serializer;
			_msgFunc = options?.MessageFunc ?? (context => context.GetMessage());
			_serializedMessageKey = options?.SerializedMessageKey ?? PipeKey.SerializedMessage;
		}

		public override Task InvokeAsync(IPipeContext context, CancellationToken token)
		{
			var message = _msgFunc(context);
			var serialized = _serializer.Serialize(message);
			context.Properties.Add(_serializedMessageKey, serialized);
			return Next.InvokeAsync(context, token);
		}
	}
}
