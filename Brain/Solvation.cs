using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brain {
	public class Solvation {
		Field field;
		Solvation last;

		CellState player;

		int row;

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

		public bool isMove {
			get {
				return canMove;
			}
		}

		public Solvation(Field field, CellState player) {
			this.field = field;
			this.player = player;
			last = null;
		}

		public Solvation(Solvation last, int row) {
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
				mathWait = player == res ? 1 : -1;
				last.toStart(loses, wins, mathWait);
			}
		}

		void toStart(int wins, int loses, float mathWait) {
			this.wins += wins;
			this.loses += loses;
			this.mathWait -= mathWait * 0.1f;
		}

		public static CellState invertCell(CellState player) {
			return (player == CellState.Cross) ? CellState.Zero : CellState.Cross;
		}

		public string getTurn() {
			return row.ToString();
		}

		public Solvation isGreater(Solvation other) {
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
		}

	}
}
