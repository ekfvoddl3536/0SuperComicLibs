using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperComicLib.Threading
{
	public static class ParallelEx
	{
		public static void While(Func<bool> body) =>
			Parallel.ForEach(InfEnumerator.Default, new SetWhile(body).CB);

		public static void While(ParallelOptions options, Func<bool> body) =>
			Parallel.ForEach(InfEnumerator.Default, options, new SetWhile(body).CB);

		private readonly struct SetWhile
		{
			private readonly Func<bool> body;

			public SetWhile(Func<bool> body) => this.body = body;

			public void CB(bool _, ParallelLoopState state)
			{
				if (!body.Invoke())
                    state.Stop();
			}
		}

		private sealed class InfEnumerator : IEnumerator<bool>, IEnumerable<bool>
		{
			public static readonly InfEnumerator Default = new InfEnumerator();

			private InfEnumerator() { }

			bool IEnumerator<bool>.Current => true;

			object IEnumerator.Current => true;

			void IDisposable.Dispose() { }

			void IEnumerator.Reset() { }

			bool IEnumerator.MoveNext() => true;

			public IEnumerator<bool> GetEnumerator() => this;

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}