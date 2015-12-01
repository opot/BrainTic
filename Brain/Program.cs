using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Brain {

	public enum CellState {
		Empty, Cross, Zero, Block
	}

	sealed class Program {

		DateTime start;
		CellState player;
		int time;
		string path;

		int depth = 0;
		int calcField = 1;
		int nextCycleLength = 0;
		int curCycleLength = 1;
		Queue<Solvation> que = new Queue<Solvation>();

		Solvation[] branch = new Solvation[10];

		public Program(string fold, CellState player, int time) {
			start = DateTime.Now;
			this.player = player;
			this.time = time;
			this.path = fold;

			Field field = new Field(fold);

			Solve(new Solvation(field, Solvation.invertCell(player)));
			que.CopyTo(branch, 0);

			curCycleLength = nextCycleLength;
			nextCycleLength = 0;
			while (isinTime()) {
				for (int i = 0; i < curCycleLength; i++)
					Solve(que.Dequeue());

				calcField += curCycleLength;
				curCycleLength = nextCycleLength;
				nextCycleLength = 0;
				depth++;
			}


			Solvation max = branch[0];

			try {
				for (int i = 1; i < 10; i++)
					if (branch[i] != null)
						max = branch[i].isGreater(max);

			int turn = Directory.GetFiles(fold).Length/2 + 1;
			string path = fold + (player == CellState.Cross ? "X" : "O") + turn.ToString() + ".txt";
			File.WriteAllLines(path, new String[] { max.getTurn() });
			} catch (NullReferenceException e) {
				String[] s = new String[11];
				for (int i = 0; i < 10; i++)
					s[i] = branch[i] == null ? "null" : branch[i].getTurn();
				s[10] = e.Message;
				File.WriteAllLines(fold + (player == CellState.Cross ? "X" : "O") + 
									(Directory.GetFiles(fold).Length / 2 + 1).ToString() + ".txt", s);
				return;
			}
		}

		void Solve(Solvation solve) {
			for(int i = 0; i < 10; i++) {
				Solvation buf = new Solvation(solve, (i+5)%10);
				if (!buf.isFinalized) {
					que.Enqueue(buf);
					nextCycleLength++;
				}
			}
		}

		bool isinTime() {
			double d = (DateTime.Now - start).TotalMilliseconds;
			double needtime = d*(1 + 1.5 / calcField*curCycleLength) + 100;
			return needtime < time;
		}

		static void Main(string[] args) {
			new Program(args[0], args[1][0] == 'X'?CellState.Cross: CellState.Zero, Convert.ToInt32(args[2]));
			//for (int i = 0; i < args.Length; i++)
				//Console.WriteLine(args[i]);
			//File.WriteAllLines(args[0]+"out.txt", args);
			//Console.ReadKey();
		}
	}

}
