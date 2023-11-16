using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using EException = System.Exception;
namespace ItSoftware.Core.Exception
{
	[Serializable]
	public class ItsException<TItsExceptionArgs> : EException, ISerializable
		where TItsExceptionArgs : ItsExceptionArgs, new()
	{
		private const string c_args = "Args";

		private TItsExceptionArgs m_args = new TItsExceptionArgs();
		public TItsExceptionArgs Args { get { return this.m_args; } }

		public ItsException()
			: base()
		{
			
		}

		public ItsException(string message)
			: base(message)
		{

		}

		public ItsException(string message, EException innerException)
			: base(message, innerException)
		{
		}

		public ItsException(TItsExceptionArgs args, string message, EException innerException)
			: base(message, innerException)
		{
			this.m_args = args;
		}

		public ItsException(SerializationInfo info, StreamingContext context)
		{
			this.m_args = (TItsExceptionArgs)info.GetValue(c_args, typeof(TItsExceptionArgs))!;
		}

		//public override void GetObjectData(SerializationInfo info, StreamingContext context)
		//{
		//	info.AddValue(c_args, m_args);
		//	base.GetObjectData(info, context);
		//}

		public override string Message
		{
			get
			{
				var msg = new StringBuilder();
				msg.AppendLine("## EXCEPTION ##");
				msg.AppendLine(base.Message);
				msg.AppendLine("## EXCEPTION ARGUMENT ##");
				msg.AppendLine(this.m_args.Message);

				return msg.ToString();
			}
		}

		public override string ToString()
		{
			var tos = new StringBuilder();

			this.AppendInnerExceptions(tos);
			tos.AppendLine("## EXCEPTION ##");
			tos.AppendLine(base.ToString());
			tos.AppendLine("## EXCEPTION ARGUMENT ##");
			tos.AppendLine(this.m_args.Message);

			return tos.ToString();
		}

		private void AppendInnerExceptions(StringBuilder tos)
		{
			EException inner = this.InnerException!;
			while (inner != null )
			{
				tos.AppendLine("## INNER EXCEPTION ##");
				tos.AppendLine(inner.ToString());
				inner = inner.InnerException!;
			}
		}
	}
}
