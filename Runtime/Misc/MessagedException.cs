using System;

namespace UnityEssentials
{
	/// <summary>
	/// Adds a custom message to an existing exception.
	/// </summary>
	public class MessagedException : Exception
	{
		private readonly string message;
		private readonly Exception rootException;

		public override string Message => $"{message}: {rootException.Message}";
		public override string StackTrace => rootException.StackTrace;

		public MessagedException(string message, Exception rootException) : base()
		{
			this.rootException = rootException;
			this.message = message;
		}

		public override Exception GetBaseException()
		{
			return rootException;
		}

	}
}
