using System;

namespace LongBow.Common.Interfaces.Tabulation
{
	public interface IClosable
	{
		Action Close { get; set; }
	}
}
