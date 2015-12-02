using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brain {
	public class Solution {
		Field field;
		Solution last;

		CellState player;

		int row;

		int wins = 0;
		int loses = 0;
		double mathWait = 0;

		private bool canMove = true;
		private bool Finaized = false;

		public bool isMove {
			get {
				return canMove;
			}
		}
		public bool isFinalized {
			get {
				return Finaized;
			}
		}

		public Solution(Field field, CellState player) {
			this.field = field;
			this.player = player;
			last = null;
		}

		public Solution(Solution last, int row) {
			this.row = row;
			this.player = invertCell(last.player);
			if (!last.field.checkRow(row)) {
				Finaized = true;
				canMove = false;
				return;
			}

			field = new Field(last.field, row, player);
			this.last = last;
			CellState res = field.CheckField(row, player);
			if (res != CellState.Empty) {
				wins += player == res ? 1 : 0;
				loses += player == res ? 0 : 1;
				Finaized = true;
				mathWait = (player == res) ? 1 : -1;
				last.toStart(loses, wins, mathWait);
			}
		}

		void toStart(int wins, int loses, double mathWait) {
			this.wins += wins;
			this.loses += loses;
			this.mathWait -= mathWait;

			if(last != null)
				last.toStart(loses, wins, mathWait/100);
		}

		public static CellState invertCell(CellState player) {
			return (CellState)(-(int)player);
		}

		public string getTurn() {
			return row.ToString();
		}

		public Solution isGreater(Solution other) {

			if (other == null)
				return this;

			if (mathWait > other.mathWait)
				return this;
			if (mathWait < other.mathWait)
				return other;

			if (wins > other.wins)
				return this;
			if (wins < other.wins)
				return other;

			if (loses < other.loses)
				return this;

			return other;

		}

	}
}
