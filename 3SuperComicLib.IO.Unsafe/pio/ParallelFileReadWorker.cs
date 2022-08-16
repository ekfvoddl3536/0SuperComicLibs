namespace SuperComicLib.IO.AdvancedParallel
{
    internal sealed unsafe class ParallelFileReadWorker
    {
        private readonly IParallelFileReader _owner;
        private readonly ParallelReadHandler _body;
        private readonly Range64 _offset;
        private readonly int _bufferSize;
        private readonly int _workerIndex;

        public ParallelFileReadWorker(
            IParallelFileReader owner, 
            ParallelReadHandler body, 
            in Range64 offset,
            int bufferSize,
            int workerIndex)
        {
            _owner = owner;
            _body = body;
            _offset = offset;
            _bufferSize = bufferSize;
            _workerIndex = workerIndex;
        }

        public void Run()
        {
            var ops = new OffsetParallelStream(_owner, _offset, _bufferSize);

            _body.Invoke(ref ops, (uint)_workerIndex);
        }
    }
}
