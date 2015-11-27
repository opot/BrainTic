using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brain {
	class Field {
		CellState[][] cells;

		public Field(String folder) {

			cells = new CellState[10][];
			for (int i = 0; i < 10; i++)
				cells[i] = new CellState[10];
		
			cells[4][9] = CellState.Block;
			cells[5][9] = CellState.Block;

			DebugOutput();
			String[] moves = Directory.GetFiles(folder);

			for(int i = 0; i < moves.Length/2; i++) {
				String s = File.ReadAllLines(moves[i])[0];
				addNoChangeSymb(CellState.Cross, Convert.ToInt32(s));

				s = File.ReadAllLines(moves[i+1])[0];
				addNoChangeSymb(CellState.Zero, Convert.ToInt32(s));
			}
			if(moves.Length % 2 == 1) {
				String s = File.ReadAllLines(moves[moves.Length-1])[0];
				addNoChangeSymb(CellState.Cross, Convert.ToInt32(s));
			}

			Console.WriteLine(" ");
			DebugOutput();

		}

		private void addNoChangeSymb(CellState player, int row) {
			if (row != 4 && row != 5) {
				for (int i = 0; i < 9; i++)
					cells[row][i] = cells[row][i + 1];
				cells[row][9] = player;
			} else {
				for (int i = 0; i < 8; i++)
					cells[row][i] = cells[row][i + 1];
				cells[row][8] = player;
			}
		}

		public Field(Field last, int row, CellState player) {
			cells = new CellState[10][];
			for (int i = 0; i < 10; i++)
				cells[i] = last.cells[i];

			cells[row] = new CellState[10];
			for (int i = 1; i < 9; i++)
				cells[row][i] = last.cells[row][i-1];
			cells[row][9] = player;
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
				for (int i = 1; i <= 8; i++)
					cells[row][i - 1] = cells[row][i];
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

			int delta = 0;
			if(row == 4 || row == 5)
				delta = 1;

				if (cells[row][delta] != CellState.Empty)
					if (cells[row][delta] == cells[row][delta + 1] &&
					   cells[row][delta] == cells[row][delta + 2] &&
					   cells[row][delta] == cells[row][delta + 3] &&
					   cells[row][delta] == cells[row][delta + 4])
						return cells[row][delta];

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

		public class Solvation {
			Field field;
			Solvation last;

			CellState player;

			int row = 0;

			int wins = 0;
			int loses = 0;

			float mathWait = 0;

			private bool canMove = true;
			private bool Finaized = false;
			public bool isFinalized {
				get {
					return Finaized;
				}
			} 

			public Solvation(Field field, CellState player) {
				this.field = field;
				this.player = player;
				last = null;
			}

			public Solvation(Solvation last, int row){
				this.row = row;
				this.player = invertCell(last.player);
				if (!last.field.checkRow(row)) {
					Finaized = true;
					canMove = false;
					return;
				}

				field = new Field(last.field, row, player);
				this.last = last;
				CellState res = field.check(row);
				if (res != CellState.Empty) {
					wins += player == res ? 1 : 0;
					loses += player == res ? 0 : 1;
					Finaized = true;
					mathWait = player == res? 1:-1;
					last.toStart(loses, wins, mathWait);
				}
			}

			void toStart(int wins, int loses, float mathWait) {
				this.wins += wins;
				this.loses += loses;
				this.mathWait -= mathWait * 0.1f;
			}

			private CellState invertCell(CellState player) {
				return (player == CellState.Cross) ? CellState.Zero : CellState.Cross;
			}

			public string getTurn() {
				return row.ToString();
			}

			public Solvation isGreater(Solvation other) {
				if(this.canMove && other.canMove) {
					if (mathWait > other.mathWait)
						return this;
					if (mathWait < other.mathWait)
						return other;

					if (wins > other.wins)
						return this;
					if (wins < other.wins)
						return other;

					if (loses > other.loses)
						return this;
					if (loses < other.loses)
						return other;

					return this;
				} else {
					return canMove?this:other;
				}
			}

		}

	}
}
