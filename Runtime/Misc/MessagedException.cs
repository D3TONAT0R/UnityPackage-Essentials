using System;

namespace UnityEssentials
{
	/// <summary>
	/// Adds a custom message to an existing exception.
	/// </summary>
	public class MessagedException : Exception
	{
		private readonly string message;
		private readonly Exception innerException;

		public override string Message => $"{message}: {innerException?.Message}";
		public override string StackTrace => innerException?.StackTrace;

		public MessagedException(string message, Exception innerException) : base(message, innerException)
		{
			this.innerException = innerException;
			this.message = message;
		}

		public override Exception GetBaseException()
		{
			return innerException;
		}

	}
}
