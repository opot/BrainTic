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
		public float chance;

		public SolveResult(int row, float chance) {
			this.row = row;
			this.chance = chance;
		}

		public int CompareTo(SolveResult a) {
			if (chance > a.chance)
				return 1;
			if (chance < a.chance)
				return -1;
			return 0;
		}
	}

	public sealed class Program {

		public Program(string fold, CellState player) {

			//DateTime start = System.DateTime.Now;

			Field field = new Field(fold, player);
			SolveResult res = makeTurn(player, 1, field, 0);

			int turn = Directory.GetFiles(fold).Length + 1;
			string path = fold + (player == CellState.Cross ? "x" : "o") + turn.ToString() + ".txt";
			//Console.WriteLine(path + " " + res.row.ToString());
			//Console.WriteLine(analized);

			//DateTime stop = System.DateTime.Now;
			File.WriteAllLines(path, new String[] { res.row.ToString() });//, (stop.Millisecond - start.Millisecond).ToString()});
		}

		//int analized = 0;
		private SolveResult makeTurn(CellState player, int deep, Field field, int lastRow) {
			//analized++;
			CellState field_state = field.check();

			if (field_state != CellState.Empty) {
				//analized++;
				return new SolveResult(lastRow, field_state == player ? 1 : 0);
			}
			if (deep == 0)
				return new SolveResult(lastRow, 0.5f);

			List<SolveResult> result = new List<SolveResult>();
			//SolveResult thisResult = new SolveResult(lastRow, 1);

			//bool isAlmost = false;
			for (int i = 0; i < 10; i++)
				if (field.checkRow(i)) {
					result.Add(makeTurn(invertCell(player), deep - 1, new Field(field, i, player), i));
					//thisResult.chance *= (1 - result[result.Count - 1].chance);
                   
				}

			//if (!isAlmost)
				//thisResult.chance = 0;
			//Console.WriteLine();

			SolveResult min = result[0];
			for (int i = 1; i < result.Count; i++)
				if (result[i].CompareTo(min) == -1)
					min = result[i];
			return min;
			//thisResult.row = min.row;
			//return thisResult;
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
