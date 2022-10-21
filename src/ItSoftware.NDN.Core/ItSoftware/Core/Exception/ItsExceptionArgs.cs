using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItSoftware.Core.Exception
{
	[Serializable]
	public abstract class ItsExceptionArgs
	{
		public virtual string Message { get { return string.Empty; } }
	}
}
