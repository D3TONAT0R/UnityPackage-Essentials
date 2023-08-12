using System;

namespace D3T
{
	public class MessagedException : Exception
	{
		private string message;
		private Exception rootException;

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
