using ItSoftware.Core.Extensions;
using ItSoftware.Core.ID;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ItSoftware.NDN.Core.TestApplication
{
	class Program
	{
		static void Main(string[] args)
		{
			var sw = new Stopwatch();
			sw.Start();
			System.Threading.Thread.Sleep(100);
			sw.Stop();
			Console.WriteLine($"{sw.ItsGetStopWatchNanoSeconds()}ns");
			Console.WriteLine($"{sw.ItsGetStopWatchMicroSeconds()}us");
			Console.WriteLine($"{sw.ItsGetStopWatchMilliSeconds()}ms");

			var match = "a\r\naaKJETIL KRISTOFFER SOLBERGbbba\r\naaYES MANbbb".ItsRegExPatternMatchesAsArray(@"a(\s*)aa([\w ]+)bbb");
			foreach (var s in match)
			{
				Console.WriteLine(s);
			}
			Console.WriteLine();
			Console.WriteLine();
			var match2 = "a\r\naaKJETIL KRISTOFFER SOLBERGbbba\r\naaYES MANbbb".ItsRegExPatternMatches(@"a(\s*)aa(?<bingo>[\w ]+)bbb");
			foreach (Match s in match2)
			{
				Console.WriteLine(s.Groups["bingo"].Value);
			}
			Console.WriteLine();
			Console.WriteLine();
			var match3 = "a\r\naaKJETIL KRISTOFFER SOLBERGbbba\r\naaYES MANbbb".ItsRegExPatternMatchesGroupAsArray("bingo", @"a(\s*)aa(?<bingo>[\w ]+)bbb");
			foreach (var s in match3)
			{
				Console.WriteLine(s);
			}
			Console.ReadKey();
			Console.WriteLine();

			//ItsLog log = new ItsLog( "D:\\ConductorTestSettings.xml", "TEST", true );
			//log.LogInformation( "Title", "Text" );
			//return;
			/*using ( ItsHttpHost host = new ItsHttpHost( 5454 ) )
			{
				host.Start( new List<ItsMiddleware> { new Middleware1( ), new Middleware2( ) } );
				Console.WriteLine( "Server Ready..Press Any Key to Exit..." );
				Console.ReadKey( );
			}*/
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

			Console.WriteLine("Hashed MD5: " + "kjetil".ItsHashMD5(Encoding.ASCII));
			Console.WriteLine("Hashed SHA1: " + "kjetil".ItsHashSHA1(Encoding.ASCII));
			Console.WriteLine("Hashed SHA256: " + "kjetil".ItsHashSHA256(Encoding.ASCII));
			Console.WriteLine("Hashed SHA384: " + "kjetil".ItsHashSHA384(Encoding.ASCII));
			Console.WriteLine("Hashed SHA512: " + "kjetil".ItsHashSHA512(Encoding.ASCII));
			Console.ReadKey();
			Console.WriteLine();

			Console.WriteLine();
			Console.WriteLine(1000000000.ItsToDataSizeString(2, new System.Globalization.CultureInfo("en-US")));
			Console.WriteLine(int.MaxValue.ItsToDataSizeString(2, new System.Globalization.CultureInfo("en-US")));
			Console.WriteLine(uint.MaxValue.ItsToDataSizeString(2, new System.Globalization.CultureInfo("en-US")));
			Console.WriteLine(long.MaxValue.ItsToDataSizeString(2, new System.Globalization.CultureInfo("en-US")));
			Console.WriteLine(ulong.MaxValue.ItsToDataSizeString(2, new System.Globalization.CultureInfo("en-US")));
			Console.WriteLine(decimal.MaxValue.ItsToDataSizeString(2, new System.Globalization.CultureInfo("en-US")));
			Console.ReadKey();

			Console.WriteLine();
			string target = "Kjetil";
			Console.WriteLine(target.ItsWidthExpand(30, '_', ItsWidthExpandDirection.Left));
			Console.WriteLine(target.ItsWidthExpand(30, '_', ItsWidthExpandDirection.Middle));
			Console.WriteLine(target.ItsWidthExpand(30, '_', ItsWidthExpandDirection.Right));
			Console.ReadKey();

			Console.WriteLine();
			string id = string.Empty;
			Console.WriteLine($"ID(12)={ItsID.ItsCreateID(12, ItsCreateIDOptions.LowerAndUpperCase, false)}");
			Console.WriteLine($"ID(12)={ItsID.ItsCreateID(12, ItsCreateIDOptions.LowerAndUpperCase, true)}");
			Console.WriteLine($"ID(12)={ItsID.ItsCreateID(12, ItsCreateIDOptions.UpperCase, false)}");
			Console.WriteLine($"ID(12)={ItsID.ItsCreateID(12, ItsCreateIDOptions.UpperCase, true)}");
			Console.WriteLine($"ID(12)={ItsID.ItsCreateID(12, ItsCreateIDOptions.LowerCase, false)}");
			Console.WriteLine($"ID(12)={ItsID.ItsCreateID(12, ItsCreateIDOptions.LowerCase, true)}");
			Console.WriteLine($"ID(64)={ItsID.ItsCreateID(64, ItsCreateIDOptions.LowerAndUpperCase, false)}");
			Console.WriteLine($"ID(64)={ItsID.ItsCreateID(64, ItsCreateIDOptions.LowerAndUpperCase, true)}");

			//var ax = new ArgumentException("MyVariable", new ArgumentNullException("test","cannot be null"));
			//Console.Write(ax.ItsRenderException());

			Console.WriteLine();
			var wc = new WebClient();
			var text = "testing <a href=\"blah\"/><cr></cr>";//wc.DownloadString(@"https://www.google.com");

			List<string> tt = new List<string>()
			{
				@"<cr>{{ref}}","{{","}}",
				@"</{{close-tag}}>"
			};
			var result = text.ItsApplyTagTemplate(tt, "{{", "}}");
			foreach (var obj in result)
			{
				obj.ItsForEach(o => Console.WriteLine($"Key={o.Key}, Value={o.Value.Aggregate((a, b) => a + ", " + b)}"));
				//foreach (var o in obj)
				//{
				//	Console.WriteLine($"Key={o.Key}, Value={o.Value.Aggregate( (a, b) => a + ", " + b )}");
				//}
			}

			Console.WriteLine();
			var stringList = new string[] { "Item#", "Item%", "Item( Item+", "Item)", "hallo" };

			var resultStringList = stringList.ItsApplyTagTemplate("Item{{sign}}", "{{", "}}");
			foreach (var obj in resultStringList)
			{
				obj.ItsForEach(o => Console.WriteLine($"Key={o.Key}, Value={o.Value.Aggregate((a, b) => a + ", " + b)}"));
			}

			Console.WriteLine();
			Console.WriteLine(stringList.Aggregate((a, b) => a + " | " + b));

			Console.ReadKey();

			Console.WriteLine();
			TimeSpan ts = TimeSpan.FromSeconds(487_965_892);
			Console.WriteLine($"TimeSpan as string: {ts.ItsRenderTimeSpan(false)}");
			Console.WriteLine();

			Console.ReadKey();

			var x = new ArgumentException("yes", new NullReferenceException());
			x.Data.Add("StringKey", "StringValue");
			x.Data.Add("TestNULL", null);
			x.Data.Add(544, 545);
			x.Data.Add(new object(), 1001);
			Console.Write(x.ItsRenderException());
		}
	}
}
