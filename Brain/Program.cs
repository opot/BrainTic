using System;
using System.IO;
using System.Collections.Generic;

namespace Brain {

	public enum CellState : int {
		Empty = 0, Cross = 1, Zero = -1, Block = 0
	}

	sealed class Program {

		DateTime start;
		Queue<Solution> que = new Queue<Solution>();

		Solution[] branch = new Solution[10];

		public Program(string fold, CellState player, int time){
			start = DateTime.Now;

			Field field = new Field(fold);
			int delta = (new Random()).Next(10);

			Solution root = new Solution(field, Solution.invertCell(player));
			for (int i = 0; i < 10; i++) {
				Solution buf = new Solution(root, (i + delta) % 10);
				branch[i] = buf;
				if (!buf.isFinalized)
					que.Enqueue(buf);
			}

			while ((DateTime.Now - start).TotalMilliseconds + 300 < time && que.Count != 0)
				Solve(que.Dequeue());

			Solution max = branch[0];

				for (int i = 1; i < 10; i++)
					if (branch[i] != null)
						max = branch[i].isGreater(max);

			int turn = Directory.GetFiles(fold).Length/2 + 1;
			string path = fold + (player == CellState.Cross ? "X" : "O") + turn.ToString() + ".txt";
			File.WriteAllLines(path, new String[] { max.getTurn() });
		}

		void Solve(Solution solve) {
			for(int i = 0; i < 10; i++) {
				Solution buf = new Solution(solve, i);
				if (!buf.isFinalized)
					que.Enqueue(buf);
			}
		}

		static void Main(string[] args) {
			try {
				new Program(args[0], args[1][0] == 'X' ? CellState.Cross : CellState.Zero, Convert.ToInt32(args[2]));
			} catch (Exception e) {
				File.WriteAllLines(args[0] + "Exception.txt", new string[] { e.Message ,e.StackTrace });
			}
		}
	}

}
