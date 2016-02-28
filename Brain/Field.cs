/*
 *************************************************************************
 * AI for "Connect Five" game.                                   *
 *                                                                       *
 * This program should be used for Connect Five Competition.             *
 * Connect Five is the game like Connect Four; for more information see  *
 * http://www.math.spbu.ru/user/chernishev/connectfive/connectfive.html  *
 *                                                                       *
 * Author: Grigory Pervakov                                               *
 * Email: pervakov.grigory@gmail.com                           *
 * Year: 2015                                                            *
 * See the LICENSE file in the project root for more information.        *
 *************************************************************************
*/


using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brain {

	/// <summary>
	/// This class stores one state of the game field
	/// and has some methods for manipulating it
	/// </summary>
	public class Field {

		public static int SIZE = 10, WINLINE = 5;
		public static int block1 = 4, block2 = 5;

		public CellState[][] cells;

		/// <summary>
		/// Constructor that loads the game field from the list of files
		/// </summary>
		/// <param name="folder">Path to the game folder</param>
		public Field(String folder) {

			cells = new CellState[SIZE][];
			for (int i = 0; i < SIZE; i++)
				cells[i] = new CellState[SIZE];

			cells[block1][SIZE - 1] = CellState.Block;
			cells[block2][SIZE - 1] = CellState.Block;

			int turns = Directory.GetFiles(folder).Length;

			for (int i = 1; i <= turns / 2; i++) {
				String s = File.ReadAllLines(folder + "X" + i + ".txt")[0];
				addSymb(CellState.Cross, Convert.ToInt32(s));

				s = File.ReadAllLines(folder + "O" + i + ".txt")[0];
				addSymb(CellState.Zero, Convert.ToInt32(s));
			}
			if (turns % 2 == 1) {
				String s = File.ReadAllLines(folder + "X" + (turns / 2 + 1) + ".txt")[0];
				addSymb(CellState.Cross, Convert.ToInt32(s));
			}
		}

		/// <summary>
		/// Constructor that copies specified field and make move to specified column
		/// </summary>
		/// <param name="last">Field to copy</param>
		/// <param name="col">Column to move</param>
		/// <param name="player">Player who moves</param>
		public Field(Field last, int col, CellState player) {
			cells = new CellState[SIZE][];
			for (int i = 0; i < SIZE; i++)
				cells[i] = last.cells[i];

			cells[col] = new CellState[SIZE];
			int delta = 0;
			if (col == block1 || col == block2) {
				delta = 1;
				cells[col][SIZE - 1] = CellState.Block;
			}
			cells[col][SIZE - 1 - delta] = player;

			for (int i = 0; i < SIZE - 1 - delta; i++)
				cells[col][i] = last.cells[col][i + 1];
		}

		/// <summary>
		/// Checks whether specified column is available to move
		/// </summary>
		public bool checkRow(int row) {
			return (cells[row][0] == CellState.Empty);
		}

		/// <summary>
		/// Performs the move to specified column
		/// </summary>
		private void addSymb(CellState player, int row) {
			if (row != block1 && row != block2) {
				for (int i = 1; i < SIZE; i++)
					cells[row][i - 1] = cells[row][i];
				cells[row][SIZE - 1] = player;
			} else {
				for (int i = 0; i < SIZE - 2; i++)
					cells[row][i] = cells[row][i + 1];
				cells[row][SIZE - 2] = player;
			}
		}

		/// <summary>
		/// Searching victory combinations
		/// </summary>
		/// <param name="col">Last changed column</param>
		/// <param name="player">First player to check</param>
		/// <returns>the result of checking</returns>
		public CellState CheckField(int col, CellState player) {
			if (check(col, player))
				return player;

			if (check(col, Solution.invertCell(player)))
				return Solution.invertCell(player);

			return CellState.Empty;
		}

		/// <summary>
		/// checks specified player for victory combination
		/// </summary>
		private bool check(int col, CellState player) {

			int sum = 0;

			for (int line = 0; line < SIZE - 1; line++) {
				sum = 0;
				for (int i = 0; i < WINLINE; i++)
					sum += (int)cells[i][line];
				if (sum / WINLINE == (int)player)
					return true;
				for (int dx = 0; dx < SIZE - WINLINE; dx++) {
					sum += (int)cells[dx + WINLINE][line] - (int)cells[dx][line];
					if (sum / WINLINE == (int)player)
						return true;
				}
			}

			int delta = 0;
			if (col == block1 || col == block2)
				delta = 1;

			sum = 0;
			for (int i = 0; i < WINLINE; i++)
				sum += (int)cells[i + delta][col];
			if (sum / WINLINE == (int)player)
				return true;

			for (int i = 0; i < SIZE; i++)
				if (cells[col][i] != CellState.Empty)
					if (checkDiagFromIt(col, i, player))
						return true;
			return false;
		}

		/// <summary>
		/// checks victory combination on diagonals for specified player
		/// </summary>
		private bool checkDiagFromIt(int col, int row, CellState player) {
			int dx = col - min(col, row);
			int dy = row - min(col, row);

			int sum = (int)cells[dx][dy];
			while (dx != SIZE - 1 && dy != SIZE - 1) {
				dx++;
				dy++;
				sum += (int)cells[dx][dy];

				if (min(dx, dy) >= WINLINE)
					sum -= (int)cells[dx - WINLINE][dy - WINLINE];

				if (sum / WINLINE == (int)player)
					return true;
			}

			dx = col - min(col, (SIZE - row - 1));
			dy = row + min(col, (SIZE - row - 1));

			sum = (int)cells[dx][dy];
			while (dx != SIZE - 1 && dy != 0) {
				dx++;
				dy--;
				sum += (int)cells[dx][dy];

				if (min(dx, (SIZE - 1 - dy)) >= WINLINE)
					sum -= (int)cells[dx - WINLINE][dy + WINLINE];

				if (sum / WINLINE == (int)player)
					return true;
			}

			return false;
		}

		/// <returns>minimal of two</returns>
		private static int min(int a, int b) {
			return a < b ? a : b;
		}

		/// <summary>
		/// prints this field state in standart output
		/// </summary>
		public void DebugOutput() {
			for (int i = 0; i < SIZE; i++) {
				for (int j = 0; j < SIZE; j++) {
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