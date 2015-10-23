using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brain {
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
			for (int i = 0; i < turns.Length / 2; i++) {
				String s = File.ReadAllLines(turns[i + turns.Length / 2])[0];
				//Console.Write(turns[i + turns.Length/2].Split('/')[1] + " ");
				addSymb(turns[i + turns.Length / 2].Split('/')[1][0] == 'x' ? CellState.Cross : CellState.Zero, Convert.ToInt32(s));

				s = File.ReadAllLines(turns[i])[0];
				//Console.Write(turns[i].Split('/')[1] + " ");
				addSymb(turns[i].Split('/')[1][0] == 'x' ? CellState.Cross : CellState.Zero, Convert.ToInt32(s));
			}

			if (turns.Length % 2 == 1) {
				String s = File.ReadAllLines(turns[turns.Length - 1])[0];
				//Console.Write(turns[i + turns.Length / 2].Split('/')[1] + " ");
				addSymb(CellState.Cross, Convert.ToInt32(s));
			}

			Console.WriteLine(" ");
			DebugOutput();

		}

		public Field(Field last, int row, CellState player) {
			cells = last.cells;
			this.addSymb(player, row);
		}

		private void addSymb(CellState player, int row) {
			if (row != 4 && row != 5) {
				for (int i = 1; i <= 9; i++)
					cells[i - 1][row] = cells[i][row];
				cells[9][row] = player;
			} else {
				for (int i = 1; i <= 8; i++)
					cells[i - 1][row] = cells[i][row];
				cells[8][row] = player;
			}

			//	DebugOutput();

		}

		public CellState check() {
			for (int line = 0; line < 9; line++) {
				for (int i = 0; i < 5; i++) {
					if (cells[line][i] == CellState.Cross || cells[line][i] == CellState.Zero) {
						bool isAll = true;
						for (int j = i + 1; j < i + 5; j++)
							if (cells[line][i] != cells[line][j])
								isAll = false;
						if (isAll)
							return cells[line][i];
					}
				}
			}

			for (int row = 0; row < 9; row++) {
				for (int i = 0; i < 5; i++) {
					if (cells[i][row] == CellState.Cross || cells[i][row] == CellState.Zero) {
						bool isAll = true;
						for (int j = i + 1; j < i + 5; j++)
							if (cells[i][row] != cells[j][row])
								isAll = false;
						if (isAll)
							return cells[row][i];
					}
				}
			}

			/*for (int diag = 0; diag <= 4; diag++) {
				int max_streak = 0;
				int streak = 0;
				for (int row = 0; row + diag < 10; row++) {
				}
			}*/
			
			return CellState.Empty;
		}

		public void DebugOutput() {
			for (int i = 0; i < 10; i++) {
				for (int j = 0; j < 10; j++) {
					if (cells[i][j] == CellState.Cross)
						Console.Write("x");
					if (cells[i][j] == CellState.Zero)
						Console.Write("O");
					if (cells[i][j] == CellState.Empty)
						Console.Write("_");
					if (cells[i][j] == CellState.Block)
						Console.Write("=");
				}
				Console.WriteLine(" ");
			}

		}

	}
}
