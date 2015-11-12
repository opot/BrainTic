﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brain {
	struct Field {
		CellState[,] cells;

		public Field(String folder,CellState player) {

			cells = new CellState[10,10];
			for (int i = 0; i < 10; i++) {
				for (int j = 0; j < 10; j++)
					cells[i,j] = CellState.Empty;
			}
			cells[9,4] = CellState.Block;
			cells[9,5] = CellState.Block;

			String[] xMoves;
			String[] oMoves;
			if (player == CellState.Cross) {
				xMoves = Directory.GetFiles(folder);
				oMoves = Directory.GetFiles(folder + "o");
			} else {
				oMoves = Directory.GetFiles(folder);
				xMoves = Directory.GetFiles(folder + "x");
			}

			for(int i = 0; i < oMoves.Length; i++) {
				String s = File.ReadAllLines(xMoves[i])[0];
				addSymb(CellState.Cross, Convert.ToInt32(s));

				s = File.ReadAllLines(oMoves[i])[0];
				addSymb(CellState.Zero, Convert.ToInt32(s));
			}
			if(player == CellState.Zero) {
				String s = File.ReadAllLines(xMoves[xMoves.Length-1])[0];
				addSymb(CellState.Cross, Convert.ToInt32(s));
			}

			Console.WriteLine(" ");
			DebugOutput();

		}

		public Field(Field last, int row, CellState player) {
			cells = new CellState[10,10];
			for (int i = 0; i < 10; i++)
				for (int j = 0; j < 10; j++)
					cells[i, j] = last.cells[i, j];
			this.addSymb(player, row);
		}

		private void addSymb(CellState player, int row) {
			if (row != 4 && row != 5) {
				for (int i = 1; i <= 9; i++)
					cells[i - 1, row] = cells[i, row];
				cells[9, row] = player;
			} else {
				for (int i = 1; i <= 8; i++)
					cells[i - 1, row] = cells[i, row];
				cells[8, row] = player;
			}

			//	DebugOutput();

		}

		public bool checkRow(int row) {
			return (cells[0, row] == CellState.Empty);
		}

		public CellState check() {
			for (int line = 0; line < 9; line++) {
				for (int i = 0; i < 5; i++) {
					if (cells[line, i] == CellState.Cross || cells[line, i] == CellState.Zero) {
						bool isAll = true;
						for (int j = i + 1; j < i + 5; j++)
							if (cells[line, i] != cells[line, j])
								isAll = false;
						if (isAll)
							return cells[line, i];
					}
				}
			}

			for (int row = 0; row < 9; row++) {
				for (int i = 0; i < 5; i++) {
					if (cells[i, row] == CellState.Cross || cells[i, row] == CellState.Zero) {
						bool isAll = true;
						for (int j = i + 1; j < i + 5; j++)
							if (cells[i, row] != cells[j, row])
								isAll = false;
						if (isAll)
							return cells[row, i];
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
					if (cells[i, j] == CellState.Cross)
						Console.Write("x");
					if (cells[i, j] == CellState.Zero)
						Console.Write("O");
					if (cells[i, j] == CellState.Empty)
						Console.Write("_");
					if (cells[i, j] == CellState.Block)
						Console.Write("=");
				}
				Console.WriteLine();
			}

		}

	}
}