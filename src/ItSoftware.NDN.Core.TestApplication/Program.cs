using ItSoftware.Core.Extensions;
using ItSoftware.Core.ID;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using ItSoftware.Core.Log;
//using ItSoftware.Core.HttpHost;

namespace ItSoftware.NDN.Core.TestApplication
{
	class Program
	{
		static void Main(string[] args)
		{
			Tests tests = new Tests();
			tests.RunTests();
		}
	}
}
