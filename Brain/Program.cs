using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Solvation = Brain.Field.Solvation;

namespace Brain {

	public enum CellState {
		Empty, Cross, Zero, Block
	}

	sealed class Program {

		DateTime start = DateTime.Now;
		CellState player;
		int time;

		int calcField = 1;
		int nextCycleLength = 0;
		int curCycleLength = 1;
		Queue<Solvation> que = new Queue<Solvation>();

		Solvation[] branch = new Solvation[10];

		public Program(string fold, CellState player, int time) {
			this.player = player;
			this.time = time;

			Field field = new Field(fold);
			Solve(new Solvation(field, player));
			que.CopyTo(branch, 0);

			while (isinTime())
				for (int i = 0; i < curCycleLength; i++)
					Solve(que.Dequeue());

			Solvation max = branch[0];
			for (int i = 1; i < 10; i++)
				max = branch[i].isGreater(max);


			int turn = Directory.GetFiles(fold).Length + 1;
			string path = fold + (player == CellState.Cross ? "x" : "o") + turn.ToString() + ".txt";
			File.WriteAllLines(path, new String[] { max.getTurn() });
		}

		void Solve(Solvation solve) {
			for(int i = 0; i < 10; i++) {
				Solvation buf = new Solvation(solve, i);
				if (!buf.isFinalized) {
					que.Enqueue(buf);
					nextCycleLength++;
					calcField++;
				}
			}
		}

		bool isinTime() {
			int d = (DateTime.Now.Second - start.Second) * 1000 + (DateTime.Now.Millisecond - start.Millisecond);
			float needtime = (d / (float)calcField)*nextCycleLength*1.1f + 100;
			Console.WriteLine(d);
			return needtime <= time;
		}

		static void Main(string[] args) {
			new Program(args[0], args[2][0] == 'x'?CellState.Cross: CellState.Zero, Convert.ToInt32(args[2]));
			Console.ReadKey();
		}
	}

}
