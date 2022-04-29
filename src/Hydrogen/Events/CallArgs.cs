namespace Hydrogen {

	public class CallArgs {
		private readonly object[] _args;

		public CallArgs(params object[] args) {
			_args = args ?? new object[0];
		}

		public object this[int index] {
			get => _args[index];
			set => _args[index] = value;
		}

		public int ArgCount => _args.Length;

		//public object[] Args { get; set; }

	}

}