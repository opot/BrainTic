using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brain {

	public enum CellState {
		Cross, Zero, Empty, Block
	}

	struct SolveResult{
		public int row;
		public int wins;
		public int loses;

		public SolveResult(int row, int wins, int loses) {
			this.row = row;
			this.wins = wins;
			this.loses = loses;
		}

		public int CompareTo(SolveResult a) {
			if (wins > a.wins)
				return 1;
			if (wins < a.wins)
				return -1;

			if (loses < a.loses)
				return 1;
			if (loses > a.loses)
				return -1;

			return 0;
		}
	}

	public sealed class Program {

		public Program(string fold, CellState player) {

			DateTime start = System.DateTime.Now;

			Field field = new Field(fold, player);
			SolveResult res = makeTurn(invertCell(player), 4 , field,0);

			int turn = Directory.GetFiles(fold).Length + 1;
			string path = fold + (player == CellState.Cross ? "x" : "o") + turn.ToString() + ".txt";
			Console.WriteLine(path + " " + res.row.ToString());
			Console.WriteLine(analized);

			DateTime stop = System.DateTime.Now;
			File.WriteAllLines(path, new String[] { res.row.ToString(), (stop.Millisecond - start.Millisecond).ToString()});
		}

		int analized = 0;
		private SolveResult makeTurn(CellState player, int deep, Field field, int lastRow) {
			//analized++;
			CellState field_state = field.check();

			if (field_state != CellState.Empty) {
				analized++;
				return new SolveResult(lastRow, field_state == player ? 1 : 0, field_state == player ? 0 : 1);
			}
			if (deep == 0)
				return new SolveResult(lastRow, 0, 0);

			List<SolveResult> result = new List<SolveResult>();
			SolveResult thisResult = new SolveResult(0,0,0);
			thisResult = new SolveResult(lastRow, 0, 0);

			for (int i = 0; i < 10; i++) {
				//analized++;
				if (field.checkRow(i)) {
					//Console.Write(i);
					result.Add(makeTurn(invertCell(player), deep - 1, new Field(field, i, player), i));
					thisResult.wins += result[result.Count - 1].loses;
					thisResult.loses += result[result.Count - 1].wins;
				}
			}
			//Console.WriteLine();

			SolveResult max = result[0];
			for (int i = 1; i < result.Count; i++)
				if (result[i].CompareTo(max) == -1)
					max = result[i];
			thisResult.row = max.row;
			return thisResult;
		}

		public CellState invertCell(CellState player) {
			return (player == CellState.Cross) ? CellState.Zero : CellState.Cross;
        }

		static void Main(string[] args) {
			new Program(args[0], args[1][0] == 'x'?CellState.Cross: CellState.Zero);
			//Console.ReadKey();
		}
	}

}
