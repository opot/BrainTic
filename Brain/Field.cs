using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brain {
	public class Field {
		CellState[][] cells;

		public Field(String folder) {

			cells = new CellState[10][];
			for (int i = 0; i < 10; i++)
				cells[i] = new CellState[10];
		
			cells[4][9] = CellState.Block;
			cells[5][9] = CellState.Block;

			int turns = Directory.GetFiles(folder).Length;

			for(int i = 1; i <= turns/2; i++) {
				String s = File.ReadAllLines(folder+"X"+i+".txt")[0];
				addSymb(CellState.Cross, Convert.ToInt32(s));
				
				s = File.ReadAllLines(folder + "O" + i + ".txt")[0];
				addSymb(CellState.Zero, Convert.ToInt32(s));
			}
			if(turns % 2 == 1) {
				String s = File.ReadAllLines(folder+"X"+(turns/2+1)+".txt")[0];
				addSymb(CellState.Cross, Convert.ToInt32(s));
			}
		}

		public Field(Field last, int row, CellState player) {
			cells = new CellState[10][];
			for (int i = 0; i < 10; i++)
				cells[i] = last.cells[i];

			cells[row] = new CellState[10];
			int delta = 0;
			if (row == 4 || row == 5) {
				delta = 1;
				cells[row][9] = CellState.Block;
			}
			cells[row][9-delta] = player;

			for (int i = 0; i < 9 - delta; i++)
				cells[row][i] = last.cells[row][i + 1];
		}

		public bool checkRow(int row) {
			return (cells[row][0] == CellState.Empty);
		}

		private void addSymb(CellState player, int row) {
			if (row != 4 && row != 5) {
				for (int i = 1; i <= 9; i++)
					cells[row][i - 1] = cells[row][i];
				cells[row][9] = player;
			} else {
				for (int i = 0; i < 8; i++)
					cells[row][i] = cells[row][i + 1];
				cells[row][8] = player;
			}
		}

		public CellState check(int row) {

			for (int line = 0; line < 9; line++) {
				for (int i = 0; i < 5; i++) {
					if (cells[i][line] == CellState.Cross || cells[i][line] == CellState.Zero) {
						bool isAll = true;
						for (int j = i + 1; j < i + 5; j++)
							if (cells[i][line] != cells[j][line])
								isAll = false;
						if (isAll)
							return cells[i][line];
					}
				}
			}
			for (int line = 0; line < 9; line++) {
				int sum = (int)cells[0][line] + (int)cells[1][line] + 
						  (int)cells[2][line] + (int)cells[3][line] + 
						  (int)cells[4][line];
				if (sum == 5 || sum == -5)
					return (CellState)(sum / 5);
                for (int dx = 0; dx < 5; dx++) {
					sum += (int)cells[dx + 5][line] - (int)cells[dx][line];
					if (sum == 5 || sum == -5)
						return (CellState)(sum / 5);
				}
			}

					int delta = 0;
			if(row == 4 || row == 5)
				delta = 1;

			if (cells[row][9 - delta] == cells[row][8 - delta] &&
				cells[row][9 - delta] == cells[row][7 - delta] &&
				cells[row][9 - delta] == cells[row][6 - delta] &&
				cells[row][9 - delta] == cells[row][5 - delta])
				return cells[row][8];

			for(int i = 0; i < 10 - delta; i++)
				if(cells[row][i] != CellState.Empty) {
					CellState buf = checkDiagFromIt(row, i);
					if (buf != CellState.Empty)
						return buf;
				}
			
			return CellState.Empty;
		}

		private CellState checkDiagFromIt(int row, int line) {
			int dx = row, dy = line;
			while(dx != 0 && dy != 0) {
				dx--;
				dy--;
			}

			int sum = 0;
			while(dx != 9 && dy != 9) {
				sum += (int)cells[dx][dy];
				dx++;
				dy++;

				if (min(dx, dy) >= 5)
					sum -= (int)cells[dx - 5][dy - 5];

				if (sum == 5 || sum == -5)
					return (CellState)(sum / 5);
			}

			dx = row;
			dy = line;
			while (dx != 0 && dy != 9) {
				dx--;
				dy++;
			}

			sum = 0;
			while (dx != 9 && dy != 0) {
				sum += (int)cells[dx][dy];
				dx++;
				dy--;

				if (min(dx, (9 - dy)) >= 5)
					sum -= (int)cells[dx - 5][dy + 5];

				if (sum == 5 || sum == -5)
					return (CellState)(sum / 5);
			}

			return CellState.Empty;
		}

		private int min(int a, int b) {
			return a < b ? a : b;
		}

		public void DebugOutput() {
			for (int i = 0; i < 10; i++) {
				for (int j = 0; j < 10; j++) {
					if (cells[j][i] == CellState.Cross)
						Console.Write("x");
					if (cells[j][i] == CellState.Zero)
						Console.Write("O");
					if (cells[j][i] == CellState.Empty)
						Console.Write("_");
					if (cells[j][i] == CellState.Block)
						Console.Write("=");
				}
				Console.WriteLine();
			}

		}

	}
}
