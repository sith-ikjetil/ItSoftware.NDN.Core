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
using ItSoftware.Core.HttpHost;

namespace ItSoftware.NDN.Core.TestApplication
{
	class Program
	{
		static Stopwatch g_swatch = new Stopwatch();

		static void Main(string[] args)
		{
			Console.WriteLine("> Test Application - Started <");

			TestItsStopwatchStart();

			try
			{
				TestItsRegularExpressions();
				TestItsLog();
				TestItsHash();
				TestItsDataSizeString();
				TestItsWidthExpand();
				TestItsDbClient();
				TestItsID();
				TestItsRenderTimeSpan();
				TestItsRenderException();
				TestItsHttpHost();
			}
			catch (Exception y)
			{
				Console.WriteLine(y.ItsRenderException());
			}
			finally
			{
				TestItsStopwatchStop();
			}

			Console.WriteLine("> Test Application - Finished <");
		}

		static void PrintTestHeader(string name)
		{
			Console.WriteLine();
			Console.WriteLine($" {name} ".ItsWidthExpand(80, '_', ItsWidthExpandDirection.Middle));
		}

		static void TestItsStopwatchStart()
		{
			PrintTestHeader("ItsStopwatch Started");
			g_swatch.Start();
		}

		static void TestItsStopwatchStop()
		{
			PrintTestHeader("ItsStopwatch Stopped");
			
			g_swatch.Stop();
			
			Console.WriteLine($"> Elapsed time: {new TimeSpan(0,0,0,0,(int)g_swatch.ItsGetStopWatchMilliSeconds()).ItsRenderTimeSpan(true)}");
			Console.WriteLine();
		}

		static void TestItsRegularExpressions()
		{
			PrintTestHeader("ItsRegularExpressions");

			Console.WriteLine(@"a\r\naaKJETIL KRISTOFFER SOLBERGbbba\r\naaYES MANbbb"".ItsRegExPatternMatches(@""a(\s*)aa(?<bingo>[\w ]+)bbb"")");
			var match2 = "a\r\naaKJETIL KRISTOFFER SOLBERGbbba\r\naaYES MANbbb".ItsRegExPatternMatches(@"a(\s*)aa(?<bingo>[\w ]+)bbb");
			foreach (Match s in match2)
			{
				Console.WriteLine($"> {s.Groups["bingo"].Value}");
			}
			Console.WriteLine();

			Console.WriteLine(@"a\r\naaKJETIL KRISTOFFER SOLBERGbbba\r\naaYES MANbbb"".ItsRegExPatternMatchesGroupAsArray(""bingo"", @""a(\s *)aa(?< bingo >[\w] +)bbb"");");
			var match3 = "a\r\naaKJETIL KRISTOFFER SOLBERGbbba\r\naaYES MANbbb".ItsRegExPatternMatchesGroupAsArray("bingo", @"a(\s*)aa(?<bingo>[\w ]+)bbb");
			foreach (var s in match3)
			{
				Console.WriteLine($"> {s}");
			}

			Console.WriteLine();
		}

		static void TestItsLog()
		{
			PrintTestHeader("ItsLog");

			ItsLog log = new ItsLog("D:\\ItsLog.xml", "ItSoftware.NDN.Core", false);
			log.LogInformation("Information Title", "Information text");
			log.LogWarning("Warning Title", "Warning text");
			log.LogError("Error Title", "Error text");
			log.LogDebug("Debug Title", "Debug text");
			log.LogOther("Other Title", "Other text");
			
			Console.WriteLine(log.ToString());
			
			Console.WriteLine();
		}

		static void TestItsHash()
		{
			PrintTestHeader("ItsHash");

			Console.WriteLine("'kjetil' hashed MD5");
			Console.WriteLine($"> {"kjetil".ItsHashMD5(Encoding.ASCII)}");
			Console.WriteLine("'kjetil' hashed SHA1");
			Console.WriteLine($"> {"kjetil".ItsHashSHA1(Encoding.ASCII)}");
			Console.WriteLine("'kjetil' hashed SHA256");
			Console.WriteLine($"> {"kjetil".ItsHashSHA256(Encoding.ASCII)}");
			Console.WriteLine("'kjetil' hashed SHA384");
			Console.WriteLine($"> {"kjetil".ItsHashSHA384(Encoding.ASCII)}");
			Console.WriteLine("'kjetil' hashed SHA512");
			Console.WriteLine($"> {"kjetil".ItsHashSHA512(Encoding.ASCII)}");

			Console.WriteLine();
		}

		static void TestItsDataSizeString()
		{
			PrintTestHeader("ItsDataSizeString");

			Console.WriteLine("1_000_000_000");
			Console.WriteLine("> " + 1_000_000_000.ItsToDataSizeString(2, new System.Globalization.CultureInfo("en-US")));
			Console.WriteLine("int.MaxValue");
			Console.WriteLine("> " + int.MaxValue.ItsToDataSizeString(2, new System.Globalization.CultureInfo("en-US")));
			Console.WriteLine("uint.MaxValue");
			Console.WriteLine("> " + uint.MaxValue.ItsToDataSizeString(2, new System.Globalization.CultureInfo("en-US")));
			Console.WriteLine("long.MaxValue");
			Console.WriteLine("> " + long.MaxValue.ItsToDataSizeString(2, new System.Globalization.CultureInfo("en-US")));
			Console.WriteLine("ulong.MaxValue");
			Console.WriteLine("> " + ulong.MaxValue.ItsToDataSizeString(2, new System.Globalization.CultureInfo("en-US")));
			Console.WriteLine("decimal.MaxValue");
			Console.WriteLine("> " + decimal.MaxValue.ItsToDataSizeString(2, new System.Globalization.CultureInfo("en-US")));

			Console.WriteLine();
		}

