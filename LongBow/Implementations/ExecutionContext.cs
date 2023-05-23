using System;
using System.ComponentModel.Composition;
using LongBow.Common.PersistentState;

namespace LongBow.Implementations
{
	[Export(typeof(IExecutionContext))]
	public class ExecutionContext : IExecutionContext
	{
		public string CurrentExecutionPath
		{
			get { return Environment.CurrentDirectory; }
		}
	}
}
