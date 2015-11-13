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

		public bool CompareTo(SolveResult a) {
			if (loses > a.loses)
				return true;
			if (loses < a.loses)
				return false;

			if (wins < a.wins)
				return true;

			return false;
		}
	}

	public sealed class Program {

		public Program(string fold, CellState player) {

			Field field = new Field(fold, player);

			List<SolveResult> result = new List<SolveResult>(10);

			for (int i = 0; i < 10; i++) {
				if (field.checkRow(i)) {
					SolveResult cur = makeTurn(invertCell(player), 4, new Field(field, i, player), i);
					result.Add(cur);
				}
			}

			SolveResult min = result[0];
			for (int i = 1; i < result.Count; i++)
				if (min.CompareTo(result[i]))
					min = result[i];

			int turn = Directory.GetFiles(fold).Length + 1;
			string path = fold + (player == CellState.Cross ? "x" : "o") + turn.ToString() + ".txt";
			File.WriteAllLines(path, new String[] { min.row.ToString() });
		}

		private SolveResult makeTurn(CellState player, int deep, Field field, int lastRow) {
			CellState field_state = field.check();

			if (field_state != CellState.Empty)
				return new SolveResult(lastRow, field_state == player ? 1 : 0, field_state == player ? 0 : 1);

			if (deep == 0)
				return new SolveResult(lastRow, 0, 0);

			List<SolveResult> result = new List<SolveResult>(10);
			SolveResult thisResult = new SolveResult(lastRow, 0, 0);

			for (int i = 0; i < 10; i++) {
				if (field.checkRow(i)) {
					SolveResult cur = makeTurn(invertCell(player), deep - 1, new Field(field, i, player), i);
					thisResult.wins += cur.loses;
					thisResult.loses += cur.wins;
					result.Add(cur);
				}
			}

			SolveResult min = result[0];
			for (int i = 1; i < result.Count; i++)
				if (min.CompareTo(result[i]))
					min = result[i];
			//thisResult.row = min.row;
			return thisResult;
		}

		public CellState invertCell(CellState player) {
			return (player == CellState.Cross) ? CellState.Zero : CellState.Cross;
        }

		static void Main(string[] args) {
			new Program(args[0], args[2][0] == 'x'?CellState.Cross: CellState.Zero);
			//Console.ReadKey();
		}
	}

}