		static void TestItsWidthExpand()
		{
			PrintTestHeader("ItsWidthExpand");

			string target = "Kjetil";
			Console.WriteLine(target.ItsWidthExpand(30, '_', ItsWidthExpandDirection.Left));
			Console.WriteLine(target.ItsWidthExpand(30, '_', ItsWidthExpandDirection.Middle));
			Console.WriteLine(target.ItsWidthExpand(30, '_', ItsWidthExpandDirection.Right));
			
			Console.WriteLine();
		}

		static void TestItsDbClient()
		{
			PrintTestHeader("ItsDbClient");
			//using (ItsDbClient dbClient = new ItsDbClient("data source=(local);initial catalog=birken;integrated security=True;MultipleActiveResultSets=True;", true))//ItsDbClient.DefaultConnectionString, true))
			/*using (ItsDbClient dbClient = new ItsDbClient(ItsDbClient.ActiveConnectionString, true))
			{
				SqlDataReader sdr = dbClient.ExecuteQuery("SELECT [navn], [kjønn], [alder] FROM [deltaker] ORDER BY [navn] ASC");
				while (sdr.Read())
				{
					Console.WriteLine($"{sdr[0].ToString().ItsWidthExpand(30, ' ', ItsWidthExpandDirection.Right)}\t{sdr[1]}\t{sdr[2]} år");
				}
				object count = dbClient.ExecuteScalar("SELECT COUNT(*) FROM [deltaker]");
				Console.WriteLine($"Total number of rows: {count.ToString()}");
			}*/

			Console.WriteLine();
		}

		static void TestItsID()
		{
			PrintTestHeader("ItsID");

			Console.WriteLine($"ItsID.ItsCreateID(12, ItsCreateIDOptions.LowerAndUpperCase, false)");
			Console.WriteLine($"> {ItsID.ItsCreateID(12, ItsCreateIDOptions.LowerAndUpperCase, false)}");
			Console.WriteLine($"ItsID.ItsCreateID(12, ItsCreateIDOptions.LowerAndUpperCase, true)");
			Console.WriteLine($"> {ItsID.ItsCreateID(12, ItsCreateIDOptions.LowerAndUpperCase, true)}");
			Console.WriteLine($"ItsID.ItsCreateID(12, ItsCreateIDOptions.UpperCase, false)");
			Console.WriteLine($"> {ItsID.ItsCreateID(12, ItsCreateIDOptions.UpperCase, false)}");
			Console.WriteLine($"ItsID.ItsCreateID(12, ItsCreateIDOptions.UpperCase, true)");
			Console.WriteLine($"> {ItsID.ItsCreateID(12, ItsCreateIDOptions.UpperCase, true)}");
			Console.WriteLine($"ItsID.ItsCreateID(12, ItsCreateIDOptions.LowerCase, false)");
			Console.WriteLine($"> {ItsID.ItsCreateID(12, ItsCreateIDOptions.LowerCase, false)}");
			Console.WriteLine($"ItsID.ItsCreateID(12, ItsCreateIDOptions.LowerCase, true)");
			Console.WriteLine($"> {ItsID.ItsCreateID(12, ItsCreateIDOptions.LowerCase, true)}");
			Console.WriteLine($"ItsID.ItsCreateID(64, ItsCreateIDOptions.LowerAndUpperCase, false)");
			Console.WriteLine($"> {ItsID.ItsCreateID(64, ItsCreateIDOptions.LowerAndUpperCase, false)}");
			Console.WriteLine($"ItsID.ItsCreateID(64, ItsCreateIDOptions.LowerAndUpperCase, true)");
			Console.WriteLine($"> {ItsID.ItsCreateID(64, ItsCreateIDOptions.LowerAndUpperCase, true)}");

			Console.WriteLine();
		}

		static void TestItsRenderTimeSpan()
		{
			PrintTestHeader("ItsRenderTimeSpan");

			Console.WriteLine("TimeSpan.FromSeconds(487_965_892);");
			TimeSpan ts = TimeSpan.FromSeconds(487_965_892);
			Console.WriteLine($"> {ts.ItsRenderTimeSpan(false)}");
			
			Console.WriteLine();
		}

		static void TestItsRenderException()
		{
			PrintTestHeader("ItsRenderException");

			var x = new ArgumentException("yes", new NullReferenceException());
			x.Data.Add("StringKey", "StringValue");
			x.Data.Add("TestNULL", null);
			x.Data.Add(544, 545);
			x.Data.Add(new object(), 1001);
			Console.WriteLine(x.ItsRenderException());

			Console.WriteLine();
		}

		static void TestItsHttpHost()
		{
			PrintTestHeader("ItsHttpHost");

			using (ItsHttpHost host = new ItsHttpHost(5454))
			{
				host.Start(new List<ItsMiddleware> { new Middleware1(), new Middleware2() });
				try
				{
					var p = new Process();
					p.StartInfo.UseShellExecute = true;
					p.StartInfo.FileName = "http://localhost:5454";
					p.Start();
				}
				catch (Exception x)
				{
					Console.WriteLine(x.Message);
				}
				Console.WriteLine("Server Ready at http://localhost:5454");
				System.Threading.Thread.Sleep(1500);
			}

			Console.WriteLine();
		}
	}
}
