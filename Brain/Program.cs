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

	public sealed class Program {

		public Program(string fold, CellState player) {

			Field field = new Field(fold);

		}

		static void Main(string[] args) {
			new Program(args[0], args[1][0] == 'x'?CellState.Cross: CellState.Zero);
			Console.ReadKey();
		}
	}

	struct Field {
		CellState[][] cells;

		public Field(String folder) {
			cells = new CellState[10][];
			for (int i = 0; i < 10; i++) {
				cells[i] = new CellState[10];
				for (int j = 0; j < 10; j++)
					cells[i][j] = CellState.Empty;
			}
			cells[9][4] = CellState.Block;
			cells[9][5] = CellState.Block;

			string[] turns = Directory.GetFiles(folder);		
			for(int i = 0; i < turns.Length; i++) {
				StringReader sr = new StringReader(turns[i]);
				addSymb(turns[i][0] == 'x' ? CellState.Cross : CellState.Zero, Convert.ToInt32(sr.ReadLine()));
			}

			DebugOutput();

        }

		public Field(Field last, int row, CellState player) {
			cells = last.cells;
			this.addSymb(player, row);
		}

		private void addSymb(CellState player, int row) {
			if (row != 4 && row != 5) {
				for (int i = 8; i >= 0; i--)
					cells[row][i + 1] = cells[row][i];
				cells[row][0] = player;
			} else {
				for (int i = 8; i > 0; i--)
					cells[row][i + 1] = cells[row][i];
				cells[row][1] = player;
			}

			DebugOutput();

		}

		public void DebugOutput() {
			for (int i = 0; i < 10; i++) {
				for (int j = 0; j < 10; j++)
					Console.Write(cells[i][j] + " ");
				Console.WriteLine(" ");
			}
		}

	}

}
