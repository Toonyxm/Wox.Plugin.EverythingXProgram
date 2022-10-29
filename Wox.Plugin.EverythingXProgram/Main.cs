using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Wox.Plugin;
using static System.Net.Mime.MediaTypeNames;

namespace Wox.Plugin.EverythingXProgram
{
	public class Main : IPlugin
	{
		static string[] Extension = new string[] { ".bat", ".appref-ms", ".exe", ".lnk" };

		void IPlugin.Init(PluginInitContext context)
		{
			//throw new NotImplementedException();
		}

		List<Result> IPlugin.Query(Query query)
		{
			//MessageBox.Show("IPlugin.Query(Query query)");

			List<Result> result = new List<Result>();

			var res = FindProgram(query.RawQuery);

			foreach (var item in res)
			{
				Result newResult = new Result();
				newResult.Title = item.Name;
				newResult.SubTitle = item.FullName;
				newResult.IcoPath = item.FullName;

				result.Add(newResult);
			}

			return result;
		}

		public static List<FileInfo> FindProgram(string query)
		{
			List<FileInfo> result = new List<FileInfo>();

			Everything.Everything_SetSearchW(query);
			Everything.Everything_QueryW(true);
			int fileNum = Everything.Everything_GetNumResults();

			for (int i = 0; i < fileNum; i++)
			{
				const int bufsize = 1024;
				StringBuilder buf = new StringBuilder(bufsize);

				if (Everything.Everything_IsFileResult(i) == false)
					continue;

				Everything.Everything_GetResultFullPathNameW(i, buf, bufsize);

				FileInfo fileInfo = new FileInfo(buf.ToString());

				if (Extension.Contains(fileInfo.Extension))
				{
					result.Add(fileInfo);
				}
			}

			return result.OrderBy(d => d.Name).ToList();
		}
	}

	public class Test
	{
		public static void RunTest()
		{
			var res = EverythingXProgram.Main.FindProgram("launch");

			foreach (FileInfo file in res )
			{
				Console.WriteLine(file.FullName);
			}
		}
	}

	class Everything
	{
		[DllImport("Everything64.dll", CharSet = CharSet.Unicode)]
		public static extern int Everything_SetSearchW(string lpSearchString);
		[DllImport("Everything64.dll")]
		public static extern void Everything_SetMatchPath(bool bEnable);
		[DllImport("Everything64.dll")]
		public static extern void Everything_SetMatchCase(bool bEnable);
		[DllImport("Everything64.dll")]
		public static extern void Everything_SetMatchWholeWord(bool bEnable);
		[DllImport("Everything64.dll")]
		public static extern void Everything_SetRegex(bool bEnable);
		[DllImport("Everything64.dll")]
		public static extern void Everything_SetMax(int dwMax);
		[DllImport("Everything64.dll")]
		public static extern void Everything_SetOffset(int dwOffset);
		[DllImport("Everything64.dll")]
		public static extern void Everything_SetReplyWindow(IntPtr hWnd);
		[DllImport("Everything64.dll")]
		public static extern void Everything_SetReplyID(int nId);

		[DllImport("Everything64.dll")]
		public static extern bool Everything_GetMatchPath();
		[DllImport("Everything64.dll")]
		public static extern bool Everything_GetMatchCase();
		[DllImport("Everything64.dll")]
		public static extern bool Everything_GetMatchWholeWord();
		[DllImport("Everything64.dll")]
		public static extern bool Everything_GetRegex();
		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_GetMax();
		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_GetOffset();
		[DllImport("Everything64.dll")]
		public static extern string Everything_GetSearch();
		[DllImport("Everything64.dll")]
		public static extern int Everything_GetLastError();
		[DllImport("Everything64.dll")]
		public static extern IntPtr Everything_GetReplyWindow();
		[DllImport("Everything64.dll")]
		public static extern int Everything_GetReplyID();

		[DllImport("Everything64.dll")]
		public static extern bool Everything_QueryW(bool bWait);

		[DllImport("Everything64.dll")]
		public static extern bool Everything_IsQueryReply(int message, IntPtr wParam, IntPtr lParam, uint nId);

		[DllImport("Everything64.dll")]
		public static extern void Everything_SortResultsByPath();

		[DllImport("Everything64.dll")]
		public static extern int Everything_GetNumFileResults();
		[DllImport("Everything64.dll")]
		public static extern int Everything_GetNumFolderResults();
		[DllImport("Everything64.dll")]
		public static extern int Everything_GetNumResults();
		[DllImport("Everything64.dll")]
		public static extern int Everything_GetTotFileResults();
		[DllImport("Everything64.dll")]
		public static extern int Everything_GetTotFolderResults();
		[DllImport("Everything64.dll")]
		public static extern int Everything_GetTotResults();
		[DllImport("Everything64.dll")]
		public static extern bool Everything_IsVolumeResult(int nIndex);
		[DllImport("Everything64.dll")]
		public static extern bool Everything_IsFolderResult(int nIndex);
		[DllImport("Everything64.dll")]
		public static extern bool Everything_IsFileResult(int nIndex);
		[DllImport("Everything64.dll", CharSet = CharSet.Unicode)]
		public static extern void Everything_GetResultFullPathNameW(int nIndex, StringBuilder lpString, int nMaxCount);
		[DllImport("Everything64.dll")]
		public static extern void Everything_Reset();

		const int MY_REPLY_ID = 0;
	}
}
