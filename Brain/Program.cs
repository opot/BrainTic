using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Brain {

	public enum CellState : int {
		Empty = 0, Cross = 1, Zero = -1, Block = 0
	}

	sealed class Program {

		DateTime start;
		CellState player;
		int time;
		string path;
		int calcField = 1;

		Queue<Solvation> que = new Queue<Solvation>();

		Solvation[] branch = new Solvation[10];

		public Program(string fold, CellState player, int time) {
			start = DateTime.Now;
			this.player = player;
			this.time = time;
			this.path = fold;

			Field field = new Field(fold);

			int delta = (new Random()).Next(10);

			Solvation root = new Solvation(field, Solvation.invertCell(player));
			for (int i = 0; i < 10; i++) {
				Solvation buf = new Solvation(root, (i + delta) % 10);
				calcField++;
				if (!buf.isFinalized)
					que.Enqueue(buf);
				if (buf.isMove)
					branch[i] = buf;
			}

			while ((DateTime.Now - start).TotalMilliseconds + 500 < time && que.Count != 0)
				for (int i = 0; i < 1000 && que.Count != 0; i++)
					Solve(que.Dequeue());

			Solvation max = branch[0];

				for (int i = 1; i < 10; i++)
					if (branch[i] != null)
						max = branch[i].isGreater(max);

			int turn = Directory.GetFiles(fold).Length/2 + 1;
			string path = fold + (player == CellState.Cross ? "X" : "O") + turn.ToString() + ".txt";
			File.WriteAllLines(path, new String[] { max.getTurn() });
		}

		void Solve(Solvation solve) {
			for(int i = 0; i < 10; i++) {
				Solvation buf = new Solvation(solve, (i+5)%10);
				calcField++;
				if (!buf.isFinalized)
					que.Enqueue(buf);
			}
		}

		static void Main(string[] args) {
			new Program(args[0], args[1][0] == 'X'?CellState.Cross: CellState.Zero, Convert.ToInt32(args[2]));
		}
	}

}
