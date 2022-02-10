using System;
using System.Threading.Tasks;

namespace SuperComicLib.Threading
{
    public static class ParallelEx
	{
		public static readonly ParallelOptions defaultOpt = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

		public static void While(Func<bool> body) => 
			While(defaultOpt, body);

		public static void While(int worker_count, Func<bool> body) => 
			While(new ParallelOptions { MaxDegreeOfParallelism = worker_count }, body);

		public static void While(ParallelOptions option, Func<bool> body)
        {
			Action[] acts = new Action[option.MaxDegreeOfParallelism];
			Action local = new SetWhile(body).CB;
			for (int i = option.MaxDegreeOfParallelism; --i >= 0;)
				acts[i] = local;

			Parallel.Invoke(option, acts);
        }

		public static void BatchFor(
            int from_inclusive,
            int to_exclusive,
            int batch_size,
            ParallelOptions options,
            Action<int, BatchLoopState> body)
        {
			int add_idx = batch_size * (options.MaxDegreeOfParallelism - 1);

			Action[] acts = new Action[options.MaxDegreeOfParallelism];

			BatchLoopState state = new BatchLoopState();

			for (int i = options.MaxDegreeOfParallelism; --i >= 0;)
				acts[i] = new BatchForWorker(from_inclusive * batch_size, to_exclusive, add_idx, batch_size, body, state).Run;

			Parallel.Invoke(options, acts);
        }

		private struct SetWhile
		{
			private readonly Func<bool> body;
			private volatile int block;

            public SetWhile(Func<bool> body)
            {
                this.body = body;
				block = default;
            }

			public void CB()
			{
				while (block == 0 && body.Invoke())
					;

				block = 1;
			}
		}

		private sealed class BatchForWorker
        {
			public int sidx_cur;
			public int sidx_add;
			public int eidx;
			public int batch_sz;
			public Action<int, BatchLoopState> body;
			public BatchLoopState state;

			public BatchForWorker(int sidx, int eidx, int add, int bsize, Action<int, BatchLoopState> body, BatchLoopState state)
            {
				sidx_cur = sidx;
				sidx_add = add;

				batch_sz = bsize;

				this.eidx = eidx;

				this.body = body;
				this.state = state;
            }

			public void Run()
            {
				int sidx = sidx_cur;
				int add = sidx_add;
				Action<int, BatchLoopState> body = this.body;
				BatchLoopState state = this.state;

				int bsize = batch_sz;
				for (int i = eidx; sidx < i && state.NeedBreak(sidx); sidx += add)
					for (int j = 0; j++ < bsize && state.NeedBreak(sidx); sidx++)
					{
						body.Invoke(sidx, state);

						if (state.m_state == BatchLoopState.STATE_BREAK)
							state.m_lowestBreak.Min(sidx);
					}
            }
        }
	}
}