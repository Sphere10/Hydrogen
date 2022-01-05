using System;
using System.Collections.Generic;
using System.IO;

namespace Sphere10.Framework {

	public interface IStreamContainer {

		int Count { get; }

		Stream Add();

		Stream Open(int index);

		void Remove(int index);

		void Insert(int index);

		void Swap(int first, int second);

		void Clear();

	}

}
